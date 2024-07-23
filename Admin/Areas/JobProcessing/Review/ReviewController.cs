using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Data;
using AccurateAppend.JobProcessing;
using AccurateAppend.Websites.Admin.Controllers;
using AccurateAppend.Websites.Admin.Filters;
using EventLogger;

namespace AccurateAppend.Websites.Admin.Areas.JobProcessing.Review
{
    /// <summary>
    /// Controller for performing review actions on a <see cref="Job"/> instance.
    /// </summary>
    [Authorize()]
    public class ReviewController : ActivityLoggingController
    {
        #region Constructor
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ReviewController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ISessionContext"/> to use for this controller instance.</param>
        public ReviewController(ISessionContext context) : base(context)
        {
        }

        #endregion

        #region Action Methods

        [HandleErrorWithAjaxFilter()]
        public virtual async Task<JsonResult> Index(Int32 jobid)
        {
            try
            {
                using (var uow = this.Context.CreateScope(ScopeOptions.AutoCommit))
                {
                    var job = await this.Context.SetOf<Job>().FirstOrDefaultAsync(j => j.Id == jobid);
                    if (job == null || (job.Status != JobStatus.NeedsReview && job.Status != JobStatus.Failed)) return this.Json(new {success = false, Message = $"Job {jobid} does not exist or is not needing review."}, JsonRequestBehavior.AllowGet);

                    job.Review();

                    await uow.CommitAsync();
                }

                await this.LogEventAsync($"Set Complete, job:{jobid}");

                return this.Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (InvalidOperationException ex)
            {
                Logger.LogEvent(ex, Severity.None, Application.AccurateAppend_Admin, this.Request.UserHostAddress, this.User.Identity.Name);

                return this.Json(new { success = false, ex.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logger.LogEvent(ex, Severity.High, Application.AccurateAppend_Admin, this.Request.UserHostAddress, this.User.Identity.Name);

                throw new Exception("Unable to set job status to complete.");
            }
        }

        #endregion
    }
}
