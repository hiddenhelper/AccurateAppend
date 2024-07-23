using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Data;
using AccurateAppend.JobProcessing;
using AccurateAppend.Websites.Admin.Controllers;

namespace AccurateAppend.Websites.Admin.Areas.SmtpRules.Delete
{
    [Authorize()]
    public class DeleteController : ContextBoundController
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ISessionContext"/> to use for this controller instance.</param>
        public DeleteController(ISessionContext context)
            : base(context)
        {
        }

        #endregion

        #region Action Methods

        [OutputCache(Duration = 0, VaryByParam = "None")]
        public virtual async Task<ActionResult> Index(Int32 id)
        {
            try
            {
                using (var uow = this.Context.CreateScope(ScopeOptions.AutoCommit))
                {
                    var rule = await this.Context.SetOf<SmtpAutoprocessorRule>()
                        .Where(r => r.Id == id)
                        .Include(r => r.ManifestToRun)
                        .FirstOrDefaultAsync();
                    if (rule == null) return this.Json(new {success = true}, JsonRequestBehavior.AllowGet);

                    this.Context.SetOf<SmtpAutoprocessorRule>().Remove(rule);
                    this.Context.SetOf<ManifestCache>().Remove(rule.ManifestToRun);

                    await uow.CommitAsync();
                }

                return this.Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                EventLogger.Logger.LogEvent(ex, Severity.Medium, Application.AccurateAppend_Admin, this.Request.UserHostAddress, "Failure deleting SmtpRule:" + id);

                return this.Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
    }
}