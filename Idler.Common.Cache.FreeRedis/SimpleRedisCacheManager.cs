using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using FreeRedis;

namespace Idler.Common.Cache.FreeRedis
{
    public class SimpleRedisCacheManager<TCacheValue> : ISimpleCacheManager<TCacheValue>
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
            this.Setting =
                this.Option.Settings.SingleOrDefault(t =>
                    t.Key.Equals(settingKey, StringComparison.OrdinalIgnoreCase));
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
                    removeCount += removeCount + this.Client.Del(item);

                if (result.cursor > 0)
                    ScanRemoves(pattern, result.cursor);
            }

            return removeCount > 0;
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