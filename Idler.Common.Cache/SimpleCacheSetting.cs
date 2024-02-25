namespace Idler.Common.Cache
{
    /// <summary>
    /// 缓存设置
    /// </summary>
    public class SimpleCacheSetting
    {
        /// <summary>
        /// 缓存Key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 过期时间（秒）
        /// </summary>
        public int ExpiresTime { get; set; } = -1;

        /// <summary>
        /// 二级缓存
        /// </summary>
        public bool SecondLevelCache { get; set; }

        /// <summary>
        /// 最大重试次数
        /// </summary>
        public int MaxRetries { get; set; } = 3;
    }
}