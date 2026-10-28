using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using FreeRedis;

namespace Idler.Common.Cache.FreeRedis
{
    public class SimpleRedisCacheManager<TCacheValue> : ISimpleCacheManager<TCacheValue>, IRedisCollectionCache<TCacheValue>, IRedisStringCache<TCacheValue>
    {
        /// <summary>
        /// 成功标志
        /// </summary>
        protected static string SUCCESS_FLAG = "OK";

        /// <summary>
        /// 组装缓存Key
        /// </summary>
        /// <param name="cachePrefix">缓存前缀</param>
        /// <param name="params">参数</param>
        /// <returns></returns>
        public static string AssembleCacheKeyMD5(string cachePrefix, params string[] @params)
        {
            return InternalAssembleCacheKey(cachePrefix, @params).MD5();
        }

        /// <summary>
        /// 组装缓存Key
        /// </summary>
        /// <param name="cachePrefix">缓存前缀</param>
        /// <param name="params">参数</param>
        /// <returns></returns>
        public static string AssembleCacheKeySHA1(string cachePrefix, params string[] @params)
        {
            return InternalAssembleCacheKey(cachePrefix, @params).SHA1();
        }

        private static string InternalAssembleCacheKey(string cachePrefix, params string[] @params)
        {
            if (@params is not { Length: > 1 })
                return cachePrefix;

            return string.Concat(cachePrefix, string.Join("_", @params.OrderByDescending(t => t)));
        }

        public SimpleRedisCacheManager(string settingKey, FreeRedisService freeRedisService)
        {
            this.SettingKey = settingKey;
            this.FreeRedisService = freeRedisService;
            this.Option = this.FreeRedisService.Option;
            this.Setting = this.Option.Settings?.SingleOrDefault(t =>
                t.Key.Equals(settingKey, StringComparison.OrdinalIgnoreCase))
                ?? throw new InvalidOperationException($"未找到缓存配置 {settingKey}");
        }

        protected readonly FreeRedisService FreeRedisService;

        /// <summary>
        /// 设置
        /// </summary>
        protected SimpleCacheSetting Setting { get; set; }

        protected string SettingKey { get; set; }

        public virtual FreeRedisCacheOption Option { get; protected set; }

        protected RedisClient Client => this.FreeRedisService.Instance;

        /// <summary>
        /// 获取指定缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">返回的缓存值</param>
        /// <param name="region">分区</param>
        /// <returns></returns>
        public virtual bool TryGet(string key, out TCacheValue value, string region = "")
        {
            key.ThrowIfNull(nameof(key));

            string cacheKey = this.AssembleCacheKey(key, region);

            value = this.Client.Get<TCacheValue>(cacheKey);
            if (value != null)
                return true;

            value = default(TCacheValue);
            return false;
        }

        /// <summary>
        /// 添加或更新
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="cacheValue">要缓存的对象</param>
        /// <param name="region">分区</param>
        /// <returns></returns>
        public virtual bool Set(string key, TCacheValue cacheValue, string region)
        {
            key.ThrowIfNull(nameof(key));

            if (cacheValue == null)
                throw new ArgumentNullException(nameof(cacheValue));

            string cacheKey = this.AssembleCacheKey(key, region);

            return this.AddOrUpdate(cacheKey, cacheValue, this.Setting.ExpiresTime);
        }


        /// <summary>
        /// 缓存是否存在
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="region">分区</param>
        /// <returns></returns>
        public virtual bool Exist(string key, string region)
        {
            string cacheKey = this.AssembleCacheKey(key, region);

            return this.Client.Exists(cacheKey);
        }

        /// <summary>
        /// 删除指定缓存项
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="region">分区</param>
        /// <returns></returns>
        public virtual bool Remove(string key, string region)
        {
            string cacheKey = this.AssembleCacheKey(key, region);

            return this.Client.Del(cacheKey) > 0 ? true : false;
        }

