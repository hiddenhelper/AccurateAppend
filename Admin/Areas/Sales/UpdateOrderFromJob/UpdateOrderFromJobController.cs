using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Data;
using AccurateAppend.JobProcessing;
using AccurateAppend.JobProcessing.Manifest;
using AccurateAppend.JobProcessing.Reporting;
using AccurateAppend.Sales;
using AccurateAppend.Sales.Contracts.Services;
using AccurateAppend.Websites.Admin.Areas.Sales.OrderDetail;
using AccurateAppend.Websites.Admin.Controllers;
using AccurateAppend.Websites.Admin.Navigator;

namespace AccurateAppend.Websites.Admin.Areas.Sales.UpdateOrderFromJob
{
    /// <summary>
    /// Controller to bridge Job Reports to build an <see cref="Order"/> item.
    /// </summary>
    /// <remarks>
    /// We want to reduce the number of spots that cross context boundaries to an absolute minimum.
    /// Therefore this controller is used to specifically isolate these types of crossings in dedicated
    /// components. In the near future we will be encapsulating the jobs logic into a service.
    /// </remarks>
    [Authorize()]
    public class UpdateOrderFromJobController : ContextBoundController
    {
        #region Fields

        private readonly IOrderManagementService service;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateOrderFromJobController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ISessionContext"/> to use for this controller instance.</param>
        /// <param name="service">The <see cref="IOrderManagementService"/> that provides deal and order content management logic.</param>
        public UpdateOrderFromJobController(ISessionContext context, IOrderManagementService service) : base(context)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));

            this.service = service;
        }

        #endregion

        #region Action Methods

        public async Task<ActionResult> Index(Int32 orderId, CancellationToken cancellation)
        {
            var order = await this.service.FindOrder(orderId, cancellation);

            if (order == null)
            {
                this.TempData["message"] = $"An error has occured while updating the order. The order {orderId} could not be found or is not editable.";
                return this.View("~/Views/Shared/Error.aspx");
            }

            ManifestBuilder manifest;
            ProcessingReport report;

            using (this.Context.CreateScope(ScopeOptions.NoTracking))
            {
                var job = await this.Context.SetOf<Job>()
                    .Where(j => j.InputFileName == order.PublicKey && j.Status == JobStatus.Complete)
                    .Include(j => j.Processing)
                    .FirstOrDefaultAsync(cancellation);
                if (job == null)
                {
                    this.TempData["message"] = $"An error has occured while updating the order. The order {orderId} is not already associated with a completed job.";
                    return this.View("~/Views/Shared/Error.aspx");
                }

                var userId = job.Owner.Id;
                
                manifest = new ManifestBuilder(job.Manifest);
                manifest.UserId = userId;
                manifest.JobId = job.Id.Value;

                report = new ProcessingReport(job.Processing.Report);
            }

            await this.service.Update(orderId, manifest, report, cancellation);

            return this.NavigationFor<OrderDetailController>().Detail(orderId);
        }

        #endregion
    }
}