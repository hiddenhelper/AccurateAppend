using System;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace DomainModel.Commands
{
    /// <summary>
    /// Represents a command that 
    /// </summary>
    public class UserToGroupCommand : IUserToGroupCommand
    {
        #region Fields

        private readonly DbContext context;

        #endregion

        #region Constructor

        public UserToGroupCommand(DbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            Contract.EndContractBlock();

            this.context = context;
        }

        #endregion

        #region IUserToGroupCommand Members

        /// <summary>
        /// Adds a user logon to the indicated group.
        /// </summary>
        /// <param name="userId">The identifier of the user to add.</param>
        /// <param name="groupName">The name of the group to be added to.</param>
        public virtual Task Add(Guid userId, String groupName)
        {
            const String Sql = @"
delete from dbo.aspnet_UsersInRoles where UserId=@p0 and RoleId=(select RoleId from dbo.aspnet_Roles r inner join aspnet_users u on r.ApplicationId=u.ApplicationId where r.RoleName=@p1 and u.UserId=@p0)

insert into dbo.aspnet_UsersInRoles
select @p0, RoleId from dbo.aspnet_Roles r inner join aspnet_users u on r.ApplicationId=u.ApplicationId where RoleName=@p1 and u.UserId=@p0
";
            return this.context.Database.ExecuteSqlCommandAsync(Sql, userId, groupName);
        }

        /// <summary>
        /// Removes a user logon from the indicated group.
        /// </summary>
        /// <param name="userId">The identifier of the user to remove.</param>
        /// <param name="groupName">The name of the group to be removed from.</param>
        public virtual Task Remove(Guid userId, String groupName)
        {
            const String Sql = @"delete from dbo.aspnet_UsersInRoles where UserId=@p0 and RoleId=(select RoleId from dbo.aspnet_Roles r inner join aspnet_users u on r.ApplicationId=u.ApplicationId where r.RoleName=@p1 and u.UserId=@p0)";

            return this.context.Database.ExecuteSqlCommandAsync(Sql, userId, groupName);
        }

        #endregion
    }
}
