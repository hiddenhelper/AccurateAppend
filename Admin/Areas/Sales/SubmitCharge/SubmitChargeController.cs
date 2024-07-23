using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Mvc;
using AccurateAppend.ChargeProcessing.Contracts;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Data;
using AccurateAppend.Sales;
using AccurateAppend.Websites.Admin.Areas.Sales.SubmitCharge.Models;
using EventLogger;
using NServiceBus;
using DefaultContext = AccurateAppend.Sales.DataAccess.DefaultContext;

namespace AccurateAppend.Websites.Admin.Areas.Sales.SubmitCharge
{
    /// <summary>
    /// Controller responsible for allowing a <see cref="BillableOrder"/> to have an amount to charge and a card
    /// selected in order to be processed via the billing service.
    ///
    /// Step 1- Display order details and client wallet to select card
    /// Step 2- Submit charge request
    /// Step 3- Wait for Transaction result to post
    /// Step 4- Display result details to end user
    /// </summary>
    [Authorize()]
    public class SubmitChargeController : Controller
    {
        #region Fields

        private readonly IMessageSession bus;
        private readonly DefaultContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SubmitChargeController"/> class.
        /// </summary>
        /// <param name="bus">The message bus used to interact with the billing service.</param>
        /// <param name="context">The <see cref="DefaultContext"/> used for data access.</param>
        public SubmitChargeController(IMessageSession bus, DefaultContext context)
        {
            if (bus == null) throw new ArgumentNullException(nameof(bus));
            if (context == null) throw new ArgumentNullException(nameof(context));
            Contract.EndContractBlock();

            this.bus = bus;
            this.context = context;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Performs the action to display details of a <see cref="BillableOrder"/> charge.
        /// </summary>
        [HttpGet()]
        public virtual async Task<ActionResult> Index(Int32 orderId, CancellationToken cancellation)
        {
            var order = await this.context
                .SetOf<BillableOrder>()
                .Where(o => o.Id == orderId)
                .AsNoTracking()
                .Include(o => o.Deal)
                .Include(o => o.Deal.Client)
                .Include(o => o.Transactions)
                .Include(o => o.PendingTransactions)
                .SingleOrDefaultAsync(cancellation);

            if (order == null) return this.DisplayErrorResult($"Order {orderId} does not exist or is not a {nameof(BillableOrder)} type.");

            var deal = order.Deal;
            if (deal.Status != DealStatus.Billing) return this.DisplayErrorResult($"The deal must be in the {DealStatus.Billing} state. It is currently {deal.Status}");

            var client = deal.Client;

            var model = new Charge();

            Debug.Assert(order.Id.HasValue);

            model.Client = client.UserId;
            model.Order = order.Id.Value;
            model.OrderTotal = order.Total().RoundFractionalPennies();

            // Partial payments
            model.MaxCharge = order.OutstandingTotal().RoundFractionalPennies();

            return this.View(model);
        }

        /// <summary>
        /// Performs the action to submit the details of a <see cref="BillableOrder"/> charge to the billing services.
        /// </summary>
        [HttpPost()]
        public async Task<ActionResult> Index(Int32 orderId, Guid userId, Guid cardId, Decimal? amount, CancellationToken cancellation)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
            {
                var order = await this.context
                    .SetOf<BillableOrder>()
                    .Where(o => o.Id == orderId)
                    .Include(o => o.Deal)
                    .Include(o => o.Lines)
                    .Include(o => o.Transactions)
                    .Include(o => o.PendingTransactions)
                    .SingleOrDefaultAsync(cancellation);
                if (order == null) return this.DisplayErrorResult($"Order {orderId} does not exist or is not a {nameof(BillableOrder)} type.");

                var card = await this.context
                    .SetOf<CreditCardRef>()
                    .Where(c => c.PublicKey == cardId)
                    .Where(c => c.Client.UserId == userId)
                    .SingleOrDefaultAsync(cancellation);
                if (card == null) return this.DisplayErrorResult($"Card {cardId} does not exist or is not owned by this client");

                // Partial payments
                var remaining = order.OutstandingTotal().RoundFractionalPennies();

                // Assume 'remaining' balance
                if (amount == 0 || amount == null) amount = Math.Max(remaining, 0);

                var request = order.CreateRequest(card, amount.Value);

                using (new Correlation(request.PublicKey))
                {
                    try
                    {
                        var message = new ProcessChargeCommand
                        {
                            Amount = amount.Value,
                            CardId = cardId,
                            PublicKey = request.PublicKey,
                            OrderId = orderId,
                            OrderTotal = order.Total()
                        };

                        await this.context.SaveChangesAsync(cancellation);
                        await this.bus.SendLocal(message);

                        transaction.Complete();

                        return this.RedirectToAction("Process", new {Area = "Sales", publicKey = request.PublicKey});
                    }
                    catch (Exception ex)
                    {
                        Logger.LogEvent(ex, Severity.High, this.User.Identity.Name);
                        return this.DisplayErrorResult($"An error was encountered: {ex.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// View to wait on the results of the charge billing request.
        /// </summary>
        /// <param name="publicKey">The public key identifier relating charges to order transactions.</param>
        [HttpGet()]
        public ActionResult Process(Guid publicKey)
        {
            return this.View(publicKey);
        }
        
        #endregion
    }
}