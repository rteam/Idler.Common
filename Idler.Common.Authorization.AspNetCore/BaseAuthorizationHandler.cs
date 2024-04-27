using System.Security.Claims;
using Idler.Common.Authorization.Services;
using Idler.Common.Core;
using Idler.Common.Core.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using IAuthorizationService = Idler.Common.Authorization.Services.IAuthorizationService;

namespace Idler.Common.Authorization;

/// <summary>
/// 基础授权
/// </summary>
/// <param name="authorizationService">授权服务</param>
public class BaseAuthorizationHandler(
    IAuthorizationService authorizationService)
    : AuthorizationHandler<BaseCoreRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, BaseCoreRequirement requirement)
    {
        AuthorizationFilterContext filterContext = context.Resource as AuthorizationFilterContext;

        if (filterContext == null || context.User.Identity == null || !context.User.Identity.IsAuthenticated)
        {
            context.Fail();
            return Task.CompletedTask;
        }


        string url = this.GetUrl(filterContext);
        string userName = context.User.Identity.Name;

        if (userName == CoreAuthenticationConst.USER_ADMINISTRATOR)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }


        Claim claim = context.User.Claims.FirstOrDefault(t => t.Type == ClaimTypes.NameIdentifier);
        if (claim == null)
            return Task.CompletedTask;

        string userId = claim.Value;

        APIReturnInfo<CoreUserRoleValue> userrole = authorizationService.SingleCoreUserRole(userId);
        IList<string> roleKeys = userrole.Data.RoleKeyList;

        if (roleKeys.Count < 1)
        {
            context.Fail();
            return Task.CompletedTask;
        }

        if (roleKeys.Contains(CoreAuthenticationConst.ROLE_ADMINISTRATORS))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        if (authorizationService.AccessAuthorization(roleKeys, url))
            context.Succeed(requirement);
        else
            context.Fail();

        return Task.CompletedTask;
    }

    /// <summary>
    /// 取得验证用Url
    /// </summary>
    /// <param name="filterContext"></param>
    /// <returns></returns>
    private string GetUrl(AuthorizationFilterContext filterContext) => string.Concat("/",
        filterContext.RouteData.Values["controller"]?.ToString(), "/",
        filterContext.RouteData.Values["action"]?.ToString());
}