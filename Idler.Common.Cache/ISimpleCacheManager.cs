using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Idler.Common.Cache
{
    public interface ISimpleCacheManager<TCacheValue>
    {
        /// <summary>
        /// 缓存是否存在
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="region">分区</param>
        /// <returns></returns>
        bool Exist(string key, string region = "");
        /// <summary>
        /// 删除指定缓存项
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="region">分区</param>
        /// <returns></returns>
        bool Remove(string key, string region = "");
        /// <summary>
        /// 删除指定分区内的全部缓存
        /// </summary>
        /// <param name="region">分区</param>
        /// <returns></returns>
        bool RemoveRegion(string region);
        /// <summary>
        /// 添加或更新
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="cacheValue">要缓存的对象</param>
        /// <param name="region">分区</param>
        /// <returns></returns>
        bool Set(string key, TCacheValue cacheValue, string region = "");
        /// <summary>
        /// 添加或更新
        /// </summary>
        /// <param name="key"></param>
        /// <param name="addFactory"></param>
        /// <param name="cacheValue">缓存结果</param>
        /// <param name="region">分区</param>
        /// <returns></returns>
        bool TrySet(string key, Func<string, TCacheValue> addFactory, out TCacheValue cacheValue, string region = "");
        /// <summary>
        /// 添加或更新（异步）
        /// </summary>
        /// <param name="key"></param>
        /// <param name="addFactory"></param>
        /// <param name="region">分区</param>
        /// <returns></returns>
        Task<TCacheValue> SetAsync(string key, Func<string, Task<TCacheValue>> addFactory, string region = "");
        /// <summary>
        /// 获取或添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="valueFactory">缓存对象工厂</param>
        /// <param name="region">分区</param>
        /// <returns></returns>
        Task<TCacheValue> GetOrAddAsync(string key, Func<string, Task<TCacheValue>> valueFactory, string region = "");
        /// <summary>
        /// 获取或添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="valueFactory">缓存对象工厂</param>
        /// <param name="value">取出的缓存值</param>
        /// <param name="region">分区</param>
        /// <returns></returns>
        bool TryGetOrAdd(string key, Func<string, TCacheValue> valueFactory, out TCacheValue value, string region = "");
        /// <summary>
        /// 获取指定缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">返回的缓存值</param>
        /// <param name="region">分区</param>
        /// <returns></returns>
        bool TryGet(string key, out TCacheValue value, string region = "");
    }
}