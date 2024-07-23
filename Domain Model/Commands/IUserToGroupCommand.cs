using System;
using System.Threading.Tasks;

namespace DomainModel.Commands
{
    /// <summary>
    /// Represents a command that 
    /// </summary>
    public interface IUserToGroupCommand
    {
        /// <summary>
        /// Adds a user logon to the indicated group.
        /// </summary>
        /// <param name="userId">The identifier of the user to add.</param>
        /// <param name="groupName">The name of the group to be added to.</param>
        Task Add(Guid userId, String groupName);

        /// <summary>
        /// Removes a user logon from the indicated group.
        /// </summary>
        /// <param name="userId">The identifier of the user to remove.</param>
        /// <param name="groupName">The name of the group to be removed from.</param>
        Task Remove(Guid userId, String groupName);
    }
}
