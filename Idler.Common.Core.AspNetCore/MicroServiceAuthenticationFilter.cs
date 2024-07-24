using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using Idler.Common.Core.Authentication;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Idler.Common.Core.AspNetCore;

/// <summary>
/// 内部服务加载用户信息
/// </summary>
public class MicroServiceAuthenticationFilter : IActionFilter
{
    public const string AUTHENTICATION_TYPE = "MICRO_SERVICE";
    
    public void OnActionExecuted(ActionExecutedContext context)
    {
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        string UserName = CoreAuthenticationConst.USER_GUEST;
        string UserId = string.Empty;

        if (context.HttpContext.Request.Headers.TryGetValue(CoreClaimTypes.USER_ID, out var UserIds))
            UserId = UserIds.Count > 0 ? UserIds.FirstOrDefault() : string.Empty;

        if (UserId.IsEmpty())
            return;

        if (context.HttpContext.Request.Headers.TryGetValue(CoreClaimTypes.USER_NAME, out var UserNames))
            UserName = UserNames.Count > 0 ? UserNames.FirstOrDefault() : CoreAuthenticationConst.USER_GUEST;

        ClaimsIdentity claimsIdentity = new ClaimsIdentity(AUTHENTICATION_TYPE, ClaimTypes.Name, ClaimTypes.Role);
        claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, UserName));
        claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, UserId.ToString()));
        context.HttpContext.User = new ClaimsPrincipal(claimsIdentity);
        Thread.CurrentPrincipal = context.HttpContext.User ;
    }
}