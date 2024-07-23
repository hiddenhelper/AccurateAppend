using System;
using System.Linq;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core.Collections.Generic;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Data;
using AccurateAppend.Security;
using AccurateAppend.Websites.Admin.Areas.Clients.LeadSummary;
using AccurateAppend.Websites.Admin.Controllers;
using AccurateAppend.Websites.Admin.Areas.JobProcessing.Summary;
using AccurateAppend.Websites.Admin.Messages.Admin;
using AccurateAppend.Websites.Admin.Navigator;
using AccurateAppend.Websites.Admin.ViewModels.Authentication;
using DomainModel;
using NServiceBus;

namespace AccurateAppend.Websites.Admin.Areas.Operations.Authentication
{
    /// <summary>
    /// Controller performing log on and log out operations.
    /// </summary>
    public class AuthenticationController : ActivityLoggingController2
    {
        #region Fields

        private readonly IFormsAuthentication formsAuthentication;
        private readonly IMembershipService membershipService;
        private readonly IMessageSession bus;
        private readonly ISessionContext context;
        private readonly CaptchaVerifyer verifier;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationController"/> class.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="formsAuthentication">The <see cref="IFormsAuthentication"/> component.</param>
        /// <param name="membershipService">The <see cref="IMembershipService"/> component that provides membership functionality.</param>
        /// <param name="bus">Provides access to the messaging bus.</param>
        /// <param name="verifier">The <see cref="CaptchaVerifyer"/> component that verifies that a human, not a bot, is attempting logon.</param>
        public AuthenticationController(ISessionContext context, IFormsAuthentication formsAuthentication, IMembershipService membershipService, IMessageSession bus, CaptchaVerifyer verifier)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (formsAuthentication == null) throw new ArgumentNullException(nameof(formsAuthentication));
            if (membershipService == null) throw new ArgumentNullException(nameof(membershipService));
            if (bus == null) throw new ArgumentNullException(nameof(bus));
            if (verifier == null) throw new ArgumentNullException(nameof(verifier));
            Contract.EndContractBlock();

            this.context = context;
            this.formsAuthentication = formsAuthentication;
            this.membershipService = membershipService;
            this.bus = bus;
            this.verifier = verifier;
        }

        #endregion

        #region Actions

        /// <summary>
        /// Performs the action to log the current user out of the system.
        /// </summary>
        public virtual ActionResult LogOff()
        {
            this.formsAuthentication.SignOut();

            return this.RedirectToAction("LogOn");
        }

        /// <summary>
        /// Performs the action to present a view for a user to provide authentication credentials.
        /// </summary>
        public virtual ActionResult LogOn()
        {
            var returnUrl = this.Request["ReturnUrl"] ?? "/";
            return this.View(new LogonModel { ReturnUrl = returnUrl });
        }

        /// <summary>
        /// Performs the action to log a user on from the provided credentials.
        /// </summary>
        /// <param name="model">The <see cref="LogonModel"/> containing the authentication credentials.</param>
        /// <param name="returnUrl">The optional redirect url.</param>
        /// <param name="cancellation">Used to signal an asynchronous operation to be canceled.</param>
        [HttpPost()]
        [AllowAnonymous()]
        [ValidateAntiForgeryToken()]
        public virtual async Task<ActionResult> LogOn(LogonModel model, String returnUrl, CancellationToken cancellation)
        {
            var userName = model.UserName;
            var password = model.Password;
            var rememberMe = model.RememberMe;

            if (this.ModelState.IsValid)
            {
                var passedCaptcha = await this.verifier.Verify(this.Request, cancellation);
                if (!passedCaptcha)
                {
                    this.ModelState.AddModelError("", "Please verify the reCAPTCHA.");
                    return this.View(model);
                }

                //var result = await this.membershipService.ValidateUserAsync(userName, password, cancellation);
                var result = await this.membershipService.ValidateUserAsync(userName, password, cancellation);
#if DEBUG
                result = LoginRequestResult.Success;
#endif
                if (result == LoginRequestResult.Success)
                {
                    var principal = await this.membershipService.GetPrincipalAsync(userName, System.Security.Claims.AuthenticationTypes.Password, cancellation);
                    this.formsAuthentication.CreateAuthenticationToken(principal, rememberMe);

                    this.OnEvent($"{model.UserName} logon");

                    this.Response.Cookies["GMT Offset"].Value = model.Offset.ToString();

                    var user = (await this.context.SetOf<User>()
                            .InteractiveUser()
                            .Join(this.context.SetOf<Access>(), u => u.Id, a => a.Id, (u, a) => a)
                            .FirstAsync(cancellation))
                        .ToPrincipal(System.Security.Claims.AuthenticationTypes.Password);

                    await this.bus.Publish(new InteractiveLogonEvent() {UserId = user.Identity.GetIdentifier()});

                    this.HttpContext.User = user;

                    if (principal.Identity.IsLimitedAccess())
                    {
                        return this.NavigationFor<LeadSummaryController>().ToIndex();
                    }

                    if (!String.IsNullOrEmpty(returnUrl))
                    {
                        return this.Redirect(returnUrl);
                    }

                    var action = String.IsNullOrEmpty(model.ReturnUrl)
                                     ? this.NavigationFor<SummaryController>().ToIndex()
                                     : this.Redirect(model.ReturnUrl);
                    return action;
                }
            }

            // If we got this far, something failed, redisplay form
            this.ModelState.AddModelError("", "The user name or password provided is incorrect.");

            return this.View(model);
        }

        #endregion
    }
}
