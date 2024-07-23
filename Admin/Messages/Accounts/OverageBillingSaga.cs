using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using AccurateAppend.Core;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Data;
using AccurateAppend.Sales;
using AccurateAppend.Sales.Contracts.Messages;
using AccurateAppend.Sales.DataAccess;
using NServiceBus;

namespace AccurateAppend.Websites.Admin.Messages.Accounts
{
    /// <summary>
    /// Saga designed to process the Usage Only billing automation process.
    /// </summary>
    public class OverageBillingSaga : Saga<OverageBillingData>,
        IAmStartedByMessages<SubscriptionCreatedEvent>,
        IHandleMessages<SubscriptionCanceledEvent>,
        IAmStartedByMessages<ResumeOverageCommand>,
        IHandleTimeouts<BillingPeriodClosedTimeout>
    {
        #region Fields

        private readonly DefaultContext dataContext;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="OverageBillingSaga"/> class.
        /// </summary>
        /// <param name="dataContext">The required <see cref="DefaultContext"/> component.</param>
        public OverageBillingSaga(DefaultContext dataContext)
        {
            if (dataContext == null) throw new ArgumentNullException(nameof(dataContext));
            Contract.EndContractBlock();

            this.dataContext = dataContext;
        }

        #endregion

        #region Overrides

        /// <inheritdoc />
        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<OverageBillingData> mapper)
        {
            mapper.ConfigureMapping<SubscriptionCreatedEvent>(message => message.SubscriptionId).ToSaga(saga => saga.SubscriptionId);
            mapper.ConfigureMapping<SubscriptionCanceledEvent>(message => message.SubscriptionId).ToSaga(saga => saga.SubscriptionId);
            mapper.ConfigureMapping<ResumeOverageCommand>(message => message.SubscriptionId).ToSaga(saga => saga.SubscriptionId);
        }

        #endregion

        #region IHandleMessages<SubscriptionCreatedEvent> Members

        /// <inheritdoc />
        public virtual async Task Handle(SubscriptionCreatedEvent message, IMessageHandlerContext context)
        {
            // Store the data we need
            this.Data.PublicKey = message.PublicKey;
            this.Data.SubscriptionId = message.SubscriptionId;

            var account = await this.dataContext
                .SetOf<SubscriptionBilling>()
                .Where(s => s.Id == message.SubscriptionId)
                .FirstAsync()
                .ConfigureAwait(false);

            // Fixed rate accounts don't need overage
            if (account.FixedRate)
            {
                this.MarkAsComplete();
                return;
            }
            await this.WaitIfNeeded(context, account, account.EffectiveDate.AddDays(-1));
        }

        #endregion

        #region BILLING TIMEOUT

        /// <inheritdoc />
        public async Task Timeout(BillingPeriodClosedTimeout state, IMessageHandlerContext context)
        {
            // Automation based billing is always done as the system
            using (SecurityHelper.Alias(WellKnownIdentifiers.SystemIdentity))
            {
                if (state.Period.Type != LedgerType.ForUsage) throw new InvalidOperationException($"Overage billing timeout for {this.Data.SubscriptionId} received invalid billing instruction for {state.Period.Type}");

                var usage = await this.dataContext
                    .SetOf<SubscriptionBilling>()
                    .SingleOrDefaultAsync(s => s.Id == this.Data.SubscriptionId)
                    .ConfigureAwait(false);

                // Account doesn't exist so we just clean ourselves up and leave
                if (usage == null)
                {
                    this.MarkAsComplete();
                    return;
                }

                if (usage.EndDate == null || usage.EndDate >= state.Period.StartingOn)
                {
                    // Send a message to usage bill creation
                    var command = new CreateUsageBillCommand();
                    command.Period = state.Period;
                    command.Id = this.Data.SubscriptionId;

                    await context.SendLocal(command);
                }

                await this.WaitIfNeeded(context, usage, state.Period.EndingOn);
            }
        }

        #endregion

        #region IHandleMessages<SubscriptionCanceledEvent> Members

