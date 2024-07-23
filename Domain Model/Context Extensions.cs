using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using AccurateAppend.Core;
using AccurateAppend.Data;
using AccurateAppend.Security;

namespace AccurateAppend.Websites.Admin
{
    /// <summary>
    /// Extension methods for the <see cref="ISessionContext"/> interface.
    /// </summary>
    public static class ContextExtensions
    {
        #region CurrentUser

        /// <summary>
        /// Returns the <see cref="User"/> for the current interactive identity.
        /// </summary>
        /// <param name="context">The <see cref="ISessionContext"/> to scope to.</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> used to signal that an operation should be canceled.</param>
        /// <returns>The <see cref="User"/> instance.</returns>
        /// <exception cref="InvalidOperationException">The current thread identity could not be found in the database.</exception>
        public static async Task<User> CurrentUserAsync(this ISessionContext context, CancellationToken cancellation = new CancellationToken())
        {
            Debug.Assert(context != null);

            String id = null;
            if (HttpContext.Current != null)
            {
                id = HttpContext.Current.User.Identity.Name;
            }
            if (String.IsNullOrEmpty(id))
            {
                id = Thread.CurrentPrincipal.Identity.Name;
            }
            var user = await context
                .SetOf<User>()
                .FirstOrDefaultAsync(u => u.UserName == id, cancellation)
                .ConfigureAwait(false);

            if (user == null)
            {
                throw new InvalidOperationException($"The current user '{id}' cannot be found in the database.");
            }

            return user;
        }

        #endregion
    }
}

namespace AccurateAppend.Security
{
    /// <summary>
    /// Extension methods for the <see cref="System.Security.Principal.IIdentity"/> type (specifically as related to <see cref="Access.ToPrincipal"/>).
    /// </summary>
    public static class ContextExtensions
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