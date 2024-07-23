using System;
using System.Diagnostics.Contracts;
using AccurateAppend.Data;
using DomainModel.Commands;

namespace AccurateAppend.Websites.Admin
{
    /// <summary>
    /// Default implementation of <see cref="IRoleService"/> leveraging a <see cref="ISessionContext"/> component.
    /// </summary>
    public class AccountRoleService : IRoleService
    {
        #region Constants
        
        /// <summary>
        /// Contains the name of the ASPNET Role allowing access for the XML services.
        /// </summary>
        public const String XmlRole = "XML User";

        /// <summary>
        /// Contains the name of the ASPNET Role allowing access for the batch services.
        /// </summary>
        public const String BatchRole = "Batch User";

        #endregion

        #region Fields
        
        private readonly IUserToGroupCommand command;

        #endregion

        #region Constructor
        
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountRoleService"/> using the supplied provider.
        /// </summary>
        public AccountRoleService(IUserToGroupCommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            Contract.EndContractBlock();

            this.command = command;
        }

        #endregion

        #region IRoleService Members
        
        public virtual void AddUserToRole(Guid userId, String rolename)
        {
            this.command.Add(userId, rolename).Wait();
        }

        public void RemoveUserFromRole(Guid userId, string rolename)
        {
            this.command.Remove(userId, rolename).Wait();
        }

        #endregion
    }
}