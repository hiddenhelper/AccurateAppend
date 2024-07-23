using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Data;
using AccurateAppend.Websites.Admin.Controllers;
using AccurateAppend.Websites.Admin.Filters;
using EventLogger;

namespace AccurateAppend.Websites.Admin.Areas.JobProcessing.Resume
{
    /// <summary>
    /// Controller for resuming a failed job (as opposed to restarting).
    /// </summary>
    [Authorize()]
    public class ResumeController : ActivityLoggingController2
    {
        #region Fields

        private readonly AccurateAppend.JobProcessing.DataAccess.DefaultContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ResumeController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ISessionContext"/> to use for this controller instance.</param>
        public ResumeController(AccurateAppend.JobProcessing.DataAccess.DefaultContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            this.context = context;
        }

        #endregion

        #region Actions

        [HandleErrorWithAjaxFilter()]
        public async Task<ActionResult> Index(Int32 jobid)
        {
            this.OnEvent($"Resume job: {jobid}");

            try
            {
                await this.context.Database.ExecuteSqlCommandAsync("exec jobs.ResumeJob @JobId=@p0", jobid);

                return this.Json(new {success = true}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logger.LogEvent(ex, Severity.High, Application.AccurateAppend_Admin, this.Request.UserHostAddress, $"Failure resuming job {jobid}");

                return this.Json(new {success = false, error = "Server error"}, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
    }
}
