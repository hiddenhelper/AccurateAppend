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
using EventLogger;
using NServiceBus;
using DefaultContext = AccurateAppend.JobProcessing.DataAccess.DefaultContext;

namespace AccurateAppend.Websites.Admin.Areas.JobProcessing.Reassign
{
    /// <summary>
    /// Controller for performing Job Reassignment.
    /// </summary>
    [Authorize()]
    public class ReassignController : ActivityLoggingController2
    {
        #region Fields

        private readonly DefaultContext context;
        private readonly IMessageSession bus;

        #endregion
        
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ReassignController"/> class.
        /// </summary>
        /// <param name="bus">The <see cref="IMessageSession"/> component.</param>
        public ReassignController(DefaultContext context, IMessageSession bus)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (bus == null) throw new ArgumentNullException(nameof(bus));
            Contract.EndContractBlock();

            this.context = context;
            this.bus = bus;
        }

        #endregion

        #region Action Methods

        [OutputCache(Duration = 0, VaryByParam = "None")]
        public async Task<ActionResult> Index(Int32 jobId, Guid userid, CancellationToken cancellation)
        {
            try
            {
                var job = await this.context
                    .SetOf<Job>()
                    .Where(j => j.Id == jobId)
                    .Include(j => j.Lookups)
                    .FirstOrDefaultAsync(cancellation)
                    .ConfigureAwait(false);
                if (job == null) return this.Json(new {success = true, message = "Job does not exist"});

                if (job is IntegrationJob)
                {
                    return this.Json(new {success = false, message = "NationBuilder jobs cannot be reassigned"});
                }
                if (job is ClientJob)
                {
                    return this.Json(new {success = false, message = "Client jobs cannot be reassigned"});
                }
                if (job is ListbuilderJob)
                {
                    return this.Json(new {success = false, message = "List builder jobs cannot be reassigned"});
                }

                if (job.Status == JobStatus.Complete && job.DateCompleted < DateTime.Now.ToFirstOfMonth().AddHours(1))
                {
                    return this.Json(new {success = false, message = "This job was part of a closed month and cannot be reassigned"});
                }

                if (job.AccessLookups().AssociatedWithDeal != null)
                {
                    return this.Json(new { success = false, message = $"This job is already associated with deal {job.AccessLookups().AssociatedWithDeal} and cannot be reassigned"});
                }

                var command = new ReassignJobCommand();
                command.JobId = jobId;
                command.NewUserId = userid;

                await this.bus.Send(command);

                this.OnEvent($"Reassigned job: {jobId} to {userid}");

                return this.Json(new {success = true, message = "Job reassigned"});
            }
            catch (Exception ex)
            {
                Logger.LogEvent(ex, Severity.High,Application.AccurateAppend_Admin, this.Request.UserHostAddress, $"Failure reassigning job {jobId}");
                return this.Json(new { success = false, message = $"Server exception: {ex.Message}" });
            }
        }

        #endregion
    }
}