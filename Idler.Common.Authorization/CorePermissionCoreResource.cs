using Idler.Common.Core.Domain;

namespace Idler.Common.Authorization;

public class CorePermissionCoreResource : EntityWithGuid
{
    public CorePermissionCoreResource() { }

    public CorePermissionCoreResource(string PermissionId, string ResourceId)
    {
        this.PermissionId = PermissionId;
        this.ResourceId = ResourceId;
    }
    /// <summary>
    /// 资源Id
    /// </summary>
    public string ResourceId { get; set; }
    /// <summary>
    /// 权限Id
    /// </summary>
    public string PermissionId { get; set; }
    /// <summary>
    /// 关联权限
    /// </summary>
    public virtual CorePermission Permission { get; set; }
    /// <summary>
    /// 关联资源
    /// </summary>
    public virtual CoreResource Resource { get; set; }
}