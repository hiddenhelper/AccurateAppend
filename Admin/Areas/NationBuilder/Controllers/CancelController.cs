using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Data;
using AccurateAppend.Websites.Admin.Controllers;
using Integration.NationBuilder.Data;

namespace AccurateAppend.Websites.Admin.Areas.NationBuilder.Controllers
{
    /// <summary>
    /// Controller used to cancel nation builder pushes.
    /// </summary>
    [Authorize()]
    public class CancelController : ContextBoundController
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ISessionContext"/> to use for this controller instance.</param>
        public CancelController(ISessionContext context)
            : base(context)
        {
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// <see cref="PushRequest.Cancel">Cancels</see> NationBuilder <see cref="PushRequest"/>.
        /// </summary>
        public virtual async Task<ActionResult> Index(Int32 id)
        {
            try
            {
                using (var uow = this.Context.CreateScope(ScopeOptions.AutoCommit))
                {
                    var push = await this.Context.SetOf<PushRequest>().Include(p => p.Billing).FirstOrDefaultAsync(p => p.Id == id);
                    if (push != null && push.Status != PushStatus.Complete)
                    {
                        // Already billed so cannot be canceled
                        if (push.Billing != null && push.Billing.Status == BillingStatus.Complete)
                        {
                            uow.Rollback();
                            return this.Json(new { HttpStatusCode = (Int32)HttpStatusCode.InternalServerError, Sucess = false, Message = "This push has already been billed and cannot be canceled" }, JsonRequestBehavior.AllowGet);
                        }

                        var user = await this.Context.CurrentUserAsync();

                        push.Cancel(user, "Push canceled by administrator");
                        return this.Json(new { HttpStatusCode = (Int32)HttpStatusCode.OK, Sucess = true, Message = "Push canceled" }, JsonRequestBehavior.AllowGet);
                    }

                    await uow.CommitAsync();
                    return this.Json(new { HttpStatusCode = (Int32)HttpStatusCode.NotFound, Sucess = false, Message = "Push was unable to be canceled" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                EventLogger.Logger.LogEvent(ex, Severity.Medium, $"Error cancel push {id}");
                return this.Json(new { HttpStatusCode = (Int32)HttpStatusCode.InternalServerError, Sucess = false, Message = $"Error: {ex.Message}" }, JsonRequestBehavior.AllowGet);    
            }
        }

        #endregion
    }
}