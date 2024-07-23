using System;
using System.Linq;
using System.Web.Mvc;
using AccurateAppend.Sales;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Data;
using AccurateAppend.Websites.Admin.Navigator;
using EventLogger;
using System.Threading;
using System.Threading.Tasks;
using AccurateAppend.Websites.Admin.Areas.Billing.ViewCreditCards;
using NServiceBus;

namespace AccurateAppend.Websites.Admin.Areas.Billing.DeleteCreditCard
{
    /// <summary>
    /// Controller for removing a payment account from the system.
    /// </summary>
    [Authorize()]
    public class DeleteCreditCardController : Controller
    {
        #region Fields

        private readonly AccurateAppend.Sales.DataAccess.DefaultContext context;
        private readonly IMessageSession bus;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteCreditCardController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="AccurateAppend.Sales.DataAccess.DefaultContext"/> to use for this controller instance.</param>
        /// <param name="bus">The message bus used to interact with the billing service.</param>
        public DeleteCreditCardController(AccurateAppend.Sales.DataAccess.DefaultContext context, IMessageSession bus)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (bus == null) throw new ArgumentNullException(nameof(bus));

            this.context = context;
            this.bus = bus;
        }

        #endregion

        #region Action Methods

        public virtual async Task<ActionResult> Index(Int32 cardId, CancellationToken cancellation)
        {
            try
            {
                var deals = this.context
                    .SetOf<DealBinder>()
                    .Where(d => d.Status == DealStatus.Billing ||
                                (d.Status == DealStatus.Approval && d.Orders.Any(o => o.Bill.ContractType == ContractType.Receipt)));

                var cards = this.context
                    .SetOf<CreditCardRef>()
                    .Where(c => c.Id == cardId);

                var ordersInProcess = deals.Join(cards, d => d.Client.UserId, c => c.Client.UserId, (d, c) => d);
                if (await ordersInProcess.AnyAsync(cancellation))
                {
                    throw new InvalidOperationException("This card has Deals currently in Approval/Billing status. Card cannot be removed until they are completed or canceled.");
                }

                var card = await cards.Include(c => c.Client).FirstOrDefaultAsync(cancellation);
                if (card == null) return this.DisplayErrorResult($"Card: {cardId} does not exist");

                var command = new ChargeProcessing.Contracts.DeletePaymentProfileCommand();
                command.CardId = card.PublicKey;

                await this.bus.SendLocal(command);

                // Fake wait processing time because we're not using callbacks
                await Task.Delay(TimeSpan.FromSeconds(4), cancellation);

                return this.NavigationFor<ViewCreditCardsController>().Detail(card.Client.UserId);
            }
            catch (InvalidOperationException ex)
            {
                return this.DisplayErrorResult(ex.Message);
            }
            catch (Exception ex)
            {
                Logger.LogEvent(ex, Severity.Medium, Application.AccurateAppend_Admin);
                return this.DisplayErrorResult($"An error has occured while deleting the card. Message: {ex.Message }");
            }
        }

        #endregion
    }
}