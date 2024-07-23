using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Security;
using AccurateAppend.Websites.Clients.Areas.Public.Signup.Models;
using AccurateAppend.Websites.Clients.Security;
using DomainModel;
using EventLogger;

namespace AccurateAppend.Websites.Clients.Areas.Public.Signup
{
    /// <summary>
    /// Responsible for managing public self-service signups.
    /// </summary>
    [AllowAnonymous()]
    [RestrictUrl("clients.2020connect.net", "http://www.2020connect.net")]
    [RestrictUrl("devclients.2020connect.net", "http://www.2020connect.net")]
    [Restricted()]
    public class Controller : System.Web.Mvc.Controller
    {
        #region Fields

        private readonly IFormsAuthentication fa;
        private readonly AccountSignupService service;
        private readonly CaptchaVerifyer verifier;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Controller"/> class.
        /// </summary>
        public Controller(IFormsAuthentication fa, AccountSignupService service, CaptchaVerifyer verifier)
        {
            if (fa == null) throw new ArgumentNullException(nameof(fa));
            if (service == null) throw new ArgumentNullException(nameof(service));
            if (verifier == null) throw new ArgumentNullException(nameof(verifier));
            Contract.EndContractBlock();

            this.fa = fa;
            this.service = service;
            this.verifier = verifier;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Action method to display the public signup form.
        /// </summary>
        /// <remarks>
        /// If you are currently authenticated and attempt to view this form, you will be redirected to the current order screen.
        /// </remarks>
        public ActionResult Index()
        {
            if (this.User.Identity.IsAuthenticated) return this.RedirectToAction("Index", "Current", new { area = "Order" });

            return this.View(new PublicCreateAccountModel());
        }

        /// <summary>
        /// Action method to process the public signup form submission.
        /// </summary>
        /// <remarks>
        /// If you are currently authenticated and attempt to view this form, you will be redirected to the current order screen.
        /// </remarks>
        [HttpPost()]
        public virtual async Task<ActionResult> Index(PublicCreateAccountModel userModel, CancellationToken cancellation)
        {
            if (this.User.Identity.IsAuthenticated) return this.RedirectToAction("Index", "Current", new { area = "Order" });

            userModel = userModel ?? new PublicCreateAccountModel();
            if (!this.ModelState.IsValid) return this.View(userModel);

            var passedCaptcha = await this.verifier.Verify(this.Request, cancellation);
            if (!passedCaptcha) return this.View(userModel);

            try
            {
                // Create user
                var principal = await this.service.Create(userModel, cancellation);

                // Log client in
                this.fa.CreateAuthenticationToken(principal, false);

                return this.RedirectToAction("Start", "Csv", new {area = "Order"});
            }
            catch (ValidationException ex)
            {
                // registration error

                var memberName = ex.ValidationResult.MemberNames.First();
                var errorMessage = ex.ValidationResult.ErrorMessage;

                this.ModelState.AddModelError(memberName, errorMessage);
                return this.View(userModel);
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached) Debugger.Break();
                Logger.LogEvent(ex, Severity.High, "Create User Failed");

                this.TempData["RegError"] = "We're sorry but we encountered an error creating your account.";
                return this.View(userModel);
            }
        }

        #endregion
    }
}