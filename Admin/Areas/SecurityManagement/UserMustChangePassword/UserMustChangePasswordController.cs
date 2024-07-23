using System;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core.Definitions;
using DomainModel.Commands;
using EventLogger;

namespace AccurateAppend.Websites.Admin.Areas.SecurityManagement.UserMustChangePassword
{
    [Authorize()]
    public class UserMustChangePasswordController : Controller
    {
        #region Fields

        private readonly ILogonMustResetPasswordCommand command;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="UserMustChangePasswordController"/> class.
        /// </summary>
        /// <param name="command">The <see cref="ILogonMustResetPasswordCommand"/> to use for this controller instance.</param>
        public UserMustChangePasswordController(ILogonMustResetPasswordCommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            Contract.EndContractBlock();

            this.command = command;
        }

        #endregion

        #region Action Methods

        public virtual async Task<ActionResult> Index(Guid userId, CancellationToken cancellation)
        {
            try
            {
                await this.command.Run(userId);

                return this.Json(new { Sucess = true, Message = "Security Updated" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logger.LogEvent(ex, Severity.Medium, Application.AccurateAppend_Admin);
                return this.Json(new { Sucess = false, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
    }
}