        /// <summary>
        /// 删除指定区域内的全部缓存
        /// </summary>
        /// <param name="region">分区</param>
        /// <returns></returns>
        public virtual bool RemoveRegion(string region)
        {
            long removeCount = 0;

            ScanRemoves(string.Concat(this.Option.Prefix, region, "_", "*"), 0);

            void ScanRemoves(string pattern, long cursor)
            {
                var result = this.Client.Scan(cursor, pattern, 10, "");

                foreach (var item in result.items)
                    removeCount += this.Client.Del(item);

                if (result.cursor > 0)
                    ScanRemoves(pattern, result.cursor);
            }

            return removeCount > 0;
        }

        /// <summary>
        /// 新增或更新有序集合成员
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="score">成员分数</param>
        /// <param name="member">成员值</param>
        /// <param name="region">分区</param>
        /// <returns>实际新增的成员数量，更新已有成员的分数时返回 0</returns>
        public virtual long SortedSetAdd(string key, decimal score, string member, string region)
        {
            key.ThrowIfNull(nameof(key));
            member.ThrowIfNull(nameof(member));

            string cacheKey = this.AssembleCacheKey(key, region);
            int expiresTime = this.Setting.ExpiresTime;
            if (expiresTime <= 0)
                return this.Client.ZAdd(cacheKey, score, member);

            using (RedisClient.TransactionHook transaction = this.Client.Multi())
            {
                transaction.ZAdd(cacheKey, score, member);
                transaction.Expire(cacheKey, expiresTime);

                return Convert.ToInt64(transaction.Exec()[0]);
            }
        }

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
        public virtual string[] SortedSetRangeByScore(string key, decimal minScore, decimal maxScore, int skip, int take, string region)
        {
            key.ThrowIfNull(nameof(key));

            var (offset, count) = ResolveRangeLimit(skip, take);
            string cacheKey = this.AssembleCacheKey(key, region);
            return this.Client.ZRangeByScore(cacheKey, minScore, maxScore, offset, count);
        }

        /// <summary>
        /// 将 skip / take 转换为 FreeRedis ZRangeByScore 的 offset / count
        /// </summary>
        /// <remarks>
        /// FreeRedis 仅在 offset &gt; 0 或 count &gt; 0 时附加 LIMIT，
        /// count 为 -1 时 Redis 返回 offset 之后的全部成员
        /// </remarks>
        /// <param name="skip">跳过数量</param>
        /// <param name="take">返回数量，小于等于零时不限制</param>
        /// <returns>offset 与 count</returns>
        internal static (int Offset, int Count) ResolveRangeLimit(int skip, int take)
        {
            if (skip < 0)
                throw new ArgumentOutOfRangeException(nameof(skip));

            return (skip, take > 0 ? take : -1);
        }

        /// <summary>
        /// 删除有序集合成员
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="member">成员值</param>
        /// <param name="region">分区</param>
        /// <returns>实际删除的成员数量</returns>
        public virtual long SortedSetRemove(string key, string member, string region)
        {
            key.ThrowIfNull(nameof(key));
            member.ThrowIfNull(nameof(member));

            string cacheKey = this.AssembleCacheKey(key, region);
            return this.Client.ZRem(cacheKey, member);
        }

        /// <summary>
        /// 获取Hash全部字段
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="region">分区</param>
        /// <returns>Hash字段集合，缓存Key不存在时返回空字典</returns>
        public virtual Dictionary<string, string> HashGetAll(string key, string region)
        {
            key.ThrowIfNull(nameof(key));

            string cacheKey = this.AssembleCacheKey(key, region);
            return this.Client.HGetAll(cacheKey);
        }

        /// <summary>
        /// 获取Hash指定字段
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="field">字段</param>
        /// <param name="region">分区</param>
        /// <returns>Hash字段值，字段不存在时返回 null</returns>
        public virtual string HashGet(string key, string field, string region)
        {
            key.ThrowIfNull(nameof(key));
            field.ThrowIfNull(nameof(field));

            string cacheKey = this.AssembleCacheKey(key, region);
            return this.Client.HGet(cacheKey, field);
        }

