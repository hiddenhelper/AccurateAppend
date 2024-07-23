using System;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core.Definitions;
using AccurateAppend.JobProcessing;
using AccurateAppend.JobProcessing.DataAccess;
using AccurateAppend.Websites.Admin.Controllers;
using AccurateAppend.Websites.Admin.Filters;
using EventLogger;

namespace AccurateAppend.Websites.Admin.Areas.JobProcessing.ChangeJobPriority
{
    /// <summary>
    /// Controller for managing <see cref="Job"/> <see cref="Priority"/>.
    /// </summary>
    [Authorize()]
    public class ChangeJobPriorityController : ActivityLoggingController2
    {
        #region Fields

        private readonly IChangePriorityCommand command;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeJobPriorityController"/> class.
        /// </summary>
        /// <param name="command">The command application logic.</param>
        public ChangeJobPriorityController(IChangePriorityCommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            Contract.EndContractBlock();

            this.command = command;
        }

        #endregion

        #region Actions

        /// <summary>
        /// Performs the action to change a job priority.
        /// </summary>
        [HandleErrorWithAjaxFilter()]
        public virtual async Task< ActionResult> Index(Int32 jobid, Priority priority, CancellationToken cancellation)
         {
             this.OnEvent($"Change priority, job:{jobid}, priority:{priority}");

            try
            {
                await this.command.Change(jobid, priority, cancellation);
                
                return this.Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logger.LogEvent(ex, Severity.High, Application.AccurateAppend_Admin, this.Request.UserHostAddress, this.User.Identity.Name);

                return this.Json(new {success = false, error = "Server error"}, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
    }
}
