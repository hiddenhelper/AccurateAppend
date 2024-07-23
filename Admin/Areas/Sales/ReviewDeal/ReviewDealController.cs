using System;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AccurateAppend.Sales;
using AccurateAppend.Websites.Admin.Areas.Sales.DealDetail;
using AccurateAppend.Websites.Admin.Controllers;
using AccurateAppend.Websites.Admin.Navigator;
using DomainModel.ActionResults;
using DomainModel.Enum;
using DomainModel.Services;

namespace AccurateAppend.Websites.Admin.Areas.Sales.ReviewDeal
{
    /// <summary>
    /// Controller performing review operation of <see cref="DealBinder"/> entities.
    /// </summary>
    [Authorize()]
    public class ReviewDealController : ActivityLoggingController2
    {
        #region Fields

        private readonly DealProcessService service;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <seealso cref="ReviewDealController"/> class.
        /// </summary>
        /// <param name="service">The <see cref="DealProcessService"/> api.</param>
        public ReviewDealController(DealProcessService service)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));

            this.service = service;
        }

        #endregion

        #region Actions

        /// <summary>
        /// Performs the action to display the bill details for a Deal.
        /// </summary>
        [HttpGet()]
        public virtual async Task<ActionResult> Index(Int32 dealId, CancellationToken cancellation)
        {
            try
            {
                var deal = await this.service.Review(dealId, cancellation);
                return this.View(deal);
            }
            catch (Exception ex)
            {
                this.TempData["message"] = ex.Message;
                return this.View("~/Views/Shared/Error.aspx");
            }
        }

        /// <summary>
        /// Performs the action to process the review operation.
        /// </summary>
        [HttpPost()]
        public async Task<ActionResult> Index(Int32 dealId, ReviewAction action, String approvalnote, CancellationToken cancellation)
        {
            if (String.IsNullOrWhiteSpace(approvalnote)) approvalnote = $"Action: {action}";
            
            try
            {
                if (action == ReviewAction.Decline)
                {
                    await this.service.Decline(dealId, approvalnote, cancellation);
                }
                else
                {
                    await this.service.Approve(dealId, approvalnote, cancellation);
                }

                this.OnEvent($"Reviewed deal: {dealId}");

                return this.NavigationFor<DealDetailController>().Detail(dealId);
            }
            catch (Exception ex)
            {
                this.TempData["message"] = ex.Message;
                return this.View("~/Views/Shared/Error.aspx");
            }
        }

        [HttpGet()]
        public virtual async Task<ActionResult> Content(Int32 dealId, CancellationToken cancellation)
        {
            try
            {
                var bill = await this.service.Review(dealId, cancellation);
                if (bill == null) return new EmptyResult();

                if (bill.IsHtml) return this.Content(bill.Body, MediaTypeNames.Text.Html);

                return new LiteralResult(true) { Data = HttpUtility.HtmlEncode(bill.Body) };
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