        /// <summary>
        /// 新增或更新Hash字段
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="values">字段值集合</param>
        /// <param name="region">分区</param>
        /// <returns>实际新增的字段数量，更新已有字段时不计入</returns>
        public virtual long HashSet(string key, IDictionary<string, string> values, string region)
        {
            key.ThrowIfNull(nameof(key));
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            if (values.Count == 0)
                return 0;

            string cacheKey = this.AssembleCacheKey(key, region);
            var fieldValues = new Dictionary<string, string>(values);
            int expiresTime = this.Setting.ExpiresTime;
            if (expiresTime <= 0)
                return this.Client.HSet(cacheKey, fieldValues);

            using (RedisClient.TransactionHook transaction = this.Client.Multi())
            {
                transaction.HSet(cacheKey, fieldValues);
                transaction.Expire(cacheKey, expiresTime);

                return Convert.ToInt64(transaction.Exec()[0]);
            }
        }

        /// <summary>
        /// 删除Hash字段
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="fields">字段集合</param>
        /// <param name="region">分区</param>
        /// <returns>实际删除的字段数量</returns>
        public virtual long HashDelete(string key, IEnumerable<string> fields, string region)
        {
            key.ThrowIfNull(nameof(key));
            if (fields == null)
                throw new ArgumentNullException(nameof(fields));

            string[] fieldArray = fields.ToArray();
            if (fieldArray.Length == 0)
                return 0;
            if (fieldArray.Any(t => t == null))
                throw new ArgumentException("字段集合不能包含空值", nameof(fields));

            string cacheKey = this.AssembleCacheKey(key, region);
            return this.Client.HDel(cacheKey, fieldArray);
        }

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
        public virtual bool SetNx(string key, string value, string region)
        {
            key.ThrowIfNull(nameof(key));
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            string cacheKey = this.AssembleCacheKey(key, region);
            int expiresTime = this.Setting.ExpiresTime;
            if (expiresTime <= 0)
                return this.Client.SetNx(cacheKey, value);

            return this.Client.SetNx(cacheKey, value, expiresTime);
        }

        /// <summary>
        /// 将缓存Key中的整数增加指定值
        /// </summary>
        /// <remarks>
        /// 缓存Key不存在时从零开始递增，不设置过期时间
        /// </remarks>
        /// <param name="key">缓存Key</param>
        /// <param name="increment">增加的值，可为负数</param>
        /// <param name="region">分区</param>
        /// <returns>增加后的值</returns>
        public virtual long IncrBy(string key, long increment, string region)
        {
            key.ThrowIfNull(nameof(key));

            string cacheKey = this.AssembleCacheKey(key, region);
            return this.Client.IncrBy(cacheKey, increment);
        }

        /// <summary>
        /// 获取或添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="valueFactory">缓存对象工厂</param>
        /// <param name="region">分区</param>
        /// <returns></returns>
        public virtual async Task<TCacheValue> GetOrAddAsync(string key, Func<string, Task<TCacheValue>> valueFactory,
            string region)
        {
            key.ThrowIfNull(nameof(key));

            if (valueFactory == null)
                throw new ArgumentNullException(nameof(valueFactory));

            string cacheKey = this.AssembleCacheKey(key, region);

            TCacheValue value = await this.Client.GetAsync<TCacheValue>(cacheKey).ConfigureAwait(false);
            if (value != null)
                return value;

            value = await valueFactory(key).ConfigureAwait(false);
            if (value == null)
                return default(TCacheValue);

            if (await this.AddOrUpdateAsync(cacheKey, value, this.Setting.ExpiresTime).ConfigureAwait(false))
                return value;

            return default(TCacheValue);
        }

