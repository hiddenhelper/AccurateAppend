
using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using System.Web;
using AccurateAppend.Websites.Clients.Security;
using RestSharp;


namespace AccurateAppend.Websites.Clients.Areas.Authentication.LinkedIn
{
    public class LinkedInSecurityConfiguration : ExternalLogonConfiguration
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="LinkedInSecurityConfiguration"/> class.
        /// </summary>
        /// <param name="appId">The <see cref="ExternalLogonConfiguration.AppId"/> value.</param>
        /// <param name="secret">The <see cref="ExternalLogonConfiguration.Secret"/> value.</param>
        public LinkedInSecurityConfiguration(String appId, String secret)
        {
            if (appId == null) throw new ArgumentNullException(nameof(appId));
            if (secret == null) throw new ArgumentNullException(nameof(secret));

            Contract.EndContractBlock();

            this.AppId = appId;
            this.Secret = secret;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Gets the relative Uri path that's used for this authentication callback.
        /// </summary>
        protected override String PostbackRelativePath => "Authentication/LinkedIn/DoPostback";

        /// <summary>
        /// Creates the url that a client should be redirected to in order to use the current External logon provider logon.
        /// </summary>
        /// <param name="context">The current <see cref="HttpContextBase"/> that the user request is operating under.</param>
        /// <returns>The absolute url that the client should be redirected to for logon.</returns>
        public override String CreateLogonUrl(HttpContextBase context)
        {
            var callback = this.CreateCallbackUrl(context);

            var location = $"https://www.linkedin.com/uas/oauth2/authorization?response_type=code&client_id={this.AppId}&scope=r_liteprofile&redirect_uri={callback}";

            return location;
        }

        /// <summary>
        /// Creates the url that a client should be redirected to in when they've declined authorize our application to use LinkedIn logon.
        /// </summary>
        /// <param name="context">The current <see cref="HttpContextBase"/> that the user request is operating under.</param>
        /// <returns>The absolute url that the client should be redirected to for logon.</returns>
        public override String ReRequestPermissions(HttpContextBase context)
        {
            var callback = this.CreateCallbackUrl(context);

            var location = $"https://www.linkedin.com/uas/oauth2/authorization?response_type=code&client_id={this.AppId}&r_liteprofile&redirect_uri={callback}";

            return location;
        }

        /// <summary>
        /// Handles the response of a logon request from LinkedIn.
        /// </summary>
        /// <param name="context">The current <see cref="HttpContextBase"/> that the user request is operating under.</param>
        /// <returns>The <see cref="LogonResult"/> containing the information about the logon response.</returns>
        public override async Task<LogonResult> HandleAuthPostback(HttpContextBase context)
        {
            if (context.Session == null) throw new InvalidOperationException("The provided context does not have Sessions enabled.");
            if (context.Session.IsReadOnly) throw new InvalidOperationException("The provided context has Sessions enabled but the current Session is read-only. This is likely due to the call being made a point in the ASPNET pipeline before the request state has been acquired.");

            LogonResult result;
            var code = context.Request["code"];
            var errorReason = context.Request["error"];
            if (!String.IsNullOrWhiteSpace(code))
            {
                if (context.Session["External Logon result"] is LogonResult existingResult && existingResult.Type == ExternalLogonResult.Accepted)
                {
                    result = existingResult;
                }
                else
                {
                    var token = await this.GetUserToken(context, code);
                    var details = await this.InspectToken(token);
                    var identifier = details.id;
                    result = new LogonResult()
                    {
                        Identifier = identifier,
                        Token = token.access_token,
                        Type = ExternalLogonResult.Accepted
                    };
                }
            }
            else if (errorReason == "user_denied")
            {
                result = new LogonResult() { Type = ExternalLogonResult.UserRejected };
            }
            else
            {
                result = new LogonResult() { Type = ExternalLogonResult.Unknown };
            }

            context.Session["External Logon result"] = result;
            return result;
        }

        /// <summary>
        /// Call to indicate that the external logon process has completed (regardless of success).
        /// </summary>
        /// <param name="context">The current <see cref="HttpContextBase"/> that the user request is operating under.</param>
        public override void FinalizeLogon(HttpContextBase context)
        {
            context.Session.Remove("External Logon result");
        }

        /// <summary>
        /// Allows user detail information related to this <see cref="LogonResult"/> to be acquired for a successful log-on action.
        /// </summary>
        /// <remarks>Only the display name is currently supported.</remarks>
        /// <param name="logonResult">The result to acquire user detail information for.</param>
        /// <returns>The display name of the provided <see cref="LogonResult"/>.</returns>
        public override async Task<String> LookupUserDetails(LogonResult logonResult)
        {
            if (logonResult == null) throw new ArgumentNullException(nameof(logonResult));
            if (logonResult.Type != ExternalLogonResult.Accepted) throw new InvalidOperationException("The supplied LogonResult must be a successful authentication response");
            Contract.EndContractBlock();

            const String Uri = "/v2/me";

            var client = new RestClient(new Uri("https://api.linkedin.com", UriKind.Absolute));
            var request = new RestRequest(new Uri(Uri, UriKind.Relative));
            request.AddParameter("Authorization", string.Format("Bearer " + logonResult.Token), ParameterType.HttpHeader);

            var response = await client.ExecuteTaskAsync<UserDetail>(request).ConfigureAwait(false);

            return response.Data.localizedFirstName;
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Uses the provided one-time use <paramref name="code"/> value from LinkedIn to acquire a <see cref="TokenResponse">user token</see>.
        /// </summary>
        /// <param name="context">The current <see cref="HttpContextBase"/> that the user request is operating under.</param>
        /// <param name="code">The one time use code from LinkedIn that can be used to request a user token with.</param>
        /// <returns>The <see cref="TokenResponse"/> containing the user details.</returns>
        protected virtual async Task<TokenResponse> GetUserToken(HttpContextBase context, String code)
        {
            var callback = this.CreateCallbackUrl(context);
            var uri = $"/oauth/v2/accessToken?client_id={this.AppId}&redirect_uri={callback}&client_secret={this.Secret}&grant_type=authorization_code&code={code}";

            var client = new RestClient(new Uri("https://www.linkedin.com", UriKind.Absolute));
            var request = new RestRequest(new Uri(uri, UriKind.Relative), Method.POST);

            var response = await client.ExecuteTaskAsync<TokenResponse>(request).ConfigureAwait(false);

            return response.Data;
        }

        private async Task<TokenDetails> InspectToken(TokenResponse userToken)
        {
            var uri = $"/v2/me";

            var client = new RestClient(new Uri("https://api.linkedin.com", UriKind.Absolute));
            var request = new RestRequest(new Uri(uri, UriKind.Relative));
            request.AddParameter("Authorization", string.Format("Bearer " + userToken.access_token), ParameterType.HttpHeader);

            var response = await client.ExecuteTaskAsync<TokenDetails>(request).ConfigureAwait(false);

            return response.Data;
        }

        #endregion

        #region Nested Types

        // ReSharper disable InconsistentNaming

        protected class TokenResponse
        {

            public String access_token { get; set; }

            public Int32 expires_in { get; set; }
        }

        protected class TokenDetails
        {
            public String id { get; set; }
        }


        [DebuggerDisplay("{" + nameof(localizedFirstName) + "}")]
        protected class UserDetail
        {
            public String localizedFirstName { get; set; }

            public String vanityName { get; set; }

            public String localizedLastName { get; set; }

            public String id { get; set; }
        }

        // ReSharper restore InconsistentNaming

        #endregion
    }
}
