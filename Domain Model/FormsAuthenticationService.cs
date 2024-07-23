using System;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Security;
using AccurateAppend.Core.Collections.Generic;
using AccurateAppend.Security;

namespace DomainModel
{
    public sealed class FormsAuthenticationService : IFormsAuthentication
    {
        void IFormsAuthentication.CreateAuthenticationToken(ClaimsPrincipal principal, Boolean isPersistentSession)
        {
            var userName = principal.Identity.Name;

            var claims = new StringBuilder();
            principal.Claims.ForEach(c => claims.AppendLine(c.ToString()));

            var ticket = new FormsAuthenticationTicket(2,
                userName,
                DateTime.UtcNow,
                DateTime.UtcNow.Add(FormsAuthentication.Timeout),
                isPersistentSession,
                claims.ToString(),
                FormsAuthentication.FormsCookiePath);

            var encoded = FormsAuthentication.Encrypt(ticket);

            HttpContext.Current.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encoded));
            HttpContext.Current.User = principal;

            Thread.CurrentPrincipal = principal;
        }

        void IFormsAuthentication.SignOut()
        {
            FormsAuthentication.SignOut();
        }
    }
}