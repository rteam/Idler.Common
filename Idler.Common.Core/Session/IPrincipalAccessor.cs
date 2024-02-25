using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Idler.Common.Core.Session
{
    /// <summary>
    /// Principal访问器
    /// </summary>
    public interface IPrincipalAccessor
    {
        /// <summary>
        /// Principal
        /// </summary>
        ClaimsPrincipal Principal { get; }
    }
}