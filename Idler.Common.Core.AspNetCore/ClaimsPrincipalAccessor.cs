using System.Security.Claims;
using Idler.Common.Core.Session;
using Microsoft.AspNetCore.Http;

namespace Idler.Common.Core.AspNetCore;

/// <summary>
/// 从HttpContext加载Principal
/// </summary>
public class ClaimsPrincipalAccessor(IHttpContextAccessor HttpContextAccessor) : IPrincipalAccessor
{
    /// <summary>
    /// Principal
    /// </summary>
    public ClaimsPrincipal Principal => HttpContextAccessor.HttpContext.User;
}