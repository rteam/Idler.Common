using System.Collections.Generic;

namespace Idler.Common.Authorization;

public class CoreRole : CoreRoleValue
{
    /// <summary>
    /// 权限
    /// </summary>
    public virtual ICollection<CorePermission> Permissions { get; set; } = new List<CorePermission>();
}