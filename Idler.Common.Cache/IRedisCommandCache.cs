namespace Idler.Common.Cache
{
    /// <summary>
    /// Redis 命令缓存接口
    /// </summary>
    /// <typeparam name="TCacheScope">
    /// 缓存作用域类型，方法本身不使用该类型，仅用于按类型区分注册实例，使各实例对应各自的缓存配置
    /// </typeparam>
    public interface IRedisCommandCache<TCacheScope>
    {
        /// <summary>
        /// 当缓存Key不存在时写入字符串值
        /// </summary>
        /// <remarks>
        /// 缓存配置的过期时间大于零时同时设置过期时间
        /// </remarks>
        /// <param name="key">缓存Key</param>
        /// <param name="value">字符串值</param>
        /// <param name="region">分区</param>
        /// <returns>写入成功返回 true，缓存Key已存在返回 false</returns>
        bool SetNx(string key, string value, string region = "");

        /// <summary>
        /// 将缓存Key中的整数加一
        /// </summary>
        /// <remarks>
        /// 缓存Key不存在时从零开始递增，缓存配置的过期时间大于零时同时设置过期时间
        /// </remarks>
        /// <param name="key">缓存Key</param>
        /// <param name="region">分区</param>
        /// <returns>递增后的值</returns>
        long Incr(string key, string region = "");

        /// <summary>
        /// 将缓存Key中的整数增加指定值
        /// </summary>
        /// <remarks>
        /// 缓存Key不存在时从零开始递增，缓存配置的过期时间大于零时同时设置过期时间
        /// </remarks>
        /// <param name="key">缓存Key</param>
        /// <param name="increment">增加的值，可为负数</param>
        /// <param name="region">分区</param>
        /// <returns>增加后的值</returns>
        long IncrBy(string key, long increment, string region = "");
    }
}
