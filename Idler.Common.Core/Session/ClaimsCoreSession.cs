using System;
using System.Linq;
using Idler.Common.Core.Authentication;

namespace Idler.Common.Core.Session
{
    public class ClaimsCoreSession : CoreSession
    {
        /// <summary>
        /// UserId
        /// </summary>
        public override Guid UserId
        {
            get
            {
                var userIdClaim =
                    PrincipalAccessor.Principal?.Claims.FirstOrDefault(t => t.Type == CoreClaimTypes.USER_ID);
                if (userIdClaim == null || userIdClaim.Value.IsEmpty())
                    return Guid.Empty;

                return userIdClaim.Value.IsGuid() ? userIdClaim.Value.GuidByString() : Guid.Empty;
            }
        }

        /// <summary>
        /// 用户名
        /// </summary>
        public override string UserName
        {
            get
            {
                var userNameClaim =
                    PrincipalAccessor.Principal?.Claims.FirstOrDefault(t => t.Type == CoreClaimTypes.USER_NAME);
                if (userNameClaim == null || userNameClaim.Value.IsEmpty())
                    return CoreAuthenticationConst.USER_GUEST;

                return userNameClaim.Value;
            }
        }

        /// <summary>
        /// PrincipalAccessor
        /// </summary>
        protected IPrincipalAccessor PrincipalAccessor { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="principalAccessor"></param>
        public ClaimsCoreSession(
            IPrincipalAccessor principalAccessor)
        {
            this.PrincipalAccessor = principalAccessor ?? new DefaultPrincipalAccessor();
        }
    }
}