using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Idler.Common.Core.Authentication;

namespace Idler.Common.Core.Session
{
    public static class CoreSessionExtensions
    {
        /// <summary>
        /// 获取用户名
        /// </summary>
        /// <param name="coreSession"></param>
        /// <returns></returns>
        public static string GetUserNameOrGuest(this ICoreSession coreSession)
        {
            if (coreSession == null || !coreSession.Authenticated || coreSession.UserName.IsEmpty())
                return CoreAuthenticationConst.USER_GUEST;

            return coreSession.UserName;
        }
    }
}