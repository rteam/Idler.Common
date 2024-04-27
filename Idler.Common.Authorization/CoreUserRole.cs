using System;

namespace Idler.Common.Authorization;

public class CoreUserRole : CoreUserRoleValue
{
    public CoreUserRole()
    {
    }

    public CoreUserRole(string userId, string name, string roleKeys)
    {
        this.Id = userId;
        this.UserId = userId;
        this.RoleKeys = roleKeys;
        this.Name = name;
    }

    public CoreUserRole(Guid userId, string name, string roleKeys)
        : this(userId.ToString(), name, roleKeys)
    {
    }
}