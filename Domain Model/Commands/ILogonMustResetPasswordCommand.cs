using System;
using System.Threading.Tasks;

namespace DomainModel.Commands
{
    /// <summary>
    /// Command to require a <see cref="AccurateAppend.Security.User"/> to change password when logging on.
    /// </summary>
    //TODO: Move this to Security
    public interface ILogonMustResetPasswordCommand
    {
        /// <summary>
        /// Performs the work to set a logon to require an updated password when logging on.
        /// </summary>
        /// <param name="id">The user logon identitifer to be set.</param>
        Task Run(Guid id);
    }
}
