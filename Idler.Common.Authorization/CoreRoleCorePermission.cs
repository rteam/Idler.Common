using Idler.Common.Core.Domain;

namespace Idler.Common.Authorization;

public class CoreRoleCorePermission : EntityWithGuid
{
    public CoreRoleCorePermission() { }

    public CoreRoleCorePermission(string RoleId, string PermissionId)
    {
        this.RoleId = RoleId;
        this.PermissionId = PermissionId;
    }

    /// <summary>
    /// 角色Id
    /// </summary>
    public string RoleId { get; set; }
    /// <summary>
    /// 权限Id
    /// </summary>
    public string PermissionId { get; set; }
    /// <summary>
    /// 关联角色
    /// </summary>
    public virtual CoreRole Role { get; set; }
    /// <summary>
    /// 关联权限
    /// </summary>
    public virtual CorePermission Permission { get; set; }
}