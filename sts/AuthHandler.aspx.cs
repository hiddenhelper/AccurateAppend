using System;
using System.Collections.Generic;
using RestSharp;

namespace AccurateAppend.SecurityTokenServer
{
    //jimmy userid=10152838553108295
    //app token=1573277119618200|D7eFUyhWaDD-_25MxW9xoelT58g
    //{"error":{"message":"This authorization code has been used.","type":"OAuthException","code":100}} BadRequest
    public partial class AuthHandler : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            foreach (var key in Request.QueryString.AllKeys)
            {
                Response.Write(key +":" + Request.QueryString[key] + "<br />");
            }
            var code = Request.QueryString["code"];
            if (String.IsNullOrWhiteSpace(code))
            {
                
            }

            var userToken = this.GetUserToken(code);

            var appToken = this.GetAppToken();

            var user = this.InspectToken(userToken, appToken);
        }

        protected virtual TokenResponse GetUserToken(String code)
        {
            var uri = "/v2.3/oauth/access_token?client_id=" + Constants.AppId +
                     "&redirect_uri=" + Constants.RedirectUrl + "&client_secret=" + Constants.Secret + "&code=" + code;

            var client = new RestClient(new Uri("https://graph.facebook.com", UriKind.Absolute));
            var request = new RestRequest(new Uri(uri, UriKind.Relative));

            var response = client.Execute<TokenResponse>(request);

            Response.Write("<br> />User Token: " + response.Content);

            return response.Data;
        }

        protected virtual String GetAppToken()
        {
            var uri = "/oauth/access_token?client_id=" + Constants.AppId + "&client_secret=" + Constants.Secret +
                      "&grant_type=client_credentials";

            var client = new RestClient(new Uri("https://graph.facebook.com", UriKind.Absolute));
            var request = new RestRequest(new Uri(uri, UriKind.Relative));

            var response = client.Execute(request);

            Response.Write("<br> />App Token: " + response.Content);

            return response.Content.Replace("access_token=", String.Empty);
        }
        
        private TokenDetails InspectToken(TokenResponse userToken, String appToken)
        {
            var uri = "/debug_token?input_token=" + userToken.access_token + "&access_token=" + appToken;

            var client = new RestClient(new Uri("https://graph.facebook.com", UriKind.Absolute));
            var request = new RestRequest(new Uri(uri, UriKind.Relative));

            var response = client.Execute<TokenDetails>(request);

            Response.Write("<br> />Token Details: " + response.Content);

            return response.Data;
        }
    }

    public class TokenResponse
    {
        public String access_token { get; set; }
        
        public String token_type { get; set; }

        public Int32 expires_in { get; set; }
    }

    public class TokenDetails
    {
        public Token data { get; set; }
    }

    public class Token
    {
        public String app_id { get; set; }

        public String application { get; set; }

        public Int32 expires_at { get; set; }

        public Boolean is_valid { get; set; }

        public Int32 issued_at { get; set; }

        public List<String> scopes { get; set; }

        public String user_id { get; set; }
    }
}