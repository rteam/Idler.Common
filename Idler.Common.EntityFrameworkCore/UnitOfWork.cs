using Idler.Common.Core;
using Idler.Common.Core.Session;

namespace Idler.Common.EntityFrameworkCore
{
    public class UnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="databaseFactory"></param>
        public UnitOfWork(IDbContextFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }

        #region IUnitOfWork 成员

        /// <summary>
        /// 签入修改
        /// </summary>
        public void Commit()
        {
            try
            {
                this.DataContext.SaveChanges();
            }
            catch (System.Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 签入修改
        /// </summary>
        public async Task CommitAsync()
        {
            try
            {
                await this.DataContext.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (System.Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            this.DataContext.Dispose();
        }

        #endregion

        private CoreDBContext dBContext;

        /// <summary>
        /// 数据上下文引用
        /// </summary>
        protected CoreDBContext DataContext
        {
            get
            {
                if (this.dBContext == null)
                    this.dBContext = this.databaseFactory.Instance();

                return this.dBContext;
            }
        }

        /// <summary>
        /// 数据库工厂
        /// </summary>
        private readonly IDbContextFactory databaseFactory;
    }
}