using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core.IdentityModel;
using Newtonsoft.Json;

namespace AccurateAppend.Websites.Clients.Areas.Box.AuthHandler
{
    [Authorize()]
    public class AuthHandlerController : Controller
    {
        #region Fields

        public const String StateField = "access request";

        private readonly ConfigurationSelector configurationManager;
        private readonly DataContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthHandlerController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="DataContext"/> component providing data access.</param>
        /// <param name="configurationManager">The <see cref="ConfigurationSelector"/> to oAuth token integration with NationBuilder.</param>
        public AuthHandlerController(DataContext context, ConfigurationSelector configurationManager)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (configurationManager == null) throw new ArgumentNullException(nameof(configurationManager));
            Contract.EndContractBlock();

            this.context = context;
            this.configurationManager = configurationManager;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Handles starting an oAuth session between Accurate Append and Box.com system.
        /// </summary>
        public virtual ActionResult Initiate(String redirectTo, Guid publicKey)
        {
            var settings = this.configurationManager.Select(this.Request);
            
            var access = new AccessPoint(settings.ClientId, settings.Secret, publicKey) { EmailAddress = this.User.Identity.Name };
            this.Session[StateField] = access;

            var url = access.BuildAuthUri(this.HttpContext, redirectTo);
            return this.Redirect(url);
        }

        /// <summary>
        /// Handles functions related to oAuth integration with NationBuilder.
        /// </summary>
        public virtual async Task<ActionResult> Index(String code, String redirectTo, CancellationToken cancellation)
        {
            var access = this.Session[StateField] as AccessPoint ?? new AccessPoint();
            this.TryValidateModel(access);

            if (!this.ModelState.IsValid) return this.DisplayErrorResult("We apologize but your request has timed out.");

            var authenticationUrl = "https://api.box.com/oauth2/token";
            var client = new HttpClient();

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<String, String>("grant_type", "authorization_code"),
                new KeyValuePair<String, String>("code", code),
                new KeyValuePair<String, String>("client_id", access.ClientId),
                new KeyValuePair<String, String>("client_secret", access.Secret)
            });

            var response = await client.PostAsync(authenticationUrl, content, cancellation);
            var data = await response.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<Token>(data);

            var userId = this.User.Identity.GetIdentifier();
            var registration = BoxRegistration.Create(userId, access.Identifier, token);
            this.context.SetOf<BoxRegistration>().Add(registration);

            await this.context.SaveChangesAsync(cancellation);

            if (String.IsNullOrWhiteSpace(redirectTo)) redirectTo = this.Url.Action("Index", "Current", new {Area = "Order"});
            return this.Redirect(redirectTo);
        }

        #endregion
    }
}