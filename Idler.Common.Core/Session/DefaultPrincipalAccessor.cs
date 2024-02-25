using System.Security.Claims;
using System.Threading;

namespace Idler.Common.Core.Session
{
    /// <summary>
    /// 默认Principal访问器
    /// </summary>
    public class DefaultPrincipalAccessor : IPrincipalAccessor
    {
        public ClaimsPrincipal Principal => Thread.CurrentPrincipal as ClaimsPrincipal;
    }
}