        /// <inheritdoc />
        public virtual async Task Handle(SubscriptionCanceledEvent message, IMessageHandlerContext context)
        {
            var subscriptionId = message.SubscriptionId;

            var account = await this.dataContext
                .SetOf<SubscriptionBilling>()
                .Where(s => s.Id == subscriptionId)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            // Account doesn't exist so we just clean ourselves up and leave. This means the subscription
            // started in the future so it was just removed entirely when it was canceled.
            if (account == null)
            {
                this.MarkAsComplete();
                return;
            }
           
            // Confirm the new end date hasn't been billed through otherwise bump end date to that time.
            // This shouldn't happen unless there's been an interim billing situation.
            var latestPeriod = await this.dataContext
                .SetOf<LedgerEntry>()
                .Where(l => l.ForAccount.PublicKey == message.PublicKey)
                .Where(l => l.Classification == LedgerType.ForUsage)
                .OrderByDescending(l => l.PeriodEnd)
                .Select(l => (DateTime?)l.PeriodEnd)
                .FirstOrDefaultAsync();

            // If the account hasn't even been billed ever just go with the existing timeout.
            if (latestPeriod == null) return;

            // Let's figure out what the next unbilled period is then
            var nextPeriod = this.DetermineEndOfPeriod(account, latestPeriod.Value);

            // Not a real period so we can just leave
            if (!nextPeriod.IsLogicalDateRange())
            {
                this.MarkAsComplete();
                return;
            }

            // If the cancellation lays outside of the next period we can just let things continue as is
            if (!nextPeriod.Within(message.EndingDate) || nextPeriod.EndingOn == message.EndingDate) return;

            // we have a foreshortened period so what we can do is schedule and the saga will finish up prior to the existing timeout
            await this.WaitIfNeeded(context, account, latestPeriod.Value);
        }

        #endregion

        #region IHandleMessages<ResumeOverageCommand> Members

        /// <inheritdoc />
        public async Task Handle(ResumeOverageCommand message, IMessageHandlerContext context)
        {
            var subscription = await this.dataContext
                .SetOf<SubscriptionBilling>()
                .FirstAsync(s => s.Id == message.SubscriptionId)
                .ConfigureAwait(false);
            this.Data.SubscriptionId = subscription.Id.Value;
            this.Data.PublicKey = subscription.PublicKey;

            if (subscription.FixedRate || subscription.EndDate < new DateTime(2019, 7, 1))
            {
                this.MarkAsComplete();
                return;
            }
            var latestPeriod = await this.dataContext
                .SetOf<LedgerEntry>()
                .Where(l => l.ForAccount.Id == message.SubscriptionId)
                .Where(l => l.Classification == LedgerType.ForUsage || l.Classification == LedgerType.GeneralAdjustment)
                .OrderByDescending(l => l.PeriodEnd)
                .Select(l => (DateTime?)l.PeriodEnd)
                .FirstOrDefaultAsync();

            // We're resuming a never billed subscription so we set it to the day before start
            latestPeriod = latestPeriod ?? subscription.EffectiveDate.AddDays(-1);

            await WaitIfNeeded(context, subscription, latestPeriod.Value);
        }

        #endregion

        #region Helpers

        private DateSpan DetermineEndOfPeriod(SubscriptionBilling subscriptionBilling, DateTime lastDayOfPreviousPeriod)
        {
            if (subscriptionBilling.EndDate <= lastDayOfPreviousPeriod) return new DateSpan(lastDayOfPreviousPeriod.AddDays(1), subscriptionBilling.EffectiveDate);

            var nextPeriod = TemporalHelper.CalculateEndOfPeriod(subscriptionBilling.EffectiveDate, subscriptionBilling.EndDate, lastDayOfPreviousPeriod.AddDays(1), period: subscriptionBilling.Recurrence);

            Console.WriteLine($"Next period calculated as {nextPeriod}");
            nextPeriod.ToSafeLocal();

            // Trim to end of subscription, if needed
            if (nextPeriod.EndingOn > subscriptionBilling.EndDate) nextPeriod.EndingOn = subscriptionBilling.EndDate.Value;

            return nextPeriod;
        }

        private BillingPeriodClosedTimeout NextTimeOut(SubscriptionBilling subscriptionBilling, DateTime lastDayOfPreviousPeriod)
        {
            var closeOfPeriod = this.DetermineEndOfPeriod(subscriptionBilling, lastDayOfPreviousPeriod);
            if (!closeOfPeriod.IsLogicalDateRange()) return null;

            var timeout = new BillingPeriodClosedTimeout();
            timeout.Period = new BillingPeriod();
            timeout.Period.Type = LedgerType.ForUsage;
            timeout.Period.PaidThrough = closeOfPeriod.StartingOn.AddDays(-1);
            timeout.Period.WaitUntilPeriodClose = true;
            timeout.Period.StartingOn = closeOfPeriod.StartingOn;
            timeout.Period.EndingOn = closeOfPeriod.EndingOn;

            return timeout;
        }

        private async Task WaitIfNeeded(IMessageHandlerContext context, SubscriptionBilling subscriptionBilling, DateTime lastDayOfPreviousPeriod)
        {
            // How long till we bill again?
            var timeout = this.NextTimeOut(subscriptionBilling, lastDayOfPreviousPeriod);

            if (timeout == null)
            {
                // All done billing. Finish this saga
                this.MarkAsComplete();
            }
            else
            {
                // Sleep until we should bill this AFTER the close of the period - note we skew 7 hours JIC
                var wait = timeout.Period.EndingOn.AddDays(1).AddHours(7).ToBillingZone().ToUniversalTime();
                await this.RequestTimeout(context, wait, timeout).ConfigureAwait(false);
            }
        }

        #endregion
    }
}