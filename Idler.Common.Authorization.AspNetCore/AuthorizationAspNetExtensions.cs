using System.Security.Claims;
using Idler.Common.Core.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Idler.Common.Authorization;

/// <summary>
/// 授权与认证的扩展方法
/// </summary>
public static class AuthorizationAspNetExtensions
{
    /// <summary>
    /// 获取UserId
    /// </summary>
    /// <param name="controller">上下文</param>
    /// <returns></returns>
    public static string UserId(this ControllerBase controller)
    {
        return controller.FindClaimValue(ClaimTypes.NameIdentifier);
    }

    /// <summary>
    /// 查找UserName
    /// </summary>
    /// <param name="controller">上下文</param>
    /// <returns></returns>
    public static string UserName(this ControllerBase controller)
    {
        return controller.FindClaimValue(ClaimTypes.Name, CoreAuthenticationConst.USER_GUEST);
    }

    /// <summary>
    /// 查找Claim
    /// </summary>
    /// <param name="controller">上下文</param>
    /// <param name="type">ClaimType</param>
    /// <param name="default">如果没找到返回的默认值</param>
    /// <returns></returns>
    public static string FindClaimValue(this ControllerBase controller, string type, string @default = "")
    {
        if (controller.User.Identity != null && !controller.User.Identity.IsAuthenticated)
            return @default;

        Claim claim = controller.User.Claims.FirstOrDefault(t =>
            t.Type.Equals(type, StringComparison.OrdinalIgnoreCase));

        if (claim == null)
            return @default;

        return claim.Value;
    }
}