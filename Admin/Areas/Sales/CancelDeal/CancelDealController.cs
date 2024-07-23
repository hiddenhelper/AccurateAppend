using System;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Sales;
using AccurateAppend.Websites.Admin.Areas.Sales.DealSummary;
using AccurateAppend.Websites.Admin.Controllers;
using AccurateAppend.Websites.Admin.Navigator;
using DomainModel.Services;

namespace AccurateAppend.Websites.Admin.Areas.Sales.CancelDeal
{
    /// <summary>
    /// Controller canceling <see cref="DealBinder"/> model.
    /// </summary>
    [Authorize()]
    public class CancelDealController : ActivityLoggingController2
    {
        #region Fields

        private readonly DealProcessService service;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelDealController"/> class.
        /// </summary>
        /// <param name="service">The <see cref="DealProcessService"/> api.</param>
        public CancelDealController(DealProcessService service)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));
            Contract.EndContractBlock();

            this.service = service;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Cancels the indicated deal by identifier.
        /// </summary>
        public virtual async Task<ActionResult> Index(Int32 dealId, CancellationToken cancellation)
        {
            try
            {
                await this.service.Cancel(dealId, cancellation);

                this.OnEvent($"Deal {dealId} canceled");

                return this.NavigationFor<DealSummaryController>().ToIndex();
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