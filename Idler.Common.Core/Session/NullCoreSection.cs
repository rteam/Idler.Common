using System;
using Idler.Common.Core.Authentication;

namespace Idler.Common.Core.Session
{
    public class NullCoreSection : ICoreSession
    {
        public bool Authenticated => false;

        public Guid UserId => Guid.Empty;

        public string UserName => CoreAuthenticationConst.USER_GUEST;

    }
}