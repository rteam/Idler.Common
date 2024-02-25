using System;

namespace Idler.Common.Core.Session
{
    public class CoreSession : ICoreSession
    {
        /// <summary>
        /// 已认证
        /// </summary>
        public virtual bool Authenticated => !this.UserId.IsEmpty();

        /// <summary>
        /// 用户Id
        /// </summary>
        public virtual Guid UserId { get; protected set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public virtual string UserName { get; protected set; }


        /// <summary>
        /// 构造函数
        /// </summary>
        public CoreSession()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userId">UserId</param>
        /// <param name="userName">用户名</param>
        public CoreSession(Guid userId, string userName)
        {
            this.UserId = userId;
            this.UserName = userName;
        }
    }
}