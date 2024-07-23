using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using System.Web;
using AccurateAppend.Websites.Clients.Security;
using RestSharp;

namespace AccurateAppend.Websites.Clients.Areas.Authentication.Facebook
{
    /// <summary>
    /// Provides authentication configuration and API logon integration to Facebook.
    /// </summary>
    /// <remarks>
    /// https://developers.facebook.com/apps/
    /// https://developers.facebook.com/docs/facebook-login/manually-build-a-login-flow
    /// 
    /// Facebook logon sequence involves redirecting a user to a specific Facebook url containing our application id and an embedded 
    /// value containing a url that is capable of handling the response. If the user has not already authorized our application they
    /// will be presented with a UI (at Facebook) displaying our company and application details (as entered by us); Otherwise they
    /// will be automatically sent back to our application.
    /// 
    /// The response indicating the status of the logon request is received at our indicated handler from the initial request, the
    /// returned values will be used to determine if the logon was successful or if the client has refused our permissions.
    /// 
    /// If the request is denied, we can return them to Facebook to request permissions.
    /// 
    /// If the request is accepted, we can use a one-time use code in a server-side call to facebook to get the encoded use details
    /// which then in turn along with our application code can be decoded into a unique user identifier.
    /// </remarks>
    public class FacebookSecurityConfiguration : ExternalLogonConfiguration
    {
        #region Fields

