using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Data;
using AccurateAppend.Security;
using AccurateAppend.Websites.Clients.Areas.LeadPhilanthropy.Signup.Models;
using AccurateAppend.Websites.Clients.Security;
using EventLogger;

namespace AccurateAppend.Websites.Clients.Areas.LeadPhilanthropy.Signup
{
    /// <summary>
    /// Handles logic for starting a new Lead Philanthropy marketing and signup use case.
    /// </summary>
    [AllowAnonymous()]
    [RestrictUrl("clients.2020connect.net", "http://www.2020connect.net")]
    [RestrictUrl("devclients.2020connect.net", "http://www.2020connect.net")]
    [Restricted()]
    public class SignupController : Controller
    {
        #region Fields

        private readonly IFormsAuthentication fa;
        private readonly AccountSignupService service;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SignupController"/> class.
        /// </summary>
        public SignupController(IFormsAuthentication fa, AccountSignupService service)
        {
            if (fa == null) throw new ArgumentNullException(nameof(fa));
            if (service == null) throw new ArgumentNullException(nameof(service));
            Contract.EndContractBlock();

            this.fa = fa;
            this.service = service;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Action to display the initial signup page.
        /// </summary>
        /// <remarks>
        /// If you are currently authenticated and attempt to view this form, you will be redirected to the current order screen.
        /// </remarks>
        public virtual ActionResult Index(String slug)
        {
            if (this.User.Identity.IsAuthenticated) return this.RedirectToAction("Index", "Current", new { area = "Order" });

            var model = new LeadPhilanthropyCreateAccountModel
            {
                UrlReferrer = this.Request.UrlReferrer == null ? null : this.Request.UrlReferrer.AbsoluteUri,
                PublicKey = Guid.NewGuid()
            };

            return this.View(model);
        }

        /// <summary>
        /// Action to validate and process the signup.
        /// </summary>
        /// <remarks>
        /// If you are currently authenticated and attempt to view this form, you will be redirected to the current order screen.
        /// </remarks>
        [HttpPost()]
        public virtual async Task<ActionResult> Index(LeadPhilanthropyCreateAccountModel userModel, CancellationToken cancellation)
        {
            if (this.User.Identity.IsAuthenticated) return this.RedirectToAction("Index", "Current", new { area = "Order" });

            userModel = userModel ?? new LeadPhilanthropyCreateAccountModel();
            if (!this.ModelState.IsValid) return this.View(userModel);

            try
            {
                // Create user
                var principal = await this.service.Create(userModel, cancellation);

                // Log client in
                this.fa.CreateAuthenticationToken(principal);

                // Redirect to order page
                return this.RedirectToAction("Start", "Csv", new { area = "Order" });
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