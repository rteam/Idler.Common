using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Idler.Common.Core.Authentication;
using Idler.Common.Core.Session;
using WebApiClientCore;
using WebApiClientCore.Attributes;

namespace Idler.Common.WebAPIClient;

/// <summary>
/// 附加用户信息到请求
/// </summary>
public class AppendUserFilterAttribute : ApiFilterAttribute
{
    public override Task OnRequestAsync(ApiRequestContext context)
    {
        IPrincipalAccessor principalAccessor =
            (IPrincipalAccessor)context.HttpContext.ServiceProvider.GetService(typeof(IPrincipalAccessor));

        if (principalAccessor == null || principalAccessor.Principal == null ||
            principalAccessor.Principal.Identity == null ||
            !principalAccessor.Principal.Identity.IsAuthenticated)
            return Task.CompletedTask;

        var userIdClaim =
            principalAccessor.Principal.Claims.FirstOrDefault(t => t.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim == null || userIdClaim.Value.IsEmpty())
            return Task.CompletedTask;

        context.HttpContext.RequestMessage.Headers.Add(CoreClaimTypes.USER_ID, userIdClaim.Value);

        var userNameClaim = principalAccessor.Principal.Claims.FirstOrDefault(t => t.Type == ClaimTypes.Name);
        if (userNameClaim == null || userNameClaim.Value.IsEmpty())
            return Task.CompletedTask;

        context.HttpContext.RequestMessage.Headers.Add(CoreClaimTypes.USER_NAME, userNameClaim.Value);

        return Task.CompletedTask;
    }

    public override async Task OnResponseAsync(ApiResponseContext context)
    {
    }
}