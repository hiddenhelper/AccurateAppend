using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using AccurateAppend.Core;
using AccurateAppend.Sales;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Data;
using AccurateAppend.Sales.Contracts.Services;
using AccurateAppend.Sales.DataAccess;
using AccurateAppend.Sales.Handlers;
using EventLogger;
using NServiceBus;
using Application = AccurateAppend.Core.Definitions.Application;
using DefaultContext = AccurateAppend.Sales.DataAccess.DefaultContext;

namespace AccurateAppend.Websites.Admin.Messages.Accounts
{
    /// <summary>
    /// Handler for the <see cref="CreateSubscriptionBillCommand"/> bus message.
    /// </summary>
    /// <remarks>
    /// Responds to a message by creating any required <see cref="LedgerEntry"/>
    /// entity based on the supplied billing period and commiting it with an externally
    /// supplied <see cref="DefaultContext"/> data component.
    /// </remarks>
    public class CreateSubscriptionBillCommandHandler : BillCreationHandler<CreateSubscriptionBillCommand, SubscriptionBilling>
    {
        #region Fields

        private readonly DefaultContext dataContext;
        private readonly IBillFormatterFactory formatterFactory;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateSubscriptionBillCommandHandler"/> class.
        /// </summary>
        /// <param name="dataContext">The required <see cref="DefaultContext"/> component providing data access to the sales data.</param>
        /// <param name="formatterFactory">The <see cref="IBillFormatterFactory"/> used to create bill content formatters with.</param>
        public CreateSubscriptionBillCommandHandler(DefaultContext dataContext, IBillFormatterFactory formatterFactory)
        {
            if (dataContext == null) throw new ArgumentNullException(nameof(dataContext));
            if (formatterFactory == null) throw new ArgumentNullException(nameof(formatterFactory));
            Contract.EndContractBlock();

            this.dataContext = dataContext;
            this.formatterFactory = formatterFactory;
        }

        #endregion

        #region Overrides

        /// <inheritdoc />
        protected override Task<SubscriptionBilling> AcquireAccount(Int32 id)
        {
            return this.dataContext
                .SetOf<SubscriptionBilling>()
                .Include(s => s.ForClient)
                .FirstAsync(s => s.Id == id);
        }

        /// <inheritdoc />
        protected override async Task<LedgerEntry> CreateDeal(SubscriptionBilling account, BillingPeriod billingPeriod)
        {
            Logger.LogEvent($"Attempting to create automated billing for Subscription={account.Id}", Severity.None, Application.AccurateAppend_Admin);

            var creator = account.ForClient.OwnerId;

            var product = await this.dataContext
                .SetOf<Product>()
                .ForSubscription()
                .SingleAsync()
                .ConfigureAwait(false);

            if (!account.IsValidForPeriod(billingPeriod)) return null;

            var entry = account.CreateSubscriptionBill(creator, product, billingPeriod);
            var deal = entry.WithDeal;
            deal.Title = $"Subscription {billingPeriod.StartingOn.ToShortDateString()}-{billingPeriod.EndingOn.ToShortDateString()}";

            this.dataContext.SetOf<LedgerEntry>().Add(entry);

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
            return this.formatterFactory.ForSubscription(userId);
        }

        /// <inheritdoc />
        protected override IEnumerable<Task<FileAttachment>> CreateAttachments(BillableOrder order, DateSpan period)
        {
            yield break; // Never attachments
        }

        #endregion
    }
}