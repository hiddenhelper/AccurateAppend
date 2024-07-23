using System;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Data;
using AccurateAppend.JobProcessing;
using AccurateAppend.JobProcessing.Manifest;
using AccurateAppend.JobProcessing.Reporting;
using AccurateAppend.Sales;
using AccurateAppend.Sales.Contracts.Services;
using AccurateAppend.Sales.Contracts.ViewModels;
using AccurateAppend.Security;
using AccurateAppend.Websites.Admin.Areas.Sales.EditDeal;
using AccurateAppend.Websites.Admin.Controllers;
using AccurateAppend.Websites.Admin.Navigator;

namespace AccurateAppend.Websites.Admin.Areas.Sales.NewDealFromJob
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
    public class NewDealFromJobController : ContextBoundController
    {
        #region Fields

        private readonly IDealManagementService service;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="NewDealFromJobController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ISessionContext"/> to use for this controller instance.</param>
        /// <param name="service">The <see cref="IDealManagementService"/> that provides deal and order content management logic.</param>
        public NewDealFromJobController(ISessionContext context, IDealManagementService service) : base(context)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));
            Contract.EndContractBlock();

            this.service = service;
        }

        #endregion

        #region Action Methods

        public virtual async Task<ActionResult> Index(Int32 jobId, CancellationToken cancellation)
        {
            var jobs = this.Context.SetOf<Job>().Where(j => j.Id == jobId);

            var job = await jobs.FirstOrDefaultAsync(cancellation);
            if (job == null) return this.DisplayErrorResult($"An error has occured while creating the deal. The job {jobId} could not be found.");
            if (job is IntegrationJob) return this.DisplayErrorResult($"An error has occured while associating to the deal. The job {jobId} is for an integration system and cannot use this feature.");
            if (job.Owner.Application.IsAdminApplication()) return this.DisplayErrorResult("Admin accounts do not support Deal creation. Try reassigning to the operations account instead.");

            try
            {
                try
                {
                    var userId = job.Owner.Id;
                    var customerFileName = job.CustomerFileName;

                    var manifest = new ManifestBuilder(job.Manifest);
                    manifest.UserId = userId;
                    manifest.JobId = jobId;

                    var report = new ProcessingReport(job.Processing.Report);
                    var publicKey = new Guid(job.InputFileName);

                    var deal = new DealModel();
                    deal.UserId = userId;
                    deal.Description = $"Processed for {customerFileName}";
                    deal.Title = customerFileName;
                    deal.SuppressNotifications = true;
                    deal.AutoBill = false;
                    deal.OwnerId = WellKnownIdentifiers.SystemUserId;

                    var dealId = await this.service.Create(publicKey, deal, manifest, report, cancellation);
                    return this.NavigationFor<EditDealController>().Edit(dealId);
                }
                catch (DbUpdateException ex)
                {
                    var source = ex.Walk().Last() as SqlException;
                    if (source?.Errors.OfType<SqlError>().Any(e => e.Number == 2601) != true) throw;

                    return this.DisplayErrorResult($"This job is already associated with a deal. Message: '{source.Message}'");
                }
            }
            catch (Exception ex)
            {
                EventLogger.Logger.LogEvent(ex, Severity.Medium, Core.Definitions.Application.AccurateAppend_Admin, this.Request.UserHostAddress, this.User.Identity.Name);

                return this.DisplayErrorResult($"An error has occured while creating the deal. Message: '{ex.Message}'");
            }
        }

        #endregion
    }
}