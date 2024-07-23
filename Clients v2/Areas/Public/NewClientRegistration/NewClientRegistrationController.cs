using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Accounting;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Data;
using AccurateAppend.Security;
using AccurateAppend.Websites.Clients.Models;
using AccurateAppend.Websites.Clients.Security;
using EventLogger;
using Application = AccurateAppend.Core.Definitions.Application;

namespace AccurateAppend.Websites.Clients.Areas.Public.NewClientRegistration
{
    /// <summary>
    /// Responsible for managing sales team guided signups.
    /// </summary>
    [AllowAnonymous()]
    public class NewClientRegistrationController : Controller
    {
        #region Fields

        private readonly IFormsAuthentication fa;
        private readonly ISessionContext context;
        private readonly AccountSignupService service;
        
        #endregion

        #region Constructor

        public NewClientRegistrationController(IFormsAuthentication fa, ISessionContext context, AccountSignupService service)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (service == null) throw new ArgumentNullException(nameof(service));
            Contract.EndContractBlock();

            this.fa = fa;
            this.context = context;
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
        public async Task<ActionResult> Create(Guid id, CancellationToken cancellation)
        {
            if (this.User.Identity.IsAuthenticated) return this.RedirectToAction("Index", "Current", new { area = "Order" });

            using (this.context.CreateScope(ScopeOptions.NoTracking))
            {
                var model = new UserModel();

                var lead = await this.context.SetOf<Lead>()
                    .Where(l => l.PublicKey == id && !l.IsDeleted.Value)
                    .Where(l => l.Status != LeadStatus.ConvertedToCustomer)
                    .Select(l => new { l.Id, ApplicationId = l.Application.Id, l.Status, l.PublicKey })
                    .FirstOrDefaultAsync(cancellation);

                if (lead == null)
                {
                    return this.DisplayErrorResult("This link information cannot be found.");
                }
                if (lead.Status == LeadStatus.ConvertedToCustomer)
                {
                    return this.DisplayErrorResult("This link has already been used to create an account. If you need assistance logging in, please contact customer support.");
                }

                model.PublicKey = lead.PublicKey;
                model.ApplicationId = lead.ApplicationId;

                return this.View(model);
            }
        }

        /// <summary>
        /// Action to validate and process the signup.
        /// </summary>
        /// <remarks>
        /// If you are currently authenticated and attempt to view this form, you will be redirected to the current order screen.
        /// </remarks>
        [HttpPost()]
        public virtual async Task<ActionResult> Create(UserModel userModel, CancellationToken cancellation)
        {
            if (this.User.Identity.IsAuthenticated) return this.RedirectToAction("Index", "Current", new { area = "Order" });
            if (!this.ModelState.IsValid) return this.View(userModel);
            
            try
            {
                var principal = await this.service.Create(userModel, cancellation);

                this.fa.CreateAuthenticationToken(principal, false);

                var userId = principal.Identity.GetIdentifier();

                this.TempData["message"] = "Your account has been created and customer support has been notified. Please use the form below to send any files to customer support.";
                return this.RedirectToAction("Upload", "File", new {area = "Public", id = userId});
            }
            catch (ValidationException ex)
            {
                this.TempData["RegError"] = ex.Message;
            }
            catch (InvalidOperationException ex)
            {
                return this.DisplayErrorResult(ex.Message);
            }
            catch (Exception ex)
            {
                Logger.LogEvent(ex, TraceEventType.Critical, Severity.High, Application.Clients.ToString(), this.Request.UserHostAddress, "New client registration failing.");
                this.TempData["RegError"] = "We're sorry but we encountered an error when creating your account. Please try again, no charges or fees have been made using your information. If you continue to see this error please contact our toll free number.";

                return this.View(userModel);
            }
            return this.View(userModel);
        }

        #endregion
    }
}