namespace Idler.Common.Core
{
    public abstract class BaseDomainService : IDomainService
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="UnitOfWork"></param>
        public BaseDomainService(IUnitOfWork UnitOfWork)
        {
            this.UnitOfWork = UnitOfWork;
        }

        /// <summary>
        /// 状态器
        /// </summary>
        protected readonly IUnitOfWork UnitOfWork;

        #region IDomainService 成员

        /// <summary>
        /// 保存更改
        /// </summary>
        public void SaveChange()
        {
            this.UnitOfWork.Commit();
        }

        /// <summary>
        /// 保存更改
        /// </summary>
        public void SaveChangeAsync()
        {
            this.UnitOfWork.CommitAsync();
        }

        #endregion
    }
}