using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using AccurateAppend.Core;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Core.Utilities;
using AccurateAppend.Data;
using AccurateAppend.Sales;
using AccurateAppend.Sales.Contracts.Services;
using AccurateAppend.Sales.DataAccess;
using AccurateAppend.Sales.Formatters;
using AccurateAppend.Sales.Handlers;
using EventLogger;
using NServiceBus;
using Application = AccurateAppend.Core.Definitions.Application;
using DefaultContext = AccurateAppend.Sales.DataAccess.DefaultContext;

namespace AccurateAppend.Websites.Admin.Messages.Accounts
{
    /// <summary>
    /// Handler for the <see cref="CreateUsageBillCommand"/> bus message.
    /// </summary>
    /// <remarks>
    /// Responds to a message by creating any required <see cref="LedgerEntry"/>
    /// entity based on the supplied billing period and commiting it with an externally
    /// supplied <see cref="DefaultContext"/> data component.
    /// </remarks>
    public class CreateUsageBillCommandHandler : BillCreationHandler<CreateUsageBillCommand, RecurringBillingAccount>
    {
        #region Fields

        private readonly IBillFormatterFactory formatterFactory;
        private readonly DefaultContext dataContext;
        private readonly IFileLocation temp;
        private readonly IUsageReportBuilder reportBuilder;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateUsageBillCommandHandler"/> class.
        /// </summary>
        /// <param name="dataContext">The required <see cref="DefaultContext"/> component.</param>
        /// <param name="formatterFactory">The <see cref="IBillFormatterFactory"/> used to create bill content formatters with.</param>
        /// <param name="reportBuilder">The <see cref="IUsageReportBuilder"/> component.</param>
        /// <param name="temp">The <see cref="IFileLocation"/> </param>
        public CreateUsageBillCommandHandler(DefaultContext dataContext, IBillFormatterFactory formatterFactory, IUsageReportBuilder reportBuilder, IFileLocation temp)
        {
            if (dataContext == null) throw new ArgumentNullException(nameof(dataContext));
            if (formatterFactory == null) throw new ArgumentNullException(nameof(formatterFactory));
            if (reportBuilder == null) throw new ArgumentNullException(nameof(reportBuilder));
            if (temp == null) throw new ArgumentNullException(nameof(temp));
            Contract.EndContractBlock();

            this.dataContext = dataContext;
            this.formatterFactory = formatterFactory;
            this.reportBuilder = reportBuilder;
            this.temp = temp;
        }

        #endregion

        #region Overrides

        /// <inheritdoc />
        protected override Task<RecurringBillingAccount> AcquireAccount(Int32 id)
        {
            return this.dataContext
                .SetOf<RecurringBillingAccount>()
                .Include(s => s.ForClient)
                .FirstAsync(s => s.Id == id);
        }

        /// <inheritdoc />
        protected override async Task<LedgerEntry> CreateDeal(RecurringBillingAccount account, BillingPeriod billingPeriod)
        {
            Logger.LogEvent($"Attempting to create automated billing for Account={account.Id}, Type={account.GetType()}", Severity.None, Application.AccurateAppend_Admin);
            
            var creator = account.ForClient.OwnerId;
            var calculator = new DefaultClientUsageCalculator(this.dataContext);

            var client = account.ForClient;
            var rates = new CustomerCostService(client, this.dataContext);

            var entry = await account.CreateUsageBill(creator, billingPeriod, calculator, rates).ConfigureAwait(false);
            if (entry == null)
            {
                // NO DEAL, MARK AS EMPTY

                // declare @start date = '8-1-2017'--first day of next month after close
                // declare @closeDate date = '7-31-17'--last day the month to close
                // insert into accounting.AccountLedger
                // Select SubscriptionId,@closeDate,null,-1 from accounting.Subscriptions where StartDate <= @start and(EndDate is null or enddate >= @start) and Type<> 2-- accrual
                // and SubscriptionId not in (
                //  select AccountId from accounting.AccountLedger where EntryType <> 0 and EffectiveDate = @closeDate
                // )

                Logger.LogEvent($"Account={account.Id} has no usage - closing period", Severity.None, Application.AccurateAppend_Admin);

                entry = account is SubscriptionBilling
                    ? LedgerEntry.ClosePeriod((SubscriptionBilling) account, billingPeriod)
                    : LedgerEntry.ClosePeriod((UsageBilling) account, billingPeriod);

                this.dataContext.SetOf<LedgerEntry>().Add(entry);
                await this.dataContext.SaveChangesAsync().ConfigureAwait(false); // No deal means we're responsible for committing
            }
            else
            {
                this.dataContext.SetOf<LedgerEntry>().Add(entry);
                entry.WithDeal.Description = "Usage and overage";
                entry.WithDeal.Title = $"{(account is SubscriptionBilling ? "Overage" : "Usage")}: {billingPeriod.StartingOn.ToShortDateString()} - {billingPeriod.EndingOn.ToShortDateString()}";
            }

            return entry;
        }

        /// <inheritdoc />
        protected override async Task OnDealCreated(IMessageHandlerContext context, DealBinder deal)
        {
            await this.dataContext.SaveChangesAsync().ConfigureAwait(false); // ensures we have PKs
            await base.OnDealCreated(context, deal);
        }

        /// <inheritdoc />
        protected override async Task OnDealApproved(IMessageHandlerContext context, DealBinder deal)
        {
            await this.dataContext.SaveChangesAsync().ConfigureAwait(false); // ensures we have PKs
            await base.OnDealApproved(context, deal);
        }

        /// <inheritdoc />
        protected override Task<IBillFormatter> CreateFormatter(Guid userId)
        {
            return this.formatterFactory.ForUsage(userId);
        }

        /// <inheritdoc />
        protected override IEnumerable<Task<FileAttachment>> CreateAttachments(BillableOrder order, DateSpan period)
        {
            yield return this.GenerateUsageReport(order, period);
        }

        #endregion

        #region Usage Report Logic

        /// <summary>
        /// Generates a textual usage report for the billing period.
        /// </summary>
        /// <remarks>
        /// This file is generated and stored in the <see cref="temp"/> location using a new generated uuid on every call.
        /// This file does not have a extension.
        /// </remarks>
        /// <param name="order">The order we're attaching the usage report to.</param>
        /// <param name="period">The period to generate the usage report for.</param>
        /// <returns>A <see cref="FileAttachment"/> containing the file content (location, system name, public name, and mimetype.</returns>
        protected virtual async Task<FileAttachment> GenerateUsageReport(BillableOrder order, DateSpan period)
        {
            var generatedReport = await this.reportBuilder.GenerateUsageReport(order.Deal.Client.UserId, period).ConfigureAwait(false);
            var file = this.temp.CreateInstance(Guid.NewGuid().ToString());
            using (var stream = file.OpenStream(FileAccess.Write, true))
            {
                var reportBytes = Encoding.ASCII.GetBytes(generatedReport);

                await stream.WriteAsync(reportBytes, 0, reportBytes.Length).ConfigureAwait(false);
            }
            var filename = $"Usage: {order.Deal.Client.UserName} - {period.StartingOn.ToShortDateString()} thru {period.EndingOn.ToShortDateString()}.csv";

            return new FileAttachment(file.Path, "text/csv", filename);
        }

        #endregion       
    }
}