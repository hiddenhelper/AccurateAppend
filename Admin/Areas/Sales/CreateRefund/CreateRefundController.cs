using System;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Websites.Admin.Areas.Sales.RefundDeal;
using DomainModel.Services;

namespace AccurateAppend.Websites.Admin.Areas.Sales.CreateRefund
{
    /// <summary>
    /// Controller for creating refunds on completed orders.
    /// </summary>
    [Authorize()]
    public class CreateRefundController : Controller
    {
        #region Fields

        private readonly DealRefundService service;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RefundDealController"/> class.
        /// </summary>
        /// <param name="service">The <see cref="DealRefundService"/> used to generate refunds with.</param>
        public CreateRefundController(DealRefundService service)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));
            Contract.EndContractBlock();

            this.service = service;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Creates or acquires the current open refund for the specified deal.
        /// </summary>
        public virtual async Task<ActionResult> Index(Int32 dealId, CancellationToken cancellation)
        {
            try
            {
                var orderId = await this.service.CreateRefund(dealId, cancellation);

                return this.RedirectToAction("Index", "RefundDeal", new {Area = "Sales", orderId});
            }
            catch (Exception ex)
            {
                this.TempData["message"] = ex.Message;
                return this.View("~/Views/Shared/Error.aspx");
            }
        }

        #endregion
    }
}