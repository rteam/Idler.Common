using System.Collections.Generic;

namespace Idler.Common.Cache.FreeRedis
{
    /// <summary>
    /// 缓存配置
    /// </summary>
    public class FreeRedisCacheOption
    {
        /// <summary>
        /// 前缀
        /// </summary>
        public string Prefix { get; set; }
        /// <summary>
        /// Redis连接字符串
        /// </summary>
        public string Redis { get; set; }
        /// <summary>
        /// 设置
        /// </summary>
        public IList<SimpleCacheSetting> Settings { get; set; }
    }
}