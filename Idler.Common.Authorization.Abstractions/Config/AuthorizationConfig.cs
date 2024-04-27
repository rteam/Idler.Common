using System.Collections.Generic;
using Idler.Common.Core.Config;

namespace Idler.Common.Authorization.Config;

/// <summary>
/// 认证配置
/// </summary>
public class AuthorizationConfig : AutoBaseConfig<AuthorizationConfig>
{
    /// <summary>
    /// 缓存前缀
    /// </summary>
    public string CachePrefix { get; set; } = "Authorization_";

    /// <summary>
    /// 排除路径
    /// </summary>
    public IList<string> ExcludePaths { get; set; } = new List<string>();
}