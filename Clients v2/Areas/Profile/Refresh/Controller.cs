using System;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Security;

namespace AccurateAppend.Websites.Clients.Areas.Profile.Refresh
{
    /// <summary>
    /// Controller to perform profile refreshes when configuration has changed.
    /// </summary>
    [Authorize()]
    public class RefreshController : Controller
    {
        #region Fields

        private readonly IFormsAuthentication fa;
        private readonly IMembershipService ms;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RefreshController"/> class.
        /// </summary>
        /// <param name="ms">The <see cref="IMembershipService"/> component that provides membership functionality.</param>
        /// <param name="fa">The <see cref="IFormsAuthentication"/> component that provides logon functionality.</param>
        public RefreshController(IMembershipService ms, IFormsAuthentication fa)
        {
            if (ms == null) throw new ArgumentNullException(nameof(ms));
            if (fa == null) throw new ArgumentNullException(nameof(fa));
            Contract.EndContractBlock();

            this.ms = ms;
            this.fa = fa;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Refreshes the current user security profile for this current session and redirects the client to the indicated path.
        /// </summary>
        public virtual async Task<ActionResult> Index(String redirectTo, CancellationToken cancellation)
        {
            if (String.IsNullOrWhiteSpace(redirectTo)) return this.DisplayErrorResult("No redirect path provided");

            var principal = await this.ms.GetPrincipalAsync(this.User.Identity.Name, this.User.Identity.AuthenticationType, cancellation);
            this.fa.CreateAuthenticationToken(principal);

            return this.Redirect(redirectTo);
        }

        #endregion
    }
}