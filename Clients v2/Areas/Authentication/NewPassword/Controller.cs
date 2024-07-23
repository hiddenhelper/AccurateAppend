using System;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Security;
using AccurateAppend.Websites.Clients.Areas.Authentication.NewPassword.Models;

namespace AccurateAppend.Websites.Clients.Areas.Authentication.NewPassword
{
    /// <summary>
    /// Provides forced password change functionality for using AccurateAppend credentials.
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

        public virtual ActionResult Index(String returnUrl)
        {
            returnUrl = !String.IsNullOrEmpty(returnUrl) ? returnUrl.Trim() : String.Empty;

            var model = new ForcedPasswordChangeModel(returnUrl);

            return this.View(model);
        }

        [HttpPost()]
        [AllowAnonymous()]
        public virtual async Task<ActionResult> Index(ForcedPasswordChangeModel model, CancellationToken cancellation)
        {
            if (!this.ModelState.IsValid) return this.View(model);

            if (String.IsNullOrWhiteSpace(model.RedirectTo))
            {
                model.RedirectTo = this.Url.Action("Index", "Current", new { Area = "Order" });
            }

            if (!await this.ms.ChangePasswordAsync(this.User.Identity.Name, model.NewPassword, ApplicationExtensions.AccurateAppendId, cancellation)) return this.View(model);


            return String.IsNullOrEmpty(model.RedirectTo)
                ? (ActionResult)this.RedirectToAction(nameof(Success))
                : this.Redirect(model.RedirectTo);
        }

        public virtual ActionResult Success()
        {
            return this.View();
        }

        #endregion
    }
}