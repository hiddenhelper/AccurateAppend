using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using AccurateAppend.Core;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Data;
using AccurateAppend.Messaging;
using AccurateAppend.Sales;
using AccurateAppend.Sales.Contracts.Messages;
using DomainModel.Messages;
using NServiceBus;

namespace AccurateAppend.Websites.Admin.Messages.Accounts
{
    /// <summary>
    /// Saga designed to process the Subscription billing automation process.
    /// </summary>
    public class SubscriptionBillingSaga : Saga<SubscriptionBillingData>,
        IAmStartedByMessages<CreateSubscriptionCommand>,
        IAmStartedByMessages<ResumeSubscriptionCommand>,
        IAmStartedByMessages<CancelSubscriptionCommand>,
        IAmStartedByMessages<DealCanceledEvent>,
        IHandleTimeouts<BillingPeriodClosedTimeout>
    {
        #region Fields

        private readonly AccurateAppend.Sales.DataAccess.DefaultContext dataContext;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionBillingSaga"/> class.
        /// </summary>
        /// <param name="dataContext">The <see cref="AccurateAppend.Sales.DataAccess.DefaultContext"/> providing data access to the handler.</param>
        public SubscriptionBillingSaga(AccurateAppend.Sales.DataAccess.DefaultContext dataContext)
        {
            if (dataContext == null) throw new ArgumentNullException(nameof(dataContext));
            Contract.EndContractBlock();

            this.dataContext = dataContext;
        }

        #endregion

        #region Overrides

        /// <inheritdoc />
        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<SubscriptionBillingData> mapper)
        {
            mapper.ConfigureMapping<CreateSubscriptionCommand>(message => message.PublicKey).ToSaga(saga => saga.PublicKey);
            mapper.ConfigureMapping<CancelSubscriptionCommand>(message => message.PublicKey).ToSaga(saga => saga.PublicKey);
            mapper.ConfigureMapping<ResumeSubscriptionCommand>(message => message.PublicKey).ToSaga(saga => saga.PublicKey);
            mapper.ConfigureMapping<DealCanceledEvent>(message => message.BillingAccount).ToSaga(saga => saga.PublicKey);
        }

        #endregion

        #region IHandleMessages<CreateSubscriptionCommand> Members

