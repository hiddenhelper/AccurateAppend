using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace DomainModel.Commands
{
    public class LogonMustResetPasswordCommand : ILogonMustResetPasswordCommand
    {
        #region Fields

        private readonly AccurateAppend.Accounting.DataAccess.DefaultContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="LogonMustResetPasswordCommand"/> class.
        /// </summary>
        /// <param name="context">The data context to use for the peration of the command.</param>
        public LogonMustResetPasswordCommand(AccurateAppend.Accounting.DataAccess.DefaultContext context)
        {
            if (context == null) throw new InvalidOperationException(nameof(context));
            Contract.EndContractBlock();

            this.context = context;
        }

        #endregion

        #region ILogonMustResetPasswordCommand Members

        /// <summary>
        /// Performs the work to set a logon to require an updated password when logging on.
        /// </summary>
        /// <param name="id">The user logon identitifer to be set.</param>
        public virtual Task Run(Guid id)
        {
            return this.context.Database.ExecuteSqlCommandAsync("UPDATE dbo.aspnet_Membership SET [MustResetPwd]=1 WHERE UserId = @p0", id);
        }

        #endregion
    }
}
