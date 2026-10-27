using System.Collections.Generic;

namespace Idler.Common.Cache
{
    /// <summary>
    /// Redis 集合缓存接口
    /// </summary>
    /// <typeparam name="TCacheScope">
    /// 缓存作用域类型，方法本身不使用该类型，仅用于按类型区分注册实例，使各实例对应各自的缓存配置（过期时间等）
    /// </typeparam>
    public interface IRedisCollectionCache<TCacheScope>
    {
        /// <summary>
        /// 新增或更新有序集合成员
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="score">成员分数</param>
        /// <param name="member">成员值</param>
        /// <param name="region">分区</param>
        /// <returns>实际新增的成员数量，更新已有成员的分数时返回 0</returns>
        long SortedSetAdd(string key, decimal score, string member, string region = "");

        /// <summary>
        /// 按分数范围读取有序集合成员
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="minScore">最小分数</param>
        /// <param name="maxScore">最大分数</param>
        /// <param name="skip">跳过数量</param>
        /// <param name="take">返回数量，小于等于零时不限制</param>
        /// <param name="region">分区</param>
        /// <returns>按分数升序排列的成员，缓存Key不存在时返回空数组</returns>
        string[] SortedSetRangeByScore(string key, decimal minScore, decimal maxScore, int skip = 0, int take = 0, string region = "");

        /// <summary>
        /// 删除有序集合成员
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="member">成员值</param>
        /// <param name="region">分区</param>
        /// <returns>实际删除的成员数量</returns>
        long SortedSetRemove(string key, string member, string region = "");

        /// <summary>
        /// 获取Hash全部字段
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="region">分区</param>
        /// <returns>Hash字段集合，缓存Key不存在时返回空字典</returns>
        Dictionary<string, string> HashGetAll(string key, string region = "");

        /// <summary>
        /// 获取Hash指定字段
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="field">字段</param>
        /// <param name="region">分区</param>
        /// <returns>Hash字段值，字段不存在时返回 null</returns>
        string HashGet(string key, string field, string region = "");

        /// <summary>
        /// 新增或更新Hash字段
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="values">字段值集合</param>
        /// <param name="region">分区</param>
        /// <returns>实际新增的字段数量，更新已有字段时不计入</returns>
        long HashSet(string key, IDictionary<string, string> values, string region = "");

        /// <summary>
        /// 删除Hash字段
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="fields">字段集合</param>
        /// <param name="region">分区</param>
        /// <returns>实际删除的字段数量</returns>
        long HashDelete(string key, IEnumerable<string> fields, string region = "");

        /// <summary>
        /// 将缓存Key中的整数加一，并设置过期时间
        /// </summary>
        /// <remarks>
        /// 缓存配置的过期时间大于零时同时设置过期时间
        /// </remarks>
        /// <param name="key">缓存Key</param>
        /// <param name="region">分区</param>
        /// <returns>递增后的值</returns>
        long Incr(string key, string region = "");
    }
}
