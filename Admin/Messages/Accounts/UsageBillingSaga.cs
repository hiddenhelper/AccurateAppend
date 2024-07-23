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
using AccurateAppend.Sales.DataAccess;
using DomainModel.Messages;
using NServiceBus;

namespace AccurateAppend.Websites.Admin.Messages.Accounts
{
    /// <summary>
    /// Saga designed to process the Usage Only billing automation process.
    /// </summary>
    public class UsageBillingSaga : Saga<UsageBillingData>,
        IAmStartedByMessages<CreateUsageOnlyCommand>,
        IHandleMessages<CancelUsageOnlyCommand>,
        IAmStartedByMessages<ResumeUsageCommand>,
        IHandleTimeouts<BillingPeriodClosedTimeout>
    {
        #region Fields

        private readonly DefaultContext dataContext;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="UsageBillingSaga"/> class.
        /// </summary>
        /// <param name="dataContext">The required <see cref="ISessionContext"/> component.</param>
        public UsageBillingSaga(DefaultContext dataContext)
        {
            if (dataContext == null) throw new ArgumentNullException(nameof(dataContext));
            Contract.EndContractBlock();

            this.dataContext = dataContext;
        }

        #endregion

        #region Overrides

        /// <inheritdoc />
        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<UsageBillingData> mapper)
        {
            mapper.ConfigureMapping<CreateUsageOnlyCommand>(message => message.PublicKey).ToSaga(saga => saga.PublicKey);
            mapper.ConfigureMapping<CancelUsageOnlyCommand>(message => message.PublicKey).ToSaga(saga => saga.PublicKey);
            mapper.ConfigureMapping<ResumeUsageCommand>(message => message.PublicKey).ToSaga(saga => saga.PublicKey);
        }

        #endregion

        #region IHandleMessages<CreateUsageOnlyCommand> Members

        /// <inheritdoc />
        public virtual async Task Handle(CreateUsageOnlyCommand message, IMessageHandlerContext context)
        {
            this.Data.PublicKey = message.PublicKey;
            
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
                        throw new InvalidOperationException($"Cannot create a new usage billing starting {message.EffectiveDate} that overlaps with existing account {existingAccount.Id}");
                }

                // Create the usage only
                var client = await this.dataContext
                    .SetOf<ClientRef>()
                    .FirstAsync(c => c.UserId == message.UserId);

                var account = new UsageBilling(client, message.PublicKey, message.EffectiveDate, message.EndDate);
                account.Recurrence = message.Cycle;
                account.SpecialProcessing = message.HasCustomBilling;
                account.ApplyLimit(message.MaxBalance);

                this.dataContext.SetOf<UsageBilling>().Add(account);

                await this.dataContext.SaveChangesAsync();

                // Store the data we need
                this.Data.PublicKey = message.PublicKey;
                this.Data.SubscriptionId = account.Id.Value;

                // Reset the client state historgram on new data
                var command = new ResetClientHistorgramCommand() { UserId = message.UserId };
                await context.Send(command);

                await this.WaitIfNeeded(context, account, account.EffectiveDate.AddDays(-1));
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
                if (state.Period.Type != LedgerType.ForUsage) throw new InvalidOperationException($"Usage billing timeout for {this.Data.SubscriptionId} received invalid billing instruction for {state.Period.Type}");

                var usage = await this.dataContext
                    .SetOf<UsageBilling>()
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

        #region IHandleMessages<CancelUsageOnlyCommand> Members

        /// <inheritdoc />
        public virtual async Task Handle(CancelUsageOnlyCommand message, IMessageHandlerContext context)
        {
            var account = await this.dataContext
                .SetOf<UsageBilling>()
                .Where(s => s.PublicKey == message.PublicKey)
                .Include(s => s.ForClient)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            // Account doesn't exist so we just clean ourselves up and leave
            if (account == null)
            {
                this.MarkAsComplete();
                return;
            }

            // Confirm the new end date hasn't been billed through otherwise bump end date to that time
            var latestPeriod = await this.dataContext
                .SetOf<LedgerEntry>()
                .Where(l => l.ForAccount.PublicKey == message.PublicKey)
                .Where(l => l.Classification == LedgerType.ForUsage)
                .OrderByDescending(l => l.PeriodEnd)
                .Select(l => (DateTime?)l.PeriodEnd)
                .FirstOrDefaultAsync();

            if (latestPeriod > message.EndDate) message.EndDate = latestPeriod.Value;

            var command = new ResetClientHistorgramCommand() { UserId = account.ForClient.UserId };
            await context.Send(command);

            // If the account hasn't even started then just nuke it from orbit-nothing is billed
            if (account.EffectiveDate > message.EndDate)
            {
                this.dataContext.SetOf<UsageBilling>().Remove(account);
                this.MarkAsComplete();
            }
            else
            {
                // Expire the account on the day indicated
                account.Expire(message.EndDate);

                await this.WaitIfNeeded(context, account, latestPeriod ?? account.EffectiveDate.AddDays(-1));
            }

            await this.dataContext.SaveChangesAsync();
        }

        #endregion

        #region IHandleMessages<ResumeUsageCommand> Members

        /// <inheritdoc />
        public async Task Handle(ResumeUsageCommand message, IMessageHandlerContext context)
        {
            var account = await this.dataContext
                .SetOf<UsageBilling>()
                .FirstAsync(s => s.PublicKey == message.PublicKey)
                .ConfigureAwait(false);
            this.Data.SubscriptionId = account.Id.Value;
            this.Data.PublicKey = account.PublicKey;

            var latestPeriod = await this.dataContext
                .SetOf<LedgerEntry>()
                .Where(l => l.ForAccount.PublicKey == message.PublicKey)
                .Where(l => l.Classification == LedgerType.ForUsage || l.Classification == LedgerType.GeneralAdjustment)
                .OrderByDescending(l => l.PeriodEnd)
                .Select(l => (DateTime?)l.PeriodEnd)
                .FirstOrDefaultAsync();

            // We're resuming a never billed account so we set it to the day before start
            latestPeriod = latestPeriod ?? account.EffectiveDate.AddDays(-1);

            await WaitIfNeeded(context, account, latestPeriod.Value);
        }

        #endregion

        #region Helpers

        private DateSpan DetermineEndOfPeriod(UsageBilling usageBilling, DateTime lastDayOfPreviousPeriod)
        {
            if (usageBilling.EndDate <= lastDayOfPreviousPeriod) return new DateSpan(lastDayOfPreviousPeriod.AddDays(1), usageBilling.EffectiveDate);

            var nextPeriod = TemporalHelper.CalculateEndOfPeriod(usageBilling.EffectiveDate, usageBilling.EndDate, lastDayOfPreviousPeriod.AddDays(1), period: usageBilling.Recurrence);

            Console.WriteLine($"Next period calculated as {nextPeriod}");
            nextPeriod.ToSafeLocal();

            // Trim to end of subscription, if needed
            if (nextPeriod.EndingOn > usageBilling.EndDate) nextPeriod.EndingOn = usageBilling.EndDate.Value;

            return nextPeriod;
        }

        private BillingPeriodClosedTimeout NextTimeOut(UsageBilling usageBilling, DateTime lastDayOfPreviousPeriod)
        {
            var closeOfPeriod = this.DetermineEndOfPeriod(usageBilling, lastDayOfPreviousPeriod);
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

        private async Task WaitIfNeeded(IMessageHandlerContext context, UsageBilling usageBilling, DateTime lastDayOfPreviousPeriod)
        {
            // How long till we bill again?
            var timeout = this.NextTimeOut(usageBilling, lastDayOfPreviousPeriod);

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