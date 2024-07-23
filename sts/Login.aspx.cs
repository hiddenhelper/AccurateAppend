using System;

namespace AccurateAppend.SecurityTokenServer
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            const String location = "https://www.facebook.com/dialog/oauth?client_id=" + Constants.AppId + "&redirect_uri=" + Constants.RedirectUrl;
            Response.Redirect(location);
        }
    }
}