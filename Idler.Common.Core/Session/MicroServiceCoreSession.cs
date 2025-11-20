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

        public MicroServiceCoreSession(IPrincipalAccessor principalAccessor)
        {
            _principalAccessor = principalAccessor ?? new DefaultPrincipalAccessor();
        }

        public override Guid UserId
        {
            get
            {
                var claim = _principalAccessor.Principal?.Claims
                    .FirstOrDefault(t => t.Type == ClaimTypes.NameIdentifier);

                if (claim == null || claim.Value.IsEmpty())
                    return Guid.Empty;

                return claim.Value.IsGuid() ? claim.Value.GuidByString() : Guid.Empty;
            }
        }

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