        /// <inheritdoc />
        public virtual async Task Handle(CreateSubscriptionCommand message, IMessageHandlerContext context)
        {
            Validator.ValidateObject(message, new ValidationContext(message));

            using (context.Alias())
            {
                var existingAccount = await this.dataContext
                    .SetOf<RecurringBillingAccount>()
                    .Where(s => s.ForClient.UserId == message.UserId)
                    .OrderByDescending(s => s.EffectiveDate)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);

                // Ensures we've canceled the other account prior to creating the new one
                if (existingAccount != null)
                {
                    if (existingAccount.EndDate == null || existingAccount.EndDate >= message.EffectiveDate)
                        throw new InvalidOperationException($"Cannot create a new subscription starting {message.EffectiveDate} that overlaps with existing account {existingAccount.Id}");
                }

                // Create the subscription
                var client = await this.dataContext
                    .SetOf<ClientRef>()
                    .FirstAsync(c => c.UserId == message.UserId)
                    .ConfigureAwait(false);
                var subscription = new SubscriptionBilling(client, message.PublicKey, message.Prepayment, message.EffectiveDate, message.EndDate);
                subscription.Recurrence = message.Cycle;
                subscription.SpecialProcessing = message.HasCustomBilling;
                subscription.ApplyLimit(message.MaxBalance);
                subscription.FixedRate = message.IsFixedRate;

                this.dataContext.SetOf<SubscriptionBilling>().Add(subscription);

                await this.dataContext.SaveChangesAsync().ConfigureAwait(false);
            
                // Store the data we need
                this.Data.PublicKey = message.PublicKey;
                this.Data.SubscriptionId = subscription.Id.Value;

                // Raise the event
                var @event = new SubscriptionCreatedEvent();
                @event.SubscriptionId = subscription.Id.Value;
                @event.PublicKey = subscription.PublicKey;
                @event.StartingDate = subscription.EffectiveDate;
                @event.EndingDate = subscription.EndDate;

                await context.Publish(@event).ConfigureAwait(false);

                // Reset the client state historgram on new data
                var command = new ResetClientHistorgramCommand() {UserId = message.UserId};
                await context.Send(command).ConfigureAwait(false);

                await this.WaitIfNeeded(context, subscription, subscription.EffectiveDate.AddDays(-1)).ConfigureAwait(false);
            }
        }

        #endregion

        #region BILLING TIMEOUT

        /// <inheritdoc />
        public async Task Timeout(BillingPeriodClosedTimeout state, IMessageHandlerContext context)
        {
            // Automation based billing is always done as the system
            using (SecurityHelper.Alias(WellKnownIdentifiers.SystemIdentity))
            {
                if (state.Period.Type != LedgerType.ForSubscriptionPeriod) throw new InvalidOperationException($"Prepayment billing timeout for {this.Data.SubscriptionId} received invalid billing instruction for {state.Period.Type}");

                var subscription = await this.dataContext
                    .SetOf<SubscriptionBilling>()
                    .SingleOrDefaultAsync(s => s.Id == this.Data.SubscriptionId)
                    .ConfigureAwait(false);

                // Subscription doesn't exist so we just clean ourselves up and leave
                if (subscription == null)
                {
                    this.MarkAsComplete();
                    return;
                }

                if (subscription.EndDate == null || subscription.EndDate >= state.Period.StartingOn)
                {
                    // Send a message to subscription bill creation
                    var command = new CreateSubscriptionBillCommand();
                    command.Period = state.Period;
                    command.Id = this.Data.SubscriptionId;

                    await context.SendLocal(command).ConfigureAwait(false);
                }

                await this.WaitIfNeeded(context, subscription, state.Period.EndingOn).ConfigureAwait(false);
            }
        }

        #endregion

        #region IHandleMessages<CancelSubscriptionCommand> Members

        /// <inheritdoc />
        public virtual async Task Handle(CancelSubscriptionCommand message, IMessageHandlerContext context)
        {
            var subscription = await this.dataContext
                .SetOf<SubscriptionBilling>()
                .Where(s => s.PublicKey == message.PublicKey)
                .Include(s => s.ForClient)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            // Subscription doesn't exist so we just clean ourselves up and leave
            if (subscription == null)
            {
                this.MarkAsComplete();
                return;
            }

            // Store the data we need
            this.Data.PublicKey = message.PublicKey;
            this.Data.SubscriptionId = subscription.Id.Value;

            // Confirm the new end date hasn't been billed through otherwise bump end date to that time
            var latestPeriod = await this.dataContext
                .SetOf<LedgerEntry>()
                .Where(l => l.ForAccount.PublicKey == message.PublicKey)
                .Where(l => l.Classification == LedgerType.ForSubscriptionPeriod)
                .OrderByDescending(l => l.PeriodEnd)
                .Select(l => (DateTime?) l.PeriodEnd)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (latestPeriod > message.EndDate) message.EndDate = latestPeriod.Value;

            var command = new ResetClientHistorgramCommand() {UserId = subscription.ForClient.UserId};
            await context.Send(command);

            // If the subscription hasn't even started then just nuke it from orbit-nothing is billed
            if (subscription.EffectiveDate > message.EndDate)
            {
                this.dataContext.SetOf<SubscriptionBilling>().Remove(subscription);
                this.MarkAsComplete();
            }
            else
            {
                // Expire the account on the day indicated
                subscription.Expire(message.EndDate);

                await this.WaitIfNeeded(context, subscription, latestPeriod ?? subscription.EffectiveDate.AddDays(-1)).ConfigureAwait(false);
            }

            await this.dataContext.SaveChangesAsync().ConfigureAwait(false);

            // Raise event
            var @event = new SubscriptionCanceledEvent();
            @event.SubscriptionId = subscription.Id.Value;
            @event.PublicKey = subscription.PublicKey;

            await context.Publish(@event).ConfigureAwait(false);
        }

        #endregion

        #region IHandleMessages<ResumeSubscriptionCommand> Members

        /// <inheritdoc />
        public async Task Handle(ResumeSubscriptionCommand message, IMessageHandlerContext context)
        {
            var subscription = await this.dataContext
                .SetOf<SubscriptionBilling>()
                .FirstAsync(s => s.PublicKey == message.PublicKey)
                .ConfigureAwait(false);
            this.Data.SubscriptionId = subscription.Id.Value;
            this.Data.PublicKey = subscription.PublicKey;

            var latestPeriod = await this.dataContext
                .SetOf<LedgerEntry>()
                .Where(l => l.ForAccount.PublicKey == message.PublicKey)
                .Where(l => l.Classification == LedgerType.ForSubscriptionPeriod)
                .OrderByDescending(l => l.PeriodEnd)
                .Select(l => (DateTime?)l.PeriodEnd)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            // We're resuming a never billed subscription so we set it to the day before start
            latestPeriod = latestPeriod ?? subscription.EffectiveDate.AddDays(-1);

            await WaitIfNeeded(context, subscription, latestPeriod.Value).ConfigureAwait(false);
        }

        #endregion

        #region IHandleMessages<DealCanceledEvent> Members

        /// <inheritdoc />
        public virtual async Task Handle(DealCanceledEvent message, IMessageHandlerContext context)
        {
            var subscription = await this.dataContext
                .SetOf<SubscriptionBilling>()
                .Where(s => s.PublicKey == message.BillingAccount)
                .Include(s => s.ForClient)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            // Subscription doesn't exist so we just clean ourselves up and leave
            if (subscription == null)
            {
                this.MarkAsComplete();
                return;
            }

            // Store the data we need
            this.Data.PublicKey = message.BillingAccount;
            this.Data.SubscriptionId = subscription.Id.Value;

            var ledgerItem = await this.dataContext
                .SetOf<LedgerEntry>()
                .Where(e => e.WithDeal.Id == message.DealId)
                .Where(e => e.Classification == LedgerType.ForSubscriptionPeriod)
                .SingleOrDefaultAsync()
                .ConfigureAwait(false);

            // Ledger doesn't exist so we just clean ourselves up and leave
            if (ledgerItem == null)
            {
                this.MarkAsComplete();
                return;
            }

            // Figure out the billing period this ledger entry covered based and schedule a timeout (this will immediately fire one the handler completes)
            var period = new DateSpan(ledgerItem.PeriodStart, ledgerItem.PeriodEnd);
            
            await this.WaitIfNeeded(context, subscription, period.StartingOn.AddDays(-1)).ConfigureAwait(false);

            // Nuke the ledger entry-this will be replaced
            this.dataContext
                .SetOf<LedgerEntry>()
                .Remove(ledgerItem);

            await this.dataContext.SaveChangesAsync().ConfigureAwait(false);
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
            timeout.Period.Type = LedgerType.ForSubscriptionPeriod;
            timeout.Period.PaidThrough = closeOfPeriod.StartingOn.AddDays(-1);
            timeout.Period.WaitUntilPeriodClose = false;
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
                // Sleep until we should bill this - note we skew 1 hour JIC
                await this.RequestTimeout(context, timeout.Period.StartingOn.AddHours(1), timeout).ConfigureAwait(false);
            }
        }

        #endregion
    }
}