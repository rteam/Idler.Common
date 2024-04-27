using System.Collections.Generic;

namespace Idler.Common.Authorization;

public class CorePermission : CorePermissionValue
{
    /// <summary>
    /// 子节点
    /// </summary>
    public virtual ICollection<CorePermission> Children { get; set; } = new List<CorePermission>();
    /// <summary>
    /// 父节点
    /// </summary>
    public virtual CorePermission Parent { get; set; }
    /// <summary>
    /// 权限与资源关联关系
    /// </summary>
    public virtual ICollection<CorePermissionCoreResource> PermissionResources { get; set; } = new List<CorePermissionCoreResource>();
    /// <summary>
    /// 资源
    /// </summary>
    public virtual ICollection<CoreResource> Resources { get; set; } = new List<CoreResource>();
}