        private readonly Lazy<String> appToken;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="FacebookSecurityConfiguration"/> class.
        /// </summary>
        /// <param name="appId">The <see cref="ExternalLogonConfiguration.AppId"/> value.</param>
        /// <param name="secret">The <see cref="ExternalLogonConfiguration.Secret"/> value.</param>
        public FacebookSecurityConfiguration(String appId, String secret)
        {
            if (appId == null) throw new ArgumentNullException(nameof(appId));
            if (secret == null) throw new ArgumentNullException(nameof(secret));

            Contract.EndContractBlock();

            this.AppId = appId;
            this.Secret = secret;
            this.appToken = new Lazy<String>(this.AcquireAppToken);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the runtime Application Token used to decode Facebook tokens.
        /// </summary>
        protected virtual String AppToken => this.appToken.Value;

        #endregion

        #region Overrides

        /// <summary>
        /// Gets the relative Uri path that's used for this authentication callback.
        /// </summary>
        protected override String PostbackRelativePath => "Authentication/Facebook/DoPostback";

        /// <summary>
        /// Creates the url that a client should be redirected to in order to use the current External logon provider logon.
        /// </summary>
        /// <param name="context">The current <see cref="HttpContextBase"/> that the user request is operating under.</param>
        /// <returns>The absolute url that the client should be redirected to for logon.</returns>
        public override String CreateLogonUrl(HttpContextBase context)
        {
            var callback = this.CreateCallbackUrl(context);

            var location = $"https://www.facebook.com/dialog/oauth?client_id={this.AppId}&redirect_uri={callback}";

            return location;
        }

        /// <summary>
        /// Creates the url that a client should be redirected to in when they've declined authorize our application to use Facebook logon.
        /// </summary>
        /// <param name="context">The current <see cref="HttpContextBase"/> that the user request is operating under.</param>
        /// <returns>The absolute url that the client should be redirected to for logon.</returns>
        public override String ReRequestPermissions(HttpContextBase context)
        {
            var callback = this.CreateCallbackUrl(context);

            var location = $"https://www.facebook.com/dialog/oauth?client_id={this.AppId}&redirect_uri={callback}&auth_type=rerequest";

            return location;
        }

        /// <summary>
        /// Handles the response of a logon request from Facebook.
        /// </summary>
        /// <param name="context">The current <see cref="HttpContextBase"/> that the user request is operating under.</param>
        /// <returns>The <see cref="LogonResult"/> containing the information about the logon response.</returns>
        public override async Task<LogonResult> HandleAuthPostback(HttpContextBase context)
        {
            if (context.Session == null) throw new InvalidOperationException("The provided context does not have Sessions enabled.");
            if (context.Session.IsReadOnly) throw new InvalidOperationException("The provided context has Sessions enabled but the current Session is read-only. This is likely due to the call being made a point in the ASPNET pipeline before the request state has been acquired.");

            LogonResult result;
            var code = context.Request["code"];
            var errorReason = context.Request["error_reason"];
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

                    var identifier = details.data.user_id;
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
                result = new LogonResult() {Type = ExternalLogonResult.UserRejected};
            }
            else
            {
                result = new LogonResult() {Type = ExternalLogonResult.Unknown};
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

            var uri = $"/v2.6/{logonResult.Identifier}?access_token={logonResult.Token}";

            var client = new RestClient(new Uri("https://graph.facebook.com", UriKind.Absolute));
            var request = new RestRequest(new Uri(uri, UriKind.Relative));
            request.Method = Method.GET;

            var response = await client.ExecuteTaskAsync<UserDetail>(request).ConfigureAwait(false);

            return response.Data.name;
        }

        #endregion

        #region Methods

        private String AcquireAppToken()
        {
            var uri = $"/oauth/access_token?client_id={this.AppId}&client_secret={this.Secret}&grant_type=client_credentials";
            var client = new RestClient(new Uri("https://graph.facebook.com", UriKind.Absolute));
            var request = new RestRequest(new Uri(uri, UriKind.Relative));

            var response = client.Execute<AppTokenResponse>(request);

            return response.Data.access_token;
        }

        /// <summary>
        /// Uses the provided one-time use <paramref name="code"/> value from Facebook to acquire a <see cref="TokenResponse">user token</see>.
        /// </summary>
        /// <param name="context">The current <see cref="HttpContextBase"/> that the user request is operating under.</param>
        /// <param name="code">The one time use code from Facebook that can be used to request a user token with.</param>
        /// <returns>The <see cref="TokenResponse"/> containing the user details.</returns>
        protected virtual async Task<TokenResponse> GetUserToken(HttpContextBase context, String code)
        {
            var callback = this.CreateCallbackUrl(context);
            var uri = $"/v2.3/oauth/access_token?client_id={this.AppId}&redirect_uri={callback}&client_secret={this.Secret}&code={code}";

            var client = new RestClient(new Uri("https://graph.facebook.com", UriKind.Absolute));
            var request = new RestRequest(new Uri(uri, UriKind.Relative));

            var response = await client.ExecuteTaskAsync<TokenResponse>(request).ConfigureAwait(false);

            return response.Data;
        }

        private async Task<TokenDetails> InspectToken(TokenResponse userToken)
        {
            var uri = $"/debug_token?input_token={userToken.access_token}&access_token={this.AppToken}";

            var client = new RestClient(new Uri("https://graph.facebook.com", UriKind.Absolute));
            var request = new RestRequest(new Uri(uri, UriKind.Relative));

            var response = await client.ExecuteTaskAsync<TokenDetails>(request).ConfigureAwait(false);

            return response.Data;
        }

        #endregion

        #region Nested Types

        // ReSharper disable InconsistentNaming

        [DebuggerDisplay("{" + nameof(access_token) + "}")]
        protected class AppTokenResponse
        {
            public String access_token { get; set; }
        }

        protected class TokenResponse
        {

            public String access_token { get; set; }

            public String token_type { get; set; }

            public Int32 expires_in { get; set; }
        }

        protected class TokenDetails
        {
            public Token data { get; set; }
        }

        [DebuggerDisplay("{" + nameof(user_id) + "}")]
        protected class Token
        {
            public String app_id { get; set; }

            public String application { get; set; }

            public Int32 expires_at { get; set; }

            public Boolean is_valid { get; set; }

            public Int32 issued_at { get; set; }

            public List<String> scopes { get; set; }

            public String user_id { get; set; }
        }

        [DebuggerDisplay("{" + nameof(name) + "}")]
        protected class UserDetail
        {
            public String name { get; set; }

            public String id { get; set; }
        }

        // ReSharper restore InconsistentNaming

        #endregion
    }
}