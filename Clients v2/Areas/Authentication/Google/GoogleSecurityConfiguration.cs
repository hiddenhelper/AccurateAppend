using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using AccurateAppend.Websites.Clients.Security;
using RestSharp;

namespace AccurateAppend.Websites.Clients.Areas.Authentication.Google
{
    /// <summary>
    /// Provides authentication configuration and API logon integration to Google+.
    /// </summary>
    /// <remarks>
    /// https://console.developers.google.com/project
    /// 
    /// Google+ logon sequence involves redirecting a user to a specific Google url containing our application id and an embedded 
    /// value containing a url that is capable of handling the response. If the user has not already authorized our application they
    /// will be presented with a UI (at Google) displaying our company and application details (as entered by us); Otherwise they
    /// will be automatically sent back to our application.
    /// 
    /// The response indicating the status of the logon request is received at our indicated handler from the initial request, the
    /// returned values will be used to determine if the logon was successful or if the client has refused our permissions.
    /// 
    /// If the request is denied, we can return them to Google to request permissions.
    /// 
    /// If the request is accepted, we can use a one-time use code in a server-side call to Google to get the encoded user details
    /// which then in turn can be decoded into a unique user identifier specific to our application.
    /// </remarks>
    public class GoogleSecurityConfiguration : ExternalLogonConfiguration
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleSecurityConfiguration"/> class.
        /// </summary>
        /// <param name="appId">The <see cref="ExternalLogonConfiguration.AppId"/> value.</param>
        /// <param name="secret">The <see cref="ExternalLogonConfiguration.Secret"/> value.</param>
        public GoogleSecurityConfiguration(String appId, String secret)
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
        protected override String PostbackRelativePath => "Authentication/Google/DoPostback";

        /// <summary>
        /// Creates the url that a client should be redirected to in order to use Google+ logon.
        /// </summary>
        /// <param name="context">The current <see cref="HttpContextBase"/> that the user request is operating under.</param>
        /// <returns>The absolute url that the client should be redirected to for logon.</returns>
        public override String CreateLogonUrl(HttpContextBase context)
        {
            if (context.Session == null) throw new InvalidOperationException("The supplied HTTP Context for the request does not have session enabled.");

            var callback = this.CreateCallbackUrl(context);
            var state = context.Session.SessionID;

            var location = $"https://accounts.google.com/o/oauth2/auth?scope=profile&state={state}&redirect_uri={callback}&response_type=code&client_id={this.AppId}";

            return location;
        }

        /// <summary>
        /// Creates the url that a client should be redirected to in when they've declined authorize our application to use Google+ logon.
        /// </summary>
        /// <param name="context">The current <see cref="HttpContextBase"/> that the user request is operating under.</param>
        /// <returns>The absolute url that the client should be redirected to for logon.</returns>
        public override String ReRequestPermissions(HttpContextBase context)
        {
            if (context.Session == null) throw new InvalidOperationException("The supplied HTTP Context for the request does not have session enabled.");

            var callback = this.CreateCallbackUrl(context);
            var state = context.Session.SessionID;

            var location = $"https://accounts.google.com/o/oauth2/auth?scope=profile&state={state}&redirect_uri={callback}&response_type=code&client={this.AppId}&approval_prompt=force";

            return location;
        }

        /// <summary>
        /// Handles the response of a logon request from Google+.
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

