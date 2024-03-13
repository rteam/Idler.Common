using System.Collections.Generic;
using System.Security.Claims;
using Idler.Common.Core.Domain;

namespace Idler.Common.Core.Authentication;

/// <summary>
/// 用户标识符
/// </summary>
public class UserIdentifier : EntityWithStringNoTrace, IUserIdentifier
{
    /// <summary>
    /// 用户名称
    /// 顺序为：昵称、用户名、Email
    /// 不作为唯一标识符
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Claims
    /// </summary>
    public ICollection<Claim> Claims { get; set; } = new List<Claim>();
}