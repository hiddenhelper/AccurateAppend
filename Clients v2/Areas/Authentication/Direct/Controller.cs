using System;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Security;
using AccurateAppend.Websites.Clients.Areas.Authentication.Direct.Models;
using DomainModel;

namespace AccurateAppend.Websites.Clients.Areas.Authentication.Direct
{
    /// <summary>
    /// Provides authentication functionality using AccurateAppend credentials.
    /// </summary>
    [AllowAnonymous()]
    [RestrictUrl("clients.2020connect.net", "http://www.2020connect.net")]
    [RestrictUrl("devclients.2020connect.net", "http://www.2020connect.net")]
    [Restricted()]
    public class Controller : System.Web.Mvc.Controller
    {
        #region Fields

        private readonly IMembershipService ms;
        private readonly IFormsAuthentication fa;
        private readonly CaptchaVerifyer verifier;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Controller"/> class.
        /// </summary>
        /// <param name="ms">The <see cref="IMembershipService"/> component that provides membership functionality.</param>
        /// <param name="fa">The <see cref="IFormsAuthentication"/> component that provides logon functionality.</param>
        /// <param name="verifier">The <see cref="CaptchaVerifyer"/> component that verifies that a human, not a bot, is attempting logon.</param>
        public Controller(IMembershipService ms, IFormsAuthentication fa, CaptchaVerifyer verifier)
        {
            if (ms == null) throw new ArgumentNullException(nameof(ms));
            if (fa == null) throw new ArgumentNullException(nameof(fa));
            if (verifier == null) throw new ArgumentNullException(nameof(verifier));
            Contract.EndContractBlock();

            this.ms = ms;
            this.fa = fa;
            this.verifier = verifier;
        }

        #endregion

        #region Action Methods

        public virtual ActionResult Login(String returnUrl)
        {
            // If already logged in drop them back in the app
            if (this.User.Identity.IsAuthenticated)
            {
                returnUrl = this.Url.Action("Index", "Current", new { Area = "Order" });

                return this.Redirect(returnUrl);
            }

            var model = new DirectLoginModel(returnUrl);
            model.AllowExternalLogin = true;
            model.RememberMe = true;

            return this.View(model);
        }

        [HttpPost()]
        public virtual async Task<ActionResult> Login(DirectLoginModel model, CancellationToken cancellation)
        {
            if (!this.ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "The user name or password provided is incorrect.";
                return this.View(model);
            }

            var passedCaptcha = await this.verifier.Verify(this.Request, cancellation);
            if (!passedCaptcha)
            {
                TempData["ErrorMessage"] = "Please verify the reCAPTCHA.";
                return this.View(model);
            }

            var result = await this.ms.ValidateUserAsync(model.UserName, model.Password, cancellation);

            if (result == LoginRequestResult.Success)
            {
                var principal = await this.ms.GetPrincipalAsync(model.UserName, System.Security.Claims.AuthenticationTypes.Password, cancellation);

                this.fa.CreateAuthenticationToken(principal, model.RememberMe);

                if (String.IsNullOrWhiteSpace(model.RedirectTo))
                {
                    model.RedirectTo = this.Url.Action("Index", "Current", new { Area = "Order" });
                }

                return this.Redirect(model.RedirectTo);
            }

            if (result == LoginRequestResult.MustChangePassword)
            {
                var principal = await this.ms.GetPrincipalAsync(model.UserName, System.Security.Claims.AuthenticationTypes.Password, cancellation);

                this.fa.CreateAuthenticationToken(principal, model.RememberMe);
                return this.RedirectToAction("Index", "NewPassword", new { area = "Authentication" });
            }

            if (result == LoginRequestResult.ClosedAccount)
            {
                // Account closed by admin
                TempData["ErrorMessage"] = "Your account has been closed. Please contact support for assistance reactivating your account.";
                return this.View(model);
            }

            // If we got this far, something failed, redisplay form
            TempData["ErrorMessage"] = result == LoginRequestResult.InvalidUserNameOrPassword
                ? "The user name or password provided is incorrect."
                : "Your account is temporarily locked out for exceeding failed logon attempts. You can always reset your password or contact support if you feel this is in error.";

            return this.View(model);
        }

        #endregion
    }
}