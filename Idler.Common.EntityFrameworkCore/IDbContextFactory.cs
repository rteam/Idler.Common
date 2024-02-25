namespace Idler.Common.EntityFrameworkCore
{
    /// <summary>
    /// 数据库上下文工厂接口
    /// </summary>
    public interface IDbContextFactory : IDisposable
    {
        /// <summary>
        /// 获取数据库连接（每次新建）
        /// </summary>
        /// <returns></returns>
        CoreDBContext Get();
        /// <summary>
        /// 获取数据库实例
        /// </summary>
        /// <returns></returns>
        CoreDBContext Instance();
    }

}