using System.Collections.Generic;
using Idler.Common.Core.Config;

namespace Idler.Common.Authorization.Config;

/// <summary>
/// 认证基础数据
/// </summary>
public class AuthorizationBasicData : AutoBaseConfig<AuthorizationBasicData>
{
    /// <summary>
    /// 资源
    /// </summary>
    public IList<CoreResourceValue> Resources { get; set; } = new List<CoreResourceValue>();

    /// <summary>
    /// 权限
    /// </summary>
    public IList<CorePermissionValue> Permissions { get; set; } = new List<CorePermissionValue>();

    /// <summary>
    /// 角色
    /// </summary>
    public IList<CoreRoleValue> Roles { get; set; } = new List<CoreRoleValue>();
}