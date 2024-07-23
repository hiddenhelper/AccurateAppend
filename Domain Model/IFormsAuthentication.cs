using System;
using System.Diagnostics.Contracts;
using System.Security.Claims;
using System.Threading;

// ReSharper disable CheckNamespace
namespace AccurateAppend.Security
{
    /// <summary>
    /// The FormsAuthentication type is sealed and contains static members, so it is difficult to
    /// unit test code that calls its members. The interface and helper class below demonstrate
    /// how to create an abstract wrapper around such a type in order to make code depending on this
    /// unit testable.
    /// </summary>
    [ContractClass(typeof(IFormsAuthenticationContracts))]
    public interface IFormsAuthentication
    {
        /// <summary>
        /// Provides an abstraction for <see cref="System.Web.Security.FormsAuthentication.SetAuthCookie(String, Boolean)"/> method.
        /// </summary>
        /// <param name="principal">The <see cref="ClaimsPrincipal"/> for the authenticated user.</param>
        /// <param name="isPersistentSession">Indicates whether the logon is persistent across sessions.</param>
        void CreateAuthenticationToken(ClaimsPrincipal principal, Boolean isPersistentSession = false);

        /// <summary>
        /// Clears the current session and logs the user out.
        /// </summary>
        void SignOut();
    }

    // ReSharper disable InconsistentNaming
    [ContractClassFor(typeof(IFormsAuthentication))]
    internal abstract class IFormsAuthenticationContracts : IFormsAuthentication
    {
        void IFormsAuthentication.CreateAuthenticationToken(ClaimsPrincipal principal, Boolean isPersistentSession)
        {
            Contract.Requires<ArgumentNullException>(principal != null, nameof(principal));
            Contract.Requires<InvalidOperationException>(principal.Identity.IsAuthenticated, "User Principal is not authenticated. An authentication token cannot be created.");
            Contract.Ensures(Thread.CurrentPrincipal == principal);
        }

        void IFormsAuthentication.SignOut()
        {
        }
    }
    // ReSharper restore InconsistentNaming
}
// ReSharper restore CheckNamespace