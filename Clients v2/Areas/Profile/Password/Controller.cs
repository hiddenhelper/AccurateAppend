using System;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Security;
using AccurateAppend.Websites.Clients.Areas.Profile.Password.Models;
using EventLogger;

namespace AccurateAppend.Websites.Clients.Areas.Profile.Password
{
    /// <summary>
    /// Controller to update the interactive user login information.
    /// </summary>
    [Authorize()]
    public class Controller : System.Web.Mvc.Controller
    {
        #region Fields

        private readonly IMembershipService ms;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Controller"/> class.
        /// </summary>
        /// <param name="ms">The <see cref="IMembershipService"/> component that provides membership functionality.</param>
        public Controller(IMembershipService ms)
        {
            if (ms == null) throw new ArgumentNullException(nameof(ms));
            Contract.EndContractBlock();

            this.ms = ms;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Action to display the password update form.
        /// </summary>
        [HttpGet()]
        public virtual ActionResult Index()
        {
            return this.View();
        }

        /// <summary>
        /// Action to process the update password request.
        /// </summary>
        [HttpPost()]
        public virtual async Task<ActionResult> Index(PasswordDetailsModel model, CancellationToken cancellation)
        {
            model = model ?? new PasswordDetailsModel();

            if (!this.ModelState.IsValid) return this.View(model);

            if (!this.User.Identity.IsImpersonating())
            {
                if (await this.ms.ValidateUserAsync(this.User.Identity.Name, model.OldPassword, cancellation) != LoginRequestResult.Success)
                {
                    this.ModelState.AddModelError(nameof(model.OldPassword), "Old password does not match our records");
                }
            }

            try
            {
                if (await this.ms.ChangePasswordAsync(this.User.Identity.Name, model.NewPassword, this.User.Identity.GetApplication(), cancellation))
                {
                    this.TempData["message"] = "Your password was successfully updated.";
                    this.TempData["messageType"] = "success";

                    return this.View(new PasswordDetailsModel());
                }

                this.TempData["message"] = "Your password could not be updated. Ensure the password entered in the Current Password field matches your current password.";
                this.TempData["messageType"] = "error";
            }
            catch (Exception ex)
            {
                this.TempData["message"] = "An error occured and your password was not be updated. Please contact customer support to update your password.";
                this.TempData["messageType"] = "error";
                Logger.LogEvent(ex, Severity.High, $"{nameof(Controller)} failing.");
            }

            return this.View(model);
        }

        #endregion
    }
}