using System;
using System.Text.Json;
using FreeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Idler.Common.Cache.FreeRedis
{
    /// <summary>
    /// 缓存服务
    /// </summary>
    public class FreeRedisService
    {
        /// <summary>
        /// RedisClient
        /// </summary>
        private static RedisClient _client;

        /// <summary>
        /// 初始化配置
        /// </summary>
        public FreeRedisCacheOption Option { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public FreeRedisService(IOptions<FreeRedisCacheOption> option, IConfiguration configuration)
        {
            if (option == null)
                throw new NullReferenceException(nameof(option));

            this.Option = option.Value;
            
            string connectionStr = configuration.GetConnectionString(CacheConst.CACHE_CONNECTION_NAME);
            if (!connectionStr.IsEmpty())
                this.Option.Redis = connectionStr;
            
            InitRedisClient();
        }

        private static readonly object obj = new object();

        /// <summary>
        /// 初始化Redis
        /// </summary>
        /// <returns></returns>
        bool InitRedisClient()
        {
            if (_client == null)
            {
                lock (obj)
                {
                    if (_client == null)
                    {
                        _client = new RedisClient(this.Option.Redis)
                        {
                            Serialize = obj => obj.ToJSON(),
                            Deserialize = (json, objectType) => json.FromJson(objectType)
                        };
                        return true;
                    }
                }
            }

            return _client != null;
        }

        /// <summary>
        /// 获取Client实例
        /// </summary>
        public RedisClient Instance
        {
            get
            {
                if (InitRedisClient())
                {
                    return _client;
                }

                throw new AggregateException("Redis不可用");
            }
        }
    }
}