                    var identifier = details.user_id;
                    result = new LogonResult()
                    {
                        Identifier = identifier,
                        Token = token.access_token,
                        Type = ExternalLogonResult.Accepted
                    };
                }
            }
            else if (errorReason == "access_denied")
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
        /// <remarks>Only the display name is currently supported. The <see cref="LogonResult.Token"/> value is ignored and Google+ leverages a Server key configuration instead.</remarks>
        /// <param name="logonResult">The result to acquire user detail information for.</param>
        /// <returns>The display name of the provided <see cref="LogonResult"/>.</returns>
        public override async Task<String> LookupUserDetails(LogonResult logonResult)
        {
            if (logonResult == null) throw new ArgumentNullException(nameof(logonResult));
            if (logonResult.Type != ExternalLogonResult.Accepted) throw new InvalidOperationException("The supplied LogonResult must be a successful authentication response");
            Contract.EndContractBlock();

            var uri = $"/v1/people/{logonResult.Identifier}?access_token={logonResult.Token}&personFields=names";

            var client = new RestClient(new Uri("https://people.googleapis.com", UriKind.Absolute));
            var request = new RestRequest(new Uri(uri, UriKind.Relative));
            request.Method = Method.GET;

            var response = await client.ExecuteTaskAsync<UserDetail>(request).ConfigureAwait(false);

            return response.Data?.names?.FirstOrDefault()?.displayName;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Uses the provided one-time use <paramref name="code"/> value from Google+ to acquire a <see cref="TokenResponse">user token</see>.
        /// </summary>
        /// <param name="context">The current <see cref="HttpContextBase"/> that the user request is operating under.</param>
        /// <param name="code">The one time use code from Facebook that can be used to request a user token with.</param>
        /// <returns>The <see cref="TokenResponse"/> containing the user details.</returns>
        protected virtual async Task<TokenResponse> GetUserToken(HttpContextBase context, String code)
        {
            var client = new RestClient(new Uri("https://www.googleapis.com", UriKind.Absolute));
            var request = new RestRequest(new Uri("/oauth2/v3/token", UriKind.Relative));
            request.Method = Method.POST;
            request.AddParameter("code", code);
            request.AddParameter("client_id", this.AppId);
            request.AddParameter("client_secret", this.Secret);
            request.AddParameter("redirect_uri", this.CreateCallbackUrl(context));
            request.AddParameter("grant_type", "authorization_code");

            var response = await client.ExecuteTaskAsync<TokenResponse>(request).ConfigureAwait(false);
            
            return response.Data;
        }

        private async Task<Token> InspectToken(TokenResponse userToken)
        {
            var uri = $"/oauth2/v1/tokeninfo?access_token={userToken.access_token}";

            var client = new RestClient(new Uri("https://www.googleapis.com", UriKind.Absolute));
            var request = new RestRequest(new Uri(uri, UriKind.Relative));

            var response = await client.ExecuteTaskAsync<Token>(request).ConfigureAwait(false);

            return response.Data;
        }

        #endregion

        #region Nested Types

        // ReSharper disable InconsistentNaming

        [DebuggerDisplay("{" + nameof(access_token) + "}")]
        protected class TokenResponse
        {

            public String access_token { get; set; }

            public String token_type { get; set; }

            public Int32 expires_in { get; set; }

            public String error { get; set; }

            public String error_description { get; set; }
        }

        [DebuggerDisplay("{" + nameof(user_id) + "}")]
        protected class Token
        {
            public String audience { get; set; }

            public String user_id { get; set; }

            public String scope { get; set; }

            public String expires_in { get; set; }
        }


        /*{
          "resourceName": "people/111687034476224954266",
          "etag": "%EgUBAj03LhoEAQIFBw==",
          "names": [
            {
              "metadata": {
                "primary": true,
                "source": {
                  "type": "PROFILE",
                  "id": "111687034476224954266"
                }
              },
              "displayName": "Jimmy Zimmerman",
              "familyName": "Zimmerman",
              "givenName": "Jimmy",
              "displayNameLastFirst": "Zimmerman, Jimmy"
            }
          ]
        }*/
        [DebuggerDisplay("{" + nameof(resourceName) + "}")]
        protected class UserDetail
        {
            public String resourceName { get; set; }

            public List<Name> names { get; set; }
        }

        [DebuggerDisplay("{" + nameof(displayName) + "}")]
        public class Name
        {
            public String displayName { get; set; }
        }
        // ReSharper restore InconsistentNaming

        #endregion
    }
}