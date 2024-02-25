using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Idler.Common.Core.Domain;

namespace Idler.Common.Core.Session
{
    public interface ICoreSession
    {
        /// <summary>
        /// 已认证
        /// </summary>
        bool Authenticated { get; }
        /// <summary>
        /// 用户Id
        /// </summary>
        Guid UserId { get; }
        /// <summary>
        /// 用户名
        /// </summary>
        string UserName { get; }
    }
}