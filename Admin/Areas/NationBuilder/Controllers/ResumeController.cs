using System;
using System.Linq;
using System.Web.Mvc;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Data;
using AccurateAppend.Websites.Admin.Controllers;
using Integration.NationBuilder.Data;

namespace AccurateAppend.Websites.Admin.Areas.NationBuilder.Controllers
{
    /// <summary>
    /// Controller used to resume nation builder pushes.
    /// </summary>
    [Authorize()]
    public class ResumeController : ContextBoundController
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ResumeController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ISessionContext"/> to use for this controller instance.</param>
        public ResumeController(ISessionContext context) : base(context)
        {
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// <see cref="PushRequest.Resume">Resumes</see> NationBuilder <see cref="PushRequest"/>.
        /// </summary>
        public virtual ActionResult Index(Int32 id)
        {
            try
            {
                using (this.Context.CreateScope(ScopeOptions.AutoCommit))
                {
                    var push = this.Context.SetOf<PushRequest>().FirstOrDefault(p => p.Id == id);
                    if (push == null)
                    {
                        return this.Json(new { HttpStatusCode = 500, Sucess = false,  Message = "Push does not exist." }, JsonRequestBehavior.AllowGet);
                    }

                    var allowedStatus = new[] {PushStatus.Failed, PushStatus.Review, PushStatus.Pushing};
                    if (allowedStatus.Contains(push.Status))
                    {
                        push.Resume();
                        return this.Json(new { HttpStatusCode = 200, Sucess = true, Message = "" }, JsonRequestBehavior.AllowGet);
                    }

                    return this.Json(new { HttpStatusCode = 500, Sucess = false, Message = "Push is not allowed to be resumed" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                EventLogger.Logger.LogEvent(ex, Severity.Medium, Application.AccurateAppend_Admin, "", "Error resuming push " + id);
                return this.Json(new { HttpStatusCode = 500, Sucess = false, Message = "Error: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
    }
}