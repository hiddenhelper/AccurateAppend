using System;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Security;
using AccurateAppend.Core;

namespace AccurateAppend.Websites.Clients.Security
{
    /// <summary>
    /// Helper routines for the <see cref="System.Security.Principal.IIdentity"/> type and the contained claims for a current request.
    /// </summary>
    public static class RequestContextHelper
    {
        /// <summary>
        /// Provides a common routine for the reconstitution of the interactive authenticated user ClaimSet
        /// from the authentication token.
        /// </summary>
        public static void ConfigureClaimsForInteractiveUser(HttpContext context)
        {
            var currentIdentity = context?.User.Identity as FormsIdentity;
            if (currentIdentity == null) return;

            if (!currentIdentity.IsAuthenticated) return;

            var ticket = currentIdentity.Ticket;
            if (ticket == null) return;

            var ud = (ticket.UserData ?? String.Empty).Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var token in ud)
            {
                var values = token.Split(' ');
                var claimType = values.First();
                claimType = claimType.Left(claimType.Length - 1);

                var claim = new Claim(claimType, (values.Skip(1).Take(1).FirstOrDefault() ?? String.Empty).Trim());

                currentIdentity.AddClaim(claim);
            }
        }
    }
}