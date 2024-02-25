namespace Idler.Common.Core
{
    public interface IDomainService
    {
        /// <summary>
        /// 保存更改
        /// </summary>
        void SaveChange();

        /// <summary>
        /// 保存更改
        /// </summary>
        void SaveChangeAsync();
    }
}