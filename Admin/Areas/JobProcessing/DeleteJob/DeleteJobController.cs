using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Data;
using AccurateAppend.JobManagement.Contracts;
using AccurateAppend.JobProcessing;
using AccurateAppend.Websites.Admin.Controllers;
using AccurateAppend.Websites.Admin.Filters;
using EventLogger;
using NServiceBus;
using Application = AccurateAppend.Core.Definitions.Application;

namespace AccurateAppend.Websites.Admin.Areas.JobProcessing.DeleteJob
{
    /// <summary>
    /// Controller used to delete an append job from the system.
    /// </summary>
    [Authorize()]
    public class DeleteJobController : ActivityLoggingController
    {
        #region Fields

        private readonly IMessageSession bus;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityLoggingController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ISessionContext"/> to use for this controller instance.</param>
        /// <param name="bus">The <see cref="IMessageSession"/> used to publish messages.</param>
        public DeleteJobController(ISessionContext context, IMessageSession bus) : base(context)
        {
            if (bus == null) throw new ArgumentNullException(nameof(bus));
            Contract.EndContractBlock();

            this.bus = bus;
        }

        #endregion

        #region Actions

        /// <summary>
        /// Action to delete the job.
        /// </summary>
        [HandleErrorWithAjaxFilter()]
        public async Task<ActionResult> Index(Int32 jobid, CancellationToken cancellation)
        {
            await this.LogEventAsync($"Delete job: {jobid}");

            try
            {
                if (await this.Context.SetOf<Integration.NationBuilder.Data.PushRequest>().AnyAsync(r => r.Job.Id == jobid, cancellation))
                {
                    return this.Json(new { success = false, error = "NationBuilder jobs cannot be deleted from this screen" }, JsonRequestBehavior.AllowGet);
                }

                var cutoff = DateTime.Now.ToFirstOfMonth().AddHours(1);
                if (await this.Context.SetOf<Job>().AnyAsync(j => j.Id == jobid && j.Status == JobStatus.Complete && j.DateCompleted < cutoff, cancellation))
                {
                    return this.Json(new { success = false, error = "This job was part of a closed month and cannot be reset" }, JsonRequestBehavior.AllowGet);
                }

                var lookups = await this.Context
                    .SetOf<Lookup>()
                    .Where(l => l.Job.Id == jobid)
                    .ToArrayAsync(cancellation);
                if (lookups.Any(l => l.Key == LookupKey.AssociatedWithDeal))
                {
                    return this.Json(new { success = false, error = "This job has an open or billed order and cannot be deleted" }, JsonRequestBehavior.AllowGet);
                }

                var command = new DeleteJobCommand();
                command.JobId = jobid;

                await this.bus.Send(command);

                return this.Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logger.LogEvent(ex, Severity.High, Application.AccurateAppend_Admin, this.Request.UserHostAddress, $"Failure to delete job: {jobid}");

                return this.Json(new {success = false, error = "Server error"}, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
    }
}
