using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Data;
using AccurateAppend.Security;
using DomainModel.ActionResults;
using Integration.NationBuilder.Data;
using Integration.NationBuilder.Service;

namespace AccurateAppend.Websites.Clients.Areas.NationBuilder.AuthHandler
{
    /// <summary>
    /// Controller that handles oAuth and <see cref="Registration"/> logic for NationBuilder integration.
    /// </summary>
    [Authorize()]
    public class Controller : System.Web.Mvc.Controller
    {
        #region Fields

        public const String StateField = "access request";

        private readonly ISessionContext context;
        private readonly ConfigurationSelector configurationManager;
        private readonly IFormsAuthentication fa;
        private readonly IMembershipService ms;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Controller"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ISessionContext"/> component providing data access to this instance.</param>
        /// <param name="ms">The <see cref="IMembershipService"/> component that provides membership functionality.</param>
        /// <param name="fa">The <see cref="IFormsAuthentication"/> component that provides logon functionality.</param>
        /// <param name="configurationManager">The <see cref="ConfigurationSelector"/> to oAuth token integration with NationBuilder.</param>
        public Controller(ISessionContext context, IMembershipService ms, IFormsAuthentication fa, ConfigurationSelector configurationManager)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (ms == null) throw new ArgumentNullException(nameof(ms));
            if (fa == null) throw new ArgumentNullException(nameof(fa));
            if (configurationManager == null) throw new ArgumentNullException(nameof(configurationManager));
            Contract.EndContractBlock();

            this.context = context;
            this.ms = ms;
            this.fa = fa;
            this.configurationManager = configurationManager;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Handles starting an oAuth session between Accurate Append and NationBuilder.
        /// </summary>
        public virtual ActionResult Initiate(String slug)
        {
            slug = (slug ?? String.Empty).Trim();
            if (String.IsNullOrEmpty(slug)) return this.DisplayErrorResult("We're sorry but your Nation name was not provided in your request.");

            var settings = this.configurationManager.Select(this.Request);

            var access = new AccessPoint(slug, settings.ClientId, settings.Secret) { EmailAddress = this.User.Identity.Name };

            this.Session[StateField] = access;
            return this.Redirect(access.BuildAuthUri(this.HttpContext));
        }

        /// <summary>
        /// Handles functions related to oAuth integration with NationBuilder.
        /// </summary>
        public virtual async Task<ActionResult> Index(String code, CancellationToken cancellation)
        {
            var access = this.Session[StateField] as AccessPoint ?? new AccessPoint();
            this.TryValidateModel(access);

            if (!this.ModelState.IsValid) return this.DisplayErrorResult("We apologize but your request has timed out.");

            try
            {
                String accessToken;
                var tokenService = new DefaultTokenService(AccessPoint.BuildTokenHandlerUri(this.HttpContext));

                try
                {
                    accessToken = await tokenService.RequestTokenAsync(access.Slug, code, access.ClientId, access.Secret);
                }
                catch (TokenRequestFailureException ex)
                {
                    EventLogger.Logger.LogEvent(ex, Severity.High, "NationBuilder Token Request Failure");

                    return this.DisplayErrorResult($"We apologize but we were unable to contact your Nation to request access. Error:{ex.Message}");
                }

                using (var uow = this.context.CreateScope(ScopeOptions.AutoCommit))
                {
                    var client = await this.context.SetOf<ClientRef>()
                        .Where(c => c.UserName == this.User.Identity.Name)
                        .FirstOrDefaultAsync(cancellation);

                    if (client == null) return this.DisplayErrorResult("The provided Accurate Append account does not exist.");

                    var registration = await this.context.SetOf<Registration>()
                            .RegistrationsForInteractiveUser()
                            .Where(r => r.NationName == access.Slug)
                            .Include(r => r.Marketing)
                            .FirstOrDefaultAsync(cancellation);

                    if (registration != null)
                    {
                        registration.LatestAccessToken = accessToken;
                        registration.Reactivate(); // If we're going through a token request, they obviously want the registration active again
                    }
                    else
                    {
                        registration = new Registration(client, access.Slug, accessToken, access.ClientId, access.Secret);
                        this.context.SetOf<Registration>().Add(registration);
                    }

                    await registration.Marketing.UpdatePersonCount(ClientFactory.CreateStandardApi(registration.NationName));

                    await uow.CommitAsync(cancellation);

                    Debug.Assert(registration.Id != null);

                    this.Session.Remove(StateField);

                    var principal = await this.ms.GetPrincipalAsync(this.User.Identity.Name, System.Security.Claims.AuthenticationTypes.Password, cancellation);
                    this.fa.CreateAuthenticationToken(principal);

                    var sb = new StringBuilder();
                    sb.Append("Authorization complete. Redirecting...");

                    // Redirect script
                    sb.Append(@"<script>");
                    var redirectTo = this.Url.Action("Start", "Order", new { area = "NationBuilder" });
                    sb.Append($"window.location.replace('{redirectTo}');");
                    sb.Append("</script>");

                    var result = new LiteralResult(false);
                    result.Data = sb.ToString();

                    return result;
                }
            }
            catch (Exception ex)
            {
                EventLogger.Logger.LogEvent(ex, Severity.High, "NationBuilder Token Handler Failure");
                throw;
            }
        }
        #endregion
    }
}