using Microsoft.EntityFrameworkCore;

namespace Idler.Common.Authorization.EntityFrameworkCore
{
    /// <summary>
    /// 授权上下文
    /// </summary>
    public interface IAuthorizationDBContext
    {
        /// <summary>
        /// 权限
        /// </summary>
        DbSet<CorePermission> Permissions { get; set; }

        /// <summary>
        /// 资源
        /// </summary>
        DbSet<CoreResource> Resources { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        DbSet<CoreRole> Roles { get; set; }

        /// <summary>
        /// 用户与用户组关联关系
        /// </summary>
        DbSet<CoreUserRole> UserRoles { get; set; }

        /// <summary>
        /// 权限与资源关联关系
        /// </summary>
        DbSet<CorePermissionCoreResource> PermissionResources { get; set; }

        /// <summary>
        /// 角色与权限关联关系
        /// </summary>
        DbSet<CoreRoleCorePermission> RolePermissions { get; set; }
    }
}