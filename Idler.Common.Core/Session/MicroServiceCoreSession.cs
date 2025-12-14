using Idler.Common.Core.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Idler.Common.Core.Session
{
    public class MicroServiceCoreSession : CoreSession
    {
        private readonly IPrincipalAccessor _principalAccessor;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="principalAccessor"></param>
        public MicroServiceCoreSession(IPrincipalAccessor principalAccessor)
        {
            _principalAccessor = principalAccessor ?? new DefaultPrincipalAccessor();
        }

        /// <summary>
        /// 已认证
        /// </summary>
        public override bool Authenticated => !UserIdentifier.IsEmpty();

        /// <summary>
        /// 用户身份Id
        /// </summary>
        public override string UserIdentifier
        {
            get
            {
                var claim = _principalAccessor.Principal?.Claims
                    .FirstOrDefault(t => t.Type == ClaimTypes.NameIdentifier);

                if (claim == null || claim.Value.IsEmpty())
                    return string.Empty;

                return claim.Value;
            }
        }

        /// <summary>
        /// 用户Id
        /// </summary>
        public override Guid UserId
        {
            get
            {
                if (UserIdentifier == null || UserIdentifier.IsEmpty())
                    return Guid.Empty;

                return UserIdentifier.IsGuid() ? UserIdentifier.GuidByString() : Guid.Empty;
            }
        }

        /// <summary>
        /// 用户名
        /// </summary>
        public override string UserName
        {
            get
            {
                var claim = _principalAccessor.Principal?.Claims
                    .FirstOrDefault(t => t.Type == ClaimTypes.Name);

                if (claim == null || claim.Value.IsEmpty())
                    return CoreAuthenticationConst.USER_GUEST;

                return claim.Value;
            }
        }
    }

}
