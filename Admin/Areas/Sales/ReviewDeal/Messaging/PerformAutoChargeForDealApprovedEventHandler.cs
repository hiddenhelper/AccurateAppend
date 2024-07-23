using System;
using System.Linq;
using System.Threading.Tasks;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Data;
using AccurateAppend.Sales;
using AccurateAppend.Sales.Contracts.Messages;
using AccurateAppend.Sales.DataAccess;
using EventLogger;
using NServiceBus;
using Application = AccurateAppend.Core.Definitions.Application;
using DefaultContext = AccurateAppend.Sales.DataAccess.DefaultContext;

namespace AccurateAppend.Websites.Admin.Areas.Sales.ReviewDeal.Messaging
{
    /// <summary>
    /// Responds to the <see cref="DealApprovedEvent"/> event by checking for the <see cref="BillableOrder.PerformAutoBilling"/>
    /// flag and that the client has a valid primary billing card on file. If found, an attempt to charge the entire amount
    /// is sent. It either works or fails, which isn't the concern of this handler...
    /// </summary>
    public class PerformAutoChargeForDealApprovedEventHandler : IHandleMessages<DealApprovedEvent>
    {
        #region Fields

        private readonly DefaultContext dataContext;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PerformAutoChargeForDealApprovedEventHandler"/> class.
        /// </summary>
        /// <param name="dataContext">The <see cref="DefaultContext"/> used to provide data access for the handler.</param>
        public PerformAutoChargeForDealApprovedEventHandler(DefaultContext dataContext)
        {
            if (dataContext == null) throw new ArgumentNullException(nameof(dataContext));

            this.dataContext = dataContext;
        }

        #endregion

        #region IHandleMessages<DealApprovedEvent> Members

        /// <inheritdoc />
        public virtual async Task Handle(DealApprovedEvent message, IMessageHandlerContext context)
        {
            var dealId = message.DealId;
            var userId = message.Client;

            using (SecurityHelper.Alias(WellKnownIdentifiers.SystemIdentity))
            {
                var deal = await this.dataContext
                    .SetOf<BillableOrder>()
                    .Where(o => o.PerformAutoBilling)
                    .Where(o => o.Deal.Id == dealId)
                    .Include(o => o.Transactions)
                    .Include(o => o.PendingTransactions)
                    .Select(o => o.Deal)
                    .CanBeBilled()
                    .Include(d => d.Orders)
                    .Include(d => d.Orders.Select(o => o.Lines))
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);

                // means it wasn't or isn't still valid for auto-billing anymore
                if (deal == null)
                {
                    Logger.LogEvent($"Deal {dealId} doesn't exist or isn't eligible for auto-charging", Severity.None, Application.AccurateAppend_Admin);
                    return;
                }

                var creditCard = await this.dataContext
                    .SetOf<CreditCardRef>()
                    .Where(c => c.Client.UserId == userId)
                    .Where(c => c.IsPrimary)
                    .FirstOrDefaultAsync();

                // No valid primary card, no auto-billing
                if (creditCard == null || !creditCard.IsValid())
                {
                    deal.Notes.Add($"Client doesn't have a valid primary card on record to perform auto-billing{" :" + creditCard?.DisplayValue}");
                    await this.dataContext.SaveChangesAsync();
                    return;
                }

                var order = deal.OriginatingOrder();
                var amountToCharge = order.OutstandingTotal();
                if (amountToCharge <= 0m) return; // Someone got to this before us

                var chargeRequest = order.CreateRequest(creditCard, amountToCharge);

                var command = new ChargeProcessing.Contracts.ProcessChargeCommand
                {
                    PublicKey = chargeRequest.PublicKey,
                    Amount = amountToCharge,
                    CardId = creditCard.PublicKey,
                    OrderId = order.Id.Value,
                    OrderTotal = order.Total()
                };

                await this.dataContext.SaveChangesAsync();
                await context.SendLocal(command);
            }
        }

        #endregion
    }
}