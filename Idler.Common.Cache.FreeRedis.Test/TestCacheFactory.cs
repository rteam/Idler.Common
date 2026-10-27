using FreeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Idler.Common.Cache.FreeRedis.Test;

internal static class TestCacheFactory
{
    public const string REDIS_ENVIRONMENT_VARIABLE = "IDLER_TEST_REDIS";
    public const string DEFAULT_SETTING = "Default";
    public const string EXPIRING_SETTING = "Expiring";
    public const int EXPIRES_TIME = 60;

    // FreeRedisService 的 RedisClient 是进程内静态单例，所有测试必须使用同一个连接串
    private static readonly string ConnectionString =
        Environment.GetEnvironmentVariable(REDIS_ENVIRONMENT_VARIABLE) ?? "127.0.0.1:1";

    public static bool RedisAvailable =>
        !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(REDIS_ENVIRONMENT_VARIABLE));

    public static FreeRedisService CreateService(IList<SimpleCacheSetting> settings, string prefix = "idler-test:")
    {
        var option = new FreeRedisCacheOption
        {
            Prefix = prefix,
            Redis = ConnectionString,
            Settings = settings
        };

        IConfiguration configuration = new ConfigurationBuilder().Build();
        return new FreeRedisService(Options.Create(option), configuration);
    }

    public static IList<SimpleCacheSetting> DefaultSettings() => new List<SimpleCacheSetting>
    {
        new() { Key = DEFAULT_SETTING, ExpiresTime = -1 },
        new() { Key = EXPIRING_SETTING, ExpiresTime = EXPIRES_TIME }
    };

    public static TestableCacheManager CreateManager(string settingKey = DEFAULT_SETTING, string prefix = "idler-test:")
    {
        return new TestableCacheManager(settingKey, CreateService(DefaultSettings(), prefix));
    }
}

internal class TestableCacheManager : SimpleRedisCacheManager<string>
{
    public TestableCacheManager(string settingKey, FreeRedisService freeRedisService)
        : base(settingKey, freeRedisService)
    {
    }

    public RedisClient RedisClient => this.Client;

    public string CacheKey(string key, string region) => this.AssembleCacheKey(key, region);
}
