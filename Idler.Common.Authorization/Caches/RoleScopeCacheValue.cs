using System.Collections.Generic;

namespace Idler.Common.Authorization.Caches;

/// <summary>
/// 菜单缓存
/// </summary>
public class RoleScopeCacheValue
{
    /// <summary>
    /// 角色Id
    /// </summary>
    public string RoleId { get; set; }

    /// <summary>
    /// 相关资源列表
    /// </summary>
    public IList<CoreResourceValue> Resources { get; set; }

    /// <summary>
    /// 相关权限列表
    /// </summary>
    public IList<CorePermissionValue> Permissions { get; set; }
}