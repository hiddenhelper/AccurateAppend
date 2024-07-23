using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Security;
using AccurateAppend.Security;
using AccurateAppend.Security.DataAccess;

namespace AccurateAppend.Websites.Clients.Security
{
    /// <summary>
    /// Default implementation of the <see cref="IMembershipService"/> interface.
    /// </summary>
    /// <remarks>
    /// This component provides access to a <see cref="MembershipProvider"/> instance
    /// for use as an account system.
    /// </remarks>
    public class AccountMembershipService : CoreMembershipService
    {
        #region Fields

        private static readonly IDictionary<Guid, String> Applications = new Dictionary<Guid, String>
            {
                {ApplicationExtensions.AccurateAppendId, @"/"},
                {ApplicationExtensions.TwentyTwentyId, @"/emailappendNET"}
            };

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
        public AccountMembershipService(AuthenticationContext context) : base(context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            Contract.EndContractBlock();

            this.context = context;
            this.provider = Membership.Provider;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountMembershipService"/> class.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="provider">The <see cref="MembershipProvider"/> instance to provide access to.</param>
        public AccountMembershipService(AuthenticationContext context, MembershipProvider provider) : this(context)
        {
            if (provider == null) throw new ArgumentNullException(nameof(provider));
            Contract.EndContractBlock();

            this.provider = provider;
        }

        #endregion

        #region Overrides

        /// <summary>Adds a new membership user to the system.</summary>
        /// <param name="email">The e-mail address for the new user.</param>
        /// <param name="password">The password for the new user.</param>
        /// <param name="applicationId">The identifier of the <see cref="T:AccurateAppend.Security.Application" /> the user should be created in.</param>
        /// <param name="cancellation">The token to monitor for cancellation requests.</param>
        /// <param name="mustChangePassword">Indicates whether the user must change password before using the system.</param>
        /// <returns>A <see cref="T:System.Web.Security.MembershipCreateStatus" /> enumeration value indicating whether the user was created successfully.</returns>
        public override async Task<MembershipCreateStatus> CreateUserAsync(String email, String password, Guid applicationId, CancellationToken cancellation, Boolean mustChangePassword = false)
        {
            password = (password ?? String.Empty).Trim();

            if (String.IsNullOrWhiteSpace(email)) return MembershipCreateStatus.InvalidEmail;
            if (String.IsNullOrWhiteSpace(password) || password.Length < this.MinPasswordLength) return MembershipCreateStatus.InvalidPassword;
            if (!Applications.ContainsKey(applicationId)) return MembershipCreateStatus.InvalidProviderUserKey;

            var salt = this.CreateSalt();
            var hashPassword = this.HashPassword(password, salt);

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    await this.context.Database.Connection.OpenAsync(cancellation);

                    using (var cmd = this.context.Database.Connection.CreateCommand())
                    {
                        cmd.CommandText = @"
    DECLARE @userID uniqueidentifier

    exec @ReturnValue = [dbo].[aspnet_Membership_CreateUser]
    @ApplicationName=@p0,
    @UserName=@p1,
    @Password=@p2,
    @PasswordSalt=@p3,
    @Email=@p1,
    @PasswordQuestion=null,
    @PasswordAnswer=null,
    @IsApproved=1,
    @UniqueEmail=1,
    @PasswordFormat=@p4,
    @CurrentTimeUtc=@p5,
    @userID=@userID OUTPUT;";

                        if (mustChangePassword)
                        {
                            cmd.CommandText = cmd.CommandText + @"
UPDATE [dbo].[aspnet_Membership] SET [MustResetPwd] = 1 WHERE [UserId] = @userID";
                        }

                        cmd.CommandType = CommandType.Text;
                        if (this.context.Database.CommandTimeout.HasValue) cmd.CommandTimeout = this.context.Database.CommandTimeout.Value;

                        #region Parameters

                        var p0 = cmd.CreateParameter();
                        p0.ParameterName="@p0";
                        p0.DbType = DbType.String;
                        p0.Value = Applications[applicationId];
                        cmd.Parameters.Add(p0);

                        var p1 = cmd.CreateParameter();
                        p1.ParameterName = "@p1";
                        p1.DbType = DbType.String;
                        p1.Value = email;
                        cmd.Parameters.Add(p1);

                        var p2 = cmd.CreateParameter();
                        p2.ParameterName = "@p2";
                        p2.DbType = DbType.String;
                        p2.Value = hashPassword;
                        cmd.Parameters.Add(p2);

                        var p3 = cmd.CreateParameter();
                        p3.ParameterName = "@p3";
                        p3.DbType = DbType.String;
                        p3.Value = salt;
                        cmd.Parameters.Add(p3);

                        var p4 = cmd.CreateParameter();
                        p4.ParameterName = "@p4";
                        p4.DbType = DbType.Int32;
                        p4.Value = (Int32) MembershipPasswordFormat.Hashed;
                        cmd.Parameters.Add(p4);

                        var p5 = cmd.CreateParameter();
                        p5.ParameterName = "@p5";
                        p5.DbType = DbType.DateTime;
                        p5.Value = DateTime.UtcNow;
                        cmd.Parameters.Add(p5);

                        var returnParameter = cmd.CreateParameter();
                        returnParameter.ParameterName = "@ReturnValue";
                        returnParameter.DbType = DbType.Int32;
                        returnParameter.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(returnParameter);

                        #endregion

                        try
                        {
                            await cmd.ExecuteNonQueryAsync(cancellation);
                            var ret = (Int32?) returnParameter.Value ?? 0;
                            if (ret < 0 || ret > 11) ret = (Int32)MembershipCreateStatus.ProviderError;

                            transaction.Complete();

                            return (MembershipCreateStatus) ret;
                        }

                        catch (SqlException ex)
                        {
                            const Int32 ViolationOfPrimaryKey = 2627;
                            const Int32 CannotInsertDuplicateKey = 2601;
                            const Int32 DuplicateKeys = 2512;

                            if (ex.Number == ViolationOfPrimaryKey || ex.Number == CannotInsertDuplicateKey || ex.Number == DuplicateKeys)
                            {
                                transaction.Complete();
                                return MembershipCreateStatus.DuplicateUserName;
                            }

                            throw;
                        }
                    }
                }
                finally
                {
                    if (this.context.Database.Connection.State == ConnectionState.Open) this.context.Database.Connection.Close();
                }

            }
        }
        
        /// <inheritdoc />
        protected override DateTime LogonAttemptWindow()
        {
            return DateTime.UtcNow.AddMinutes(this.provider.PasswordAttemptWindow * -1);
        }

        /// <summary>
        /// Indicates the minimum required length, in characters, of a password.
        /// </summary>
        public override Int32 MinPasswordLength => this.provider.MinRequiredPasswordLength;

        /// <summary>
        /// Indicates the number of allowed failed logon attempts prior to being locked.
        /// </summary>
        public override Int32 MaxInvalidPasswordAttempts => this.provider.MaxInvalidPasswordAttempts;

        #endregion
    }
}