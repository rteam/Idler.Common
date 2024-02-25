using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Idler.Common.Cache.FreeRedis
{
    public static class FreeRedisExtensions
    {
        public static IServiceCollection AddSimpleCache(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<FreeRedisCacheOption>(configuration.GetSection("SimpleCache"));
            services.TryAddSingleton<FreeRedisService>();
            return services;
        }

        /// <summary>
        /// 注册缓存实例
        /// </summary>
        /// <param name="serviceCollection">服务集合</param>
        /// <param name="settingKey">设置Key</param>
        /// <returns></returns>
        public static IServiceCollection AddCacheSingleton<TCacheValue>(this IServiceCollection serviceCollection,
            string settingKey)
        {
            serviceCollection.AddSingleton<ISimpleCacheManager<TCacheValue>>(provider =>
            {
                FreeRedisService freeRedisService = provider.GetRequiredService<FreeRedisService>();

                return new SimpleRedisCacheManager<TCacheValue>(settingKey, freeRedisService);
            });

            return serviceCollection;
        }
    }
}