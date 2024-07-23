using System;

namespace AccurateAppend.Websites.Admin
{
    public interface IRoleService
    {
        void AddUserToRole(Guid userId, string rolename);

        void RemoveUserFromRole(Guid userId, string rolename);
    }
}