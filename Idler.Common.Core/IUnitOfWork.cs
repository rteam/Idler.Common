using System;
using System.Threading.Tasks;

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
        Task CommitAsync();
    }
}