using System.Collections.Generic;

namespace Idler.Common.Authorization;

public class CoreResource : CoreResourceValue
{
    public CoreResource()
    {
    }

    public CoreResource(string Id, string Name, string Url, string Description = "")
        : base(Id, Name, Url, Description)
    {

    }

    /// <summary>
    /// 子资源
    /// </summary>
    public virtual ICollection<CoreResource> Children { get; set; } = new List<CoreResource>();

    /// <summary>
    /// 父节点
    /// </summary>
    public virtual CoreResource Parent { get; set; }

    /// <summary>
    /// 权限与资源关联关系
    /// </summary>
    public virtual ICollection<CorePermissionCoreResource> PermissionResources { get; set; } =
        new List<CorePermissionCoreResource>();
}