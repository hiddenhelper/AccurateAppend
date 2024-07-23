using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Mvc;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Data;
using AccurateAppend.Security;
using AccurateAppend.Websites.Clients.Areas.Authentication.Shared.Models;
using AccurateAppend.Websites.Clients.Security;
using DomainModel.MvcModels;
using EventLogger;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.Authentication.Facebook
{
    [AllowAnonymous()]
    public class Controller : System.Web.Mvc.Controller
    {
        #region Fields

        private const String ViewPath = "~/Areas/Authentication/Shared/Views/LinkAccounts.cshtml";

        private readonly FacebookSecurityConfiguration configuration;
        private readonly Accounting.DataAccess.DefaultContext context;
        private readonly IFormsAuthentication fa;
        private readonly IMembershipService ms;
        private readonly IMessageSession bus;

        #endregion

        #region Constructor

        public Controller(Accounting.DataAccess.DefaultContext context, FacebookSecurityConfiguration configuration, IFormsAuthentication fa, IMembershipService ms, IMessageSession bus)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (fa == null) throw new ArgumentNullException(nameof(fa));
            if (ms == null) throw new ArgumentNullException(nameof(ms));
            if (bus == null) throw new ArgumentNullException(nameof(bus));
            Contract.EndContractBlock();

            this.context = context;
            this.configuration = configuration;
            this.fa = fa;
            this.ms = ms;
            this.bus = bus;
        }

        #endregion

        #region Action Methods

        public virtual ActionResult Login(String redirectTo)
        {
            if (!String.IsNullOrWhiteSpace(redirectTo)) Session["Social Redirect To"] = redirectTo;

            var location = this.configuration.CreateLogonUrl(this.HttpContext);
            return this.Redirect(location);
        }

        public virtual async Task<ActionResult> DoPostback(CancellationToken cancellation)
        {
            var logonResult = await this.configuration.HandleAuthPostback(this.HttpContext);
            if (logonResult.Type == ExternalLogonResult.Accepted)
            {
                var name = await this.configuration.LookupUserDetails(logonResult);

                var externalIdentifier = logonResult.Identifier;
                var logons = this.context.SetOf<MappedIdentityLogon>().Include(l => l.User);
                var logon = await logons.FirstOrDefaultAsync(l => l.Identity == externalIdentifier && l.Provider == IdentityProvider.Facebook, cancellation);

                // new identity and user not logged on; Display UI
                if (logon == null && !this.User.Identity.IsAuthenticated)
                {
                    return this.View(ViewPath, new LinkModel()
                    {
                        ExternalIdentifier = externalIdentifier,
                        Provider = IdentityProvider.Facebook,
                        DisplayName = name,
                        Postback = new MvcActionModel() { ActionName ="Link", AreaName = "Authentication", ControllerName = "Facebook" }
                    });
                }

                using (var transaction = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
                {
                    // new identity but user logged on; Insert
                    if (logon == null)
                    {
                        var user = await this.context.SetOf<User>().InteractiveUser().FirstAsync(cancellation);
                        logon = new MappedIdentityLogon(user, externalIdentifier, IdentityProvider.Facebook);

                        this.context.SetOf<MappedIdentityLogon>().Add(logon);

                        var @event = new ExternalLoginAddedEvent(logon);
                        await this.bus.Publish(@event);
                    }

                    logon.DisplayName = name ?? String.Empty;
                    
                    await this.context.SaveChangesAsync(cancellation);

                    transaction.Complete();
                }

                this.configuration.FinalizeLogon(this.HttpContext);

                if (!logon.User.IsLockedOut)
                {
                    logon.User.MarkActivityMoment();
                    var principal = await this.ms.GetPrincipalAsync(logon.User.UserName, Core.IdentityModel.AuthenticationTypes.Facebook, cancellation);
                    
                    if (!this.User.Identity.IsAuthenticated) this.fa.CreateAuthenticationToken(principal);

                    await this.context.SaveChangesAsync(cancellation);

                    if (this.Session["Social Redirect To"] != null)
                    {
                        var redirect = this.Session["Social Redirect To"].ToString();
                        this.Session.Remove("Social Redirect To");
                        return this.Redirect(redirect);
                    }

                    return this.RedirectToAction("Index", "Current", new { Area = "Order" });
                }

                return this.DisplayErrorResult("Your Accurate Append account is currently disabled. Please contact support.");
            }
            if (logonResult.Type == ExternalLogonResult.UserRejected)
            {
                var location = this.configuration.ReRequestPermissions(this.HttpContext);
                return this.Redirect(location);
            }

            return this.DisplayErrorResult("Facebook did not provide us a meaningful reply when accessing your profile. Please attempt to logon again. If the problem persists, please contact customer support.");
        }

        public async Task<ActionResult> Link(LinkModel model, CancellationToken cancellation)
        {
            model.Postback = new MvcActionModel() {ActionName = "Link", AreaName = "Authentication", ControllerName = "Facebook"};

            if (!this.ModelState.IsValid) return this.View(ViewPath, model);

            var logon = await this.ms.ValidateUserAsync(model.UserName, model.Password, cancellation);
            if (logon != LoginRequestResult.Success)
            {
                this.ModelState.AddModelError(nameof(LinkModel.UserName), "User name or password is invalid");
                return this.View(ViewPath, model);
            }

            try
            {
                var principal = await this.ms.GetPrincipalAsync(model.UserName, Core.IdentityModel.AuthenticationTypes.Facebook, cancellation);

                var identitites = this.context.SetOf<MappedIdentityLogon>().Include(i => i.User);
                var identity = await identitites.FirstOrDefaultAsync(i => i.Identity == model.ExternalIdentifier && i.Provider == model.Provider, cancellation);

                if (identity != null && identity.User.UserName != this.User.Identity.Name)
                {
                    return this.DisplayErrorResult("We're sorry but this Facebook account is already associated with another Accurate Append user. Perhaps you have more than one account? You can always reset your Facebook permissions and start over from scratch. Please contact customer support for further guidance.");
                }

                var interactiveUser = await this.context.SetOf<User>().Where(u => u.UserName == principal.Identity.Name).FirstAsync(cancellation);

                identity = new MappedIdentityLogon(interactiveUser, model.ExternalIdentifier, model.Provider);
                identity.DisplayName = model.DisplayName ?? String.Empty;

                this.context.SetOf<MappedIdentityLogon>().Add(identity);

                using (var transaction = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
                {
                    var @event = new ExternalLoginAddedEvent(identity);

                    await this.bus.Publish(@event);
                    await this.context.SaveChangesAsync(cancellation);

                    transaction.Complete();
                }

                this.fa.CreateAuthenticationToken(principal, model.RememberMe);

                return this.RedirectToAction("Index", "Current", new { Area = "Order" });
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached) Debugger.Break();

                Logger.LogEvent(ex, Severity.High);
                return this.DisplayErrorResult("We're sorry but we were unable to link your Facebook account with this logon. Please contact customer support for further guidance.");
            }
            finally
            {
                this.configuration.FinalizeLogon(this.HttpContext);
            }
        }

        #endregion
    }
}