        /// <summary>
        /// 获取或添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="valueFactory">缓存对象工厂</param>
        /// <param name="value">取出的缓存值</param>
        /// <param name="region">分区</param>
        /// <returns></returns>
        public virtual bool TryGetOrAdd(string key, Func<string, TCacheValue> valueFactory, out TCacheValue value,
            string region)
        {
            key.ThrowIfNull(nameof(key));

            if (valueFactory == null)
                throw new ArgumentNullException(nameof(valueFactory));

            string cacheKey = this.AssembleCacheKey(key, region);

            value = this.Client.Get<TCacheValue>(cacheKey);
            if (value != null)
                return true;

            value = valueFactory(cacheKey);
            if (value == null)
                return false;

            if (this.AddOrUpdate(cacheKey, value, this.Setting.ExpiresTime))
                return true;

            value = default(TCacheValue);
            return false;
        }

        /// <summary>
        /// 添加或更新
        /// </summary>
        /// <param name="key"></param>
        /// <param name="addFactory"></param>
        /// <param name="cacheValue">缓存值</param>
        /// <param name="region">分区</param>
        /// <returns></returns>
        public virtual bool TrySet(string key, Func<string, TCacheValue> addFactory,
            out TCacheValue cacheValue, string region)
        {
            key.ThrowIfNull(nameof(key));

            if (addFactory == null)
                throw new ArgumentNullException(nameof(addFactory));

            string cacheKey =
                this.AssembleCacheKey(key, region);

            cacheValue = addFactory(cacheKey);
            if (cacheValue == null)
                cacheValue = default(TCacheValue);

            if (this.AddOrUpdate(cacheKey, cacheValue, this.Setting.ExpiresTime))
            {
                cacheValue = default(TCacheValue);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 添加或更新（异步）
        /// </summary>
        /// <param name="key"></param>
        /// <param name="addFactory"></param>
        /// <param name="region">分区</param>
        /// <returns></returns>
        public virtual async Task<TCacheValue> SetAsync(string key, Func<string, Task<TCacheValue>> addFactory,
            string region)
        {
            key.ThrowIfNull(nameof(key));

            if (addFactory == null)
                throw new ArgumentNullException(nameof(addFactory));

            string cacheKey =
                this.AssembleCacheKey(key, region);

            TCacheValue cacheValue = await addFactory(cacheKey).ConfigureAwait(false);
            if (cacheValue == null)
                return default(TCacheValue);

            if (await this.AddOrUpdateAsync(cacheKey, cacheValue, this.Setting.ExpiresTime).ConfigureAwait(false))
                return cacheValue;

            return default(TCacheValue);
        }

        /// <summary>
        /// 组装缓存键
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="region">分区</param>
        /// <returns></returns>
        protected virtual string AssembleCacheKey(string key, string region)
        {
            return string.Concat(this.Option.Prefix, region.IsEmpty() ? key : string.Concat(region, "_", key));
        }

        /// <summary>
        /// 添加或更新
        /// </summary>
        /// <param name="cacheKey">缓存Key</param>
        /// <param name="cacheValue">缓存值</param>
        /// <param name="second">过期时间</param>
        /// <returns></returns>
        protected virtual bool AddOrUpdate(string cacheKey, TCacheValue cacheValue, int second)
        {
            return this.Client.Set(cacheKey, cacheValue, TimeSpan.FromSeconds((double)second), false,
                false, false, false) == SUCCESS_FLAG;
        }

        /// <summary>
        /// 添加或更新
        /// </summary>
        /// <param name="cacheKey">缓存Key</param>
        /// <param name="cacheValue">缓存值</param>
        /// <param name="second">过期时间</param>
        /// <returns></returns>
        protected virtual async Task<bool> AddOrUpdateAsync(string cacheKey, TCacheValue cacheValue, int second)
        {
            return await this.Client.SetAsync(cacheKey, cacheValue, TimeSpan.FromSeconds((double)second), false,
                false, false, false).ConfigureAwait(false) == SUCCESS_FLAG;
        }
    }
}
