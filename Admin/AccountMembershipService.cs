using System;
using System.Diagnostics.Contracts;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Security;
using AccurateAppend.Security;
using AccurateAppend.Security.DataAccess;

namespace AccurateAppend.Websites.Admin
{
    /// <summary>
    /// Default implementation of the <see cref="IMembershipService"/> interface.
    /// </summary>
    /// <remarks>
    /// This component provides access to a <see cref="MembershipProvider"/> instance
    /// for use as an account system.
    /// </remarks>
    public class AccountMembershipService : IMembershipService
    {
        #region Fields
        
        private readonly MembershipProvider provider;
        private readonly AuthenticationContext context;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountMembershipService"/> class.
        /// </summary>
        /// <remarks>
        /// Uses the default <see cref="MembershipProvider"/> configured in the application.
        /// </remarks>
        public AccountMembershipService(AuthenticationContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            Contract.EndContractBlock();

            this.context = context;
            this.provider = Membership.Provider;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountMembershipService"/> class.
        /// </summary>
        /// <param name="context">The <see cref="AuthenticationContext"/> to provide data access with.</param>
        /// <param name="provider">The <see cref="MembershipProvider"/> for this site to provide access to.</param>
        public AccountMembershipService(AuthenticationContext context, MembershipProvider provider) : this(context)
        {
            if (provider == null) throw new ArgumentNullException(nameof(provider));
            Contract.EndContractBlock();

            this.provider = provider;
        }

        #endregion

        #region IMembershipService Members

        /// <inheritdoc />
        public Task<MembershipCreateStatus> CreateUserAsync(String email, String password, Guid applicationId, CancellationToken cancellation, Boolean mustChangePassword = false)
        {
            MembershipCreateStatus status;
            this.provider.ApplicationName = "/Admin";

            if (mustChangePassword)
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
                {
                    this.provider.CreateUser(email, password, email, null, null, true, null, out status);
                    if (status == MembershipCreateStatus.Success) Task.WhenAll(this.context.MustResetPassword(email, cancellation));

                    transaction.Complete();
                }
            }
            else
            {
                this.provider.CreateUser(email, password, email, null, null, true, null, out status);
            }

            return Task.FromResult(status);
        }

        /// <inheritdoc />
        public Task<LoginRequestResult> ValidateUserAsync(String email, String password, CancellationToken cancellation)
        {
#if DEBUG
            // Used to allow remote team admin dev work
            if (String.Equals("system@accurateappend.com", email, StringComparison.OrdinalIgnoreCase)) return Task.FromResult(LoginRequestResult.Success);
#endif

            this.provider.ApplicationName = "/Admin";
            var result = this.provider.ValidateUser(email, password)
                ? LoginRequestResult.Success
                : LoginRequestResult.InvalidUserNameOrPassword;

            return Task.FromResult(result);
        }

        /// <inheritdoc />
        public Task<ClaimsPrincipal> GetPrincipalAsync(String email, String authenticationType, CancellationToken cancellation)
        {
            return this.context.Principal(email, authenticationType, cancellation);
        }

        /// <inheritdoc />
        public Task<Boolean> ChangePasswordAsync(String email, String password, Guid applicationId, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task MarkActiveAsync(Guid userId, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public HashedProof GenerateHash(String password)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Int32 MinPasswordLength => this.provider.MinRequiredPasswordLength;

        /// <inheritdoc />
        public Int32 MaxInvalidPasswordAttempts => this.provider.MaxInvalidPasswordAttempts;

        #endregion
    }
}