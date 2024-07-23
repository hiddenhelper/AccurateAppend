using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Data;
using AccurateAppend.JobManagement.Contracts;
using AccurateAppend.JobProcessing;
using AccurateAppend.Websites.Admin.Controllers;
using AccurateAppend.Websites.Admin.Filters;
using AccurateAppend.Websites.Admin.Navigator;
using AccurateAppend.Websites.Admin.Areas.JobProcessing.Summary;
using EventLogger;
using Integration.NationBuilder.Data;
using NServiceBus;

namespace AccurateAppend.Websites.Admin.Areas.JobProcessing.Reset
{
    /// <summary>
    /// Controller for restarting a failed job (as opposed to resuming).
    /// </summary>
    [Authorize()]
    public class ResetController : ActivityLoggingController
    {
        #region Fields

        private readonly IMessageSession bus;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ResetController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ISessionContext"/> to use for this controller instance.</param>
        /// <param name="bus">The <see cref="IMessageSession"/> used to publish messages.</param>
        public ResetController(ISessionContext context, IMessageSession bus) : base(context)
        {
            if (bus == null) throw new ArgumentNullException(nameof(bus));
            Contract.EndContractBlock();

            this.bus = bus;
        }

        #endregion

        #region Actions

        [HandleErrorWithAjaxFilter()]
        public async Task<ActionResult> Index(Int32 jobid)
        {
            await this.LogEventAsync($"Reset job: {jobid}");

            try
            {
                await this.PerformRest(jobid);

                return this.Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logger.LogEvent(ex, Severity.High, Application.AccurateAppend_Admin, this.Request.UserHostAddress, $"Failure resetting job {jobid}");

                return this.Json(new {success = false, error = ex.Message}, JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<ActionResult> Interactive(Int32 jobid)
        {
            await this.LogEventAsync($"Reset job: {jobid}");

            try
            {
                await this.PerformRest(jobid);

                return this.NavigationFor<SummaryController>().ToIndex();
            }
            catch (Exception ex)
            {
                Logger.LogEvent(ex, Severity.High, Application.AccurateAppend_Admin, this.Request.UserHostAddress, $"Failure resetting job {jobid}");

                return this.DisplayErrorResult($"Job {jobid} could not be reset. {ex.Message}");
            }
        }

        #endregion

        #region Helpers

        protected virtual async Task PerformRest(Int32 jobid)
        {
            if (await this.Context.SetOf<PushRequest>().AnyAsync(r => r.Job.Id == jobid && (r.Status == PushStatus.Pushing || r.Status == PushStatus.Complete)).ConfigureAwait(false))
            {
                throw new InvalidOperationException("NationBuilder job has already started upload and cannot be reset");
            }

            var cutoff = DateTime.Now.ToFirstOfMonth().AddHours(1);
            if (await this.Context.SetOf<Job>().AnyAsync(j => j.Id == jobid && j.Status == JobStatus.Complete && j.DateCompleted < cutoff))
            {
                throw new InvalidOperationException("This job was part of a closed month and cannot be reset");
            }

            var command = new ResetJobCommand();
            command.JobId = jobid;

            await this.bus.Send(command);
        }

        #endregion
    }
}
