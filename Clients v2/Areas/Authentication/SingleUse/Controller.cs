using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Data;
using AccurateAppend.Security;
using AccurateAppend.Websites.Clients.Security;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.Authentication.SingleUse
{
    /// <summary>
    /// Controller for allowing single use administrative logon for a user.
    /// </summary>
    public class Controller : System.Web.Mvc.Controller
    {
        #region Fields

        private readonly ISessionContext context;
        private readonly IFormsAuthentication fa;
        private readonly IMembershipService ms;
        private readonly IMessageSession bus;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Controller"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ISessionContext"/> dal component.</param>
        /// <param name="ms">The <see cref="IMembershipService"/> component that provides membership functionality.</param>
        /// <param name="fa">The <see cref="IFormsAuthentication"/> component that provides logon functionality.</param>
        public Controller(ISessionContext context, IMembershipService ms, IFormsAuthentication fa, IMessageSession bus)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (ms == null) throw new ArgumentNullException(nameof(ms));
            if (fa == null) throw new ArgumentNullException(nameof(fa));
            Contract.EndContractBlock();

            this.context = context;
            this.ms = ms;
            this.fa = fa;
            this.bus = bus;
        }

        #endregion

        #region Action Methods

        public virtual async Task<ActionResult> Login(Guid id, String email, CancellationToken cancellation)
        {
#if DEBUG
            if (id == new Guid("B33A7985-5CDD-493D-8402-A01503011B5D"))
            {
                using (this.context.CreateScope(ScopeOptions.ReadOnly))
                {
                    // Confirm this account exists
                    if (!await this.context
                        .SetOf<User>()
                        .AnyAsync(u => u.UserName == email, cancellation)) throw new Exception("Bad email");

                    var principal = await this.ms.GetPrincipalAsync(email, System.Security.Claims.AuthenticationTypes.Password, cancellation);
                    this.fa.CreateAuthenticationToken(principal);
                    return this.RedirectToAction("Index", "Current", new {Area = "Order"});

                }
            }
#endif
            using (var uow = this.context.CreateScope(ScopeOptions.AutoCommit))
            {
                var logon = await this.context
                    .SetOf<SingleUseLogon>()
                    .FirstOrDefaultAsync(l => l.Id == id && l.Logon.UserName == email && l.IsActive, cancellation);
                if (logon == null || !logon.TryLogon()) return this.RedirectToAction("Login", "Direct");

                var principal = await this.ms.GetPrincipalAsync(email, System.Security.Claims.AuthenticationTypes.Password, cancellation);
                principal.Identities.First().Impersonate(WellKnownIdentifiers.OperationsUserId);

                // publish an event
                await MembershipServiceObserver.OnLogon(this.bus, email);

                this.fa.CreateAuthenticationToken(principal);

                await uow.CommitAsync(cancellation);

                return this.RedirectToAction("Index", "Current", new {Area = "Order"});
            }
        }

        #endregion
    }
}