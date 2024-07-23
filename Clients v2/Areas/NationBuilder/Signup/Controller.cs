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
using AccurateAppend.Websites.Clients.Areas.NationBuilder.Signup.Models;
using AccurateAppend.Websites.Clients.Security;
using EventLogger;
using Integration.NationBuilder.Data;

namespace AccurateAppend.Websites.Clients.Areas.NationBuilder.Signup
{
    /// <summary>
    /// Handles logic for starting a new NationBuilder marketing and signup use case.
    /// </summary>
    [AllowAnonymous()]
    [RestrictUrl("clients.2020connect.net", "http://www.2020connect.net")]
    [RestrictUrl("devclients.2020connect.net", "http://www.2020connect.net")]
    [Restricted()]
    public class Controller : System.Web.Mvc.Controller
    {
        #region Fields

        private readonly ISessionContext context;
        private readonly IFormsAuthentication fa;
        private readonly AccountSignupService service;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Controller"/> class.
        /// </summary>
        public Controller(ISessionContext context, IFormsAuthentication fa, AccountSignupService service)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (fa == null) throw new ArgumentNullException(nameof(fa));
            if (service == null) throw new ArgumentNullException(nameof(service));
            Contract.EndContractBlock();

            this.context = context;
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

            var model = new NationBuilderCreateAccountModel
            {
                UrlReferrer = this.Request.UrlReferrer == null ? null : this.Request.UrlReferrer.AbsoluteUri,
                PublicKey = Guid.NewGuid()
            };
            model.Slug = slug ?? ParseSlugFromReferrer(model.UrlReferrer);

            return this.View(model);
        }

        /// <summary>
        /// Action to validate and process the signup.
        /// </summary>
        /// <remarks>
        /// If you are currently authenticated and attempt to view this form, you will be redirected to the current order screen.
        /// </remarks>
        [HttpPost()]
        public virtual async Task<ActionResult> Index(NationBuilderCreateAccountModel userModel, CancellationToken cancellation)
        {
            if (this.User.Identity.IsAuthenticated) return this.RedirectToAction("Index", "Current", new { area = "Order" });

            userModel = userModel ?? new NationBuilderCreateAccountModel();
            if (!this.ModelState.IsValid) return this.View(userModel);

            try
            {
                #region verify nation is not already registered to this email

                var registrations = await this.CheckNationAccountExists(userModel.Email, userModel.Slug);
                if (registrations)
                {
                    this.ModelState.AddModelError(nameof(NationBuilderCreateAccountModel.Email), "Your email is already signed up with this Nation. Please log in to access.");
                    return this.View(userModel);
                }

                #endregion
                
                // Create user
                var principal = await this.service.Create(userModel, cancellation);

                // Log client in
                this.fa.CreateAuthenticationToken(principal);

                // redirect to thank you page, Google conversion is tracked on that page, user is redirected to AuthHandler
                return this.RedirectToAction("ThankYou", "Signup", new { slug = userModel.Slug, area = "NationBuilder" });
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

        [Authorize()]
        public virtual ActionResult ThankYou(String slug)
        {
            if (String.IsNullOrEmpty(slug)) return this.DisplayErrorResult("We're sorry but your Nation name was not provided in your request.");

            var model = new NationBuilderCreateAccountModel();
            model.Slug = slug;

            return this.View(model);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Parses slug from referrals from NationBuilder admin control panel
        /// </summary>
        private static String ParseSlugFromReferrer(String referrer)
        {
            referrer = referrer ?? String.Empty;

            try
            {
                var builder = new UriBuilder(referrer);
                return builder.Host.EndsWith(".nationbuilder.com", StringComparison.OrdinalIgnoreCase)
                    ? builder.Host.Split('.')[0]
                    : null;
            }
            catch (FormatException)
            {
                return null;
            }
        }
        
        private Task<Boolean> CheckNationAccountExists(String email, String slug)
        {
            return this.context.SetOf<Registration>().Where(r => r.Owner.UserName == email && r.NationName == slug).AnyAsync();
        }

        #endregion
    }
}