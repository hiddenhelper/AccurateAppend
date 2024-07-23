using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using AccurateAppend.Security;

namespace AccurateAppend.Websites.Admin
{
    /// <summary>
    /// Role related methods for determining admin functionality security.
    /// </summary>
    public static class SecurityExtensions
    {
        #region Is2020ConnectAdmin

        /// <summary>
        /// Determines whether the current <paramref name="identity"/> has the '2020Connect Admin' role.
        /// </summary>
        /// <param name="identity">The <see cref="IIdentity"/> instance to confirm.</param>
        /// <returns>True if the <paramref name="identity"/> has the role; Otherwise false.</returns>
        public static Boolean Is2020ConnectAdmin(this IIdentity identity)
        {
            var claims = identity as ClaimsIdentity ?? new ClaimsIdentity(identity);
            if (identity.IsSuperUser()) return true;

            var result = claims.HasClaim(ClaimTypes.Role, "2020Connect");

            return result;
        }

        /// <summary>
        /// Determines whether the interactive user has the '2020Connect Admin' role.
        /// </summary>
        /// <returns>True if the user has the role; Otherwise false.</returns>
        public static Boolean Is2020ConnectAdmin()
        {
            return Thread.CurrentPrincipal.Identity.Is2020ConnectAdmin();
        }

        #endregion

        #region IsAccurateAppendAdmin

        /// <summary>
        /// Determines whether the current <paramref name="identity"/> has the 'AccurateAppend Admin' role.
        /// </summary>
        /// <param name="identity">The <see cref="IIdentity"/> instance to confirm.</param>
        /// <returns>True if the <paramref name="identity"/> has the role; Otherwise false.</returns>
        public static Boolean IsAccurateAppendAdmin(this IIdentity identity)
        {
            var claims = identity as ClaimsIdentity ?? new ClaimsIdentity(identity);
            if (identity.IsSuperUser()) return true;

            var result = claims.HasClaim(ClaimTypes.Role, "AccurateAppend");

            return result;
        }

        /// <summary>
        /// Determines whether the interactive user has the 'AccurateAppend Admin' role.
        /// </summary>
        /// <returns>True if the user has the role; Otherwise false.</returns>
        public static Boolean IsAccurateAppendAdmin()
        {
            return Thread.CurrentPrincipal.Identity.IsAccurateAppendAdmin();
        }

        #endregion

        #region IsLimitedAccess

        /// <summary>
        /// Determines whether the current <paramref name="identity"/> has the 'Limited Access' role.
        /// </summary>
        /// <param name="identity">The <see cref="IIdentity"/> instance to confirm.</param>
        /// <returns>True if the <paramref name="identity"/> has the role; Otherwise false.</returns>
        public static Boolean IsLimitedAccess(this IIdentity identity)
        {
            var claims = identity as ClaimsIdentity ?? new ClaimsIdentity(identity);
            var result = claims.HasClaim(ClaimTypes.Role, "Limited") ||
                         claims.HasClaim(ClaimTypes.Role, "Limited Access");

            return result;
        }

        /// <summary>
        /// Determines whether the interactive user has the 'Limited Access' role.
        /// </summary>
        /// <returns>True if the user has the role; Otherwise false.</returns>
        public static Boolean IsLimitedAccess()
        {
            return Thread.CurrentPrincipal.Identity.IsLimitedAccess();
        }

        #endregion

        #region IsVendor

        /// <summary>
        /// Determines whether the current <paramref name="identity"/> has the 'Limited Access' role.
        /// </summary>
        /// <param name="identity">The <see cref="IIdentity"/> instance to confirm.</param>
        /// <returns>True if the <paramref name="identity"/> has the role; Otherwise false.</returns>
        public static Boolean IsVendor(this IIdentity identity)
        {
            var claims = identity as ClaimsIdentity ?? new ClaimsIdentity(identity);
            var result = claims.HasClaim(ClaimTypes.Role, "Vendor");

            return result;
        }

        /// <summary>
        /// Determines whether the interactive user has the 'Limited Access' role.
        /// </summary>
        /// <returns>True if the user has the role; Otherwise false.</returns>
        public static Boolean IsVendor()
        {
            return Thread.CurrentPrincipal.Identity.IsVendor();
        }

        #endregion
    }
}