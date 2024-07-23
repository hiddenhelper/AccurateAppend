using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Data;
using AccurateAppend.Security;
using AccurateAppend.Websites.Admin.Controllers;

namespace AccurateAppend.Websites.Admin.Areas.SecurityManagement.LogInAsClient
{
    /// <summary>
    /// Controller for managing log in as client.
    /// </summary>
    [Authorize()]
    public class LogInAsClientController : ActivityLoggingController2
    {
        #region Fields

        private readonly AccurateAppend.Accounting.DataAccess.DefaultContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="LogInAsClientController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ISessionContext"/> to use for this controller instance.</param>
        public LogInAsClientController(AccurateAppend.Accounting.DataAccess.DefaultContext context)
        {
            this.context = context;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Action to create a single use logon and redirect to the clients application.
        /// </summary>
        public virtual async Task<ActionResult> Index(Guid userId, CancellationToken cancellation)
        {
            this.OnEvent($"Logging in as client: {userId}");

            var user = await this.context
                .SetOf<User>()
                .Include(u => u.Application)
                .FirstOrDefaultAsync(u => u.Id == userId, cancellation);
            if (user == null)
            {
                this.TempData["message"] = $"User {userId} does not exist";
                return this.View("Error");
            }

            if (user.IsLockedOut)
            {
                this.TempData["message"] = $"User {userId} is currently disabled";
                return this.View("Error");
            }

            if (!this.CheckAccess(user))
            {
                return this.DisplayErrorResult($"User {user.Id} is part of an application you do not have administrative system access for");
            }

            var logons = this.context
                .SetOf<SingleUseLogon>();
            var logon = user.CreateSingleUseLogon();
            logons.Add(logon);

            await this.context.SaveChangesAsync(cancellation);

            var location = $"https://clients.accurateappend.com/Authentication/SingleUse/Login?id={logon.Id}&email={this.Server.UrlEncode(user.UserName)}";
            return this.Redirect(location);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Checks if the current interactive user has admin access to this application.
        /// </summary>
        protected virtual Boolean CheckAccess(User user)
        {
            if (user.Application.Id == ApplicationExtensions.AccurateAppendId && !this.User.Identity.IsAccurateAppendAdmin()) return false;
            if (user.Application.Id == ApplicationExtensions.TwentyTwentyId && !this.User.Identity.Is2020ConnectAdmin()) return false;

            return true;
        }

        #endregion
    }
}