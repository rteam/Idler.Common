using System;

namespace Idler.Common.Core
{
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// 签入
        /// </summary>
        void Commit();

        /// <summary>
        /// 签入
        /// </summary>
        void CommitAsync();
    }
}