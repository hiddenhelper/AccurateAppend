using System;
using System.Diagnostics.Contracts;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Security;
using AccurateAppend.Core.ComponentModel;
using AccurateAppend.Security;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Security
{
    /// <summary>
    /// Decorator for the <see cref="IMembershipService"/>. Publishes the <see cref="PublicLogonEvent"/> when a successful logon
    /// occurs. All implementation is deferred to the underlying subject.
    /// </summary>
    public sealed class MembershipServiceObserver : DecoratorBase<IMembershipService>, IMembershipService
    {
        #region Fields

        private readonly IMessageSession bus;

        #endregion

        #region Constructor

        public MembershipServiceObserver(IMessageSession bus, IMembershipService subject) : base(subject)
        {
            if (bus == null) throw new ArgumentNullException(nameof(bus));
            Contract.EndContractBlock();

            this.bus = bus;
        }

        #endregion

        #region IMembershipService Members

        Task<MembershipCreateStatus> IMembershipService.CreateUserAsync(String email, String password, Guid applicationId, CancellationToken cancellation, Boolean mustChangePassword)
        {
            return this.Subject.CreateUserAsync(email, password, applicationId, cancellation, mustChangePassword);
        }

        async Task<LoginRequestResult> IMembershipService.ValidateUserAsync(String email, String password, CancellationToken cancellation)
        {
            var result = await this.Subject.ValidateUserAsync(email, password, cancellation).ConfigureAwait(false);
            if (result == LoginRequestResult.Success)
            {
                // publish an event
                var @event = new PublicLogonEvent { UserName = email };
                await this.bus.Publish(@event);
            }

            return result;
        }

        Task<ClaimsPrincipal> IMembershipService.GetPrincipalAsync(String email, String authenticationType, CancellationToken cancellation)
        {
            return this.Subject.GetPrincipalAsync(email, authenticationType, cancellation);
        }

        async Task<Boolean> IMembershipService.ChangePasswordAsync(String email, String password, Guid applicationId, CancellationToken cancellation)
        {
            var result = await this.Subject.ChangePasswordAsync(email, password, applicationId, cancellation).ConfigureAwait(false);

            if (result)
            {
                // publish an event
                var @event = new PublicLogonEvent { UserName = email };
                await this.bus.Publish(@event);
            }

            return result;
        }

        Task IMembershipService.MarkActiveAsync(Guid userId, CancellationToken cancellation)
        {
            return null;
        }

        HashedProof IMembershipService.GenerateHash(String password)
        {
            return this.Subject.GenerateHash(password);
        }

        Int32 IMembershipService.MinPasswordLength => this.Subject.MinPasswordLength;

        Int32 IMembershipService.MaxInvalidPasswordAttempts => this.Subject.MaxInvalidPasswordAttempts;

        #endregion

        #region Helpers

        /// <remarks>
        /// Used to keep this event information from leaking. It's a temp event and this keeps it in one place.
        /// </remarks>
        public static Task OnLogon(IMessageSession bus, String email)
        {
            if (bus == null) throw new ArgumentNullException(nameof(bus));

            // publish an event
            var @event = new PublicLogonEvent { UserName = email };
            return bus.Publish(@event);
        }

        #endregion
    }
}