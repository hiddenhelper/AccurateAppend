using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Data;
using AccurateAppend.Sales;
using AccurateAppend.Sales.DataAccess;
using AccurateAppend.Websites.Admin.Areas.Sales.SendPaymentUpdate.Messages;
using NServiceBus;

namespace AccurateAppend.Websites.Admin.Areas.Sales.SendPaymentUpdate
{
    /// <summary>
    /// Controller for dispatching payment update link emails.
    /// </summary>
    [Authorize()]
    public class SendPaymentUpdateController : Controller
    {
        #region Fields

        private readonly DefaultContext context;
        private readonly IMessageSession bus;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SendPaymentUpdateController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="DefaultContext"/> DAL component.</param>
        /// <param name="bus">The <see cref="IMessageSession"/> used to publish messages.</param>
        public SendPaymentUpdateController(DefaultContext context, IMessageSession bus)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (bus == null) throw new ArgumentNullException(nameof(bus));
            Contract.EndContractBlock();

            this.context = context;
            this.bus = bus;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Send payment update link for most recent declined charge event.
        /// </summary>
        public virtual async Task<ActionResult> ForDeal(Int32 dealId, CancellationToken cancellation)
        {
            var declined = await this.context
                .SetOf<BillableOrder>()
                .Where(o => o.Status == OrderStatus.Open)
                .Where(o => o.Deal.Id == dealId)
                .Where(o => o.Deal.Status == DealStatus.Billing)
                .SelectMany(o => o.Transactions)
                .AreNotCaptured()
                .OrderByDescending(c => c.Id)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellation);
            if (declined != null)
            {
                Debug.Assert(declined.Id.HasValue);

                var command = new SendPaymentUpdateLinkCommand {ChargeEventId = declined.Id.Value};

                await this.bus.SendLocal(command);
            }

            return this.RedirectToAction("Index", "DealDetail", new {Area = "Sales", dealId});
        }

        /// <summary>
        /// Send payment update link for the user generically.
        /// </summary>
        public virtual async Task<ActionResult> ForUser(Guid userId, CancellationToken cancellation)
        {
            var command = new SendPaymentExpiredLinkCommand {UserId = userId};

            await this.bus.SendLocal(command);

            return this.RedirectToAction("Index", "UserDetail", new {Area = "Clients", userId});
        }

        #endregion
    }
}