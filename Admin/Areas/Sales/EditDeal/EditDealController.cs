using System;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Sales;
using AccurateAppend.Sales.Contracts.Services;
using AccurateAppend.Sales.Contracts.ViewModels;
using AccurateAppend.Websites.Admin.Areas.Sales.DealDetail;
using AccurateAppend.Websites.Admin.Areas.Sales.DealSummary;
using AccurateAppend.Websites.Admin.Controllers;
using AccurateAppend.Websites.Admin.Navigator;

namespace AccurateAppend.Websites.Admin.Areas.Sales.EditDeal
{
    /// <summary>
    /// Controller updating a <see cref="DealBinder"/>.
    /// </summary>
    [Authorize()]
    public class EditDealController : ActivityLoggingController2
    {
        #region Fields

        private readonly IDealManagementService service;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="EditDealController"/> class.
        /// </summary>
        /// <param name="service">The <see cref="IDealManagementService"/> that provides deal and order content management logic.</param>
        public EditDealController(IDealManagementService service)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));
            Contract.EndContractBlock();

            this.service = service;
        }

        #endregion

        #region Actions

        [HttpGet()]
        public async Task<ActionResult> Index(Int32 dealId, CancellationToken cancellation)
        {
            var deal = await this.service.FindDeal(dealId, cancellation);
            if (deal == null) return this.NavigationFor<DealSummaryController>().ToIndex();
            
            return this.View(deal);
        }

        [HttpPost()]
        public async Task<ActionResult> Index(DealModel model, CancellationToken cancellation)
        {
            model = model ?? new DealModel();
            if (!this.ModelState.IsValid) return this.View(model);

            await this.service.Update(model, cancellation);

            this.OnEvent($"Deal {model.Id} edited");

            return this.NavigationFor<DealDetailController>().Detail(model.Id.Value);
        }

        #endregion
    }
}