using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace Idler.Common.Cache.FreeRedis.Test;

[TestClass]
[TestSubject(typeof(FreeRedisExtensions))]
public class FreeRedisExtensionsTest
{
    [TestMethod]
    public void AddCacheSingleton_ResolvesSameInstanceForAllServiceTypes()
    {
        var services = new ServiceCollection();
        services.AddSingleton(TestCacheFactory.CreateService(TestCacheFactory.DefaultSettings()));
        services.AddCacheSingleton<string>(TestCacheFactory.DEFAULT_SETTING);

        using ServiceProvider provider = services.BuildServiceProvider();

        var manager = provider.GetRequiredService<SimpleRedisCacheManager<string>>();
        Assert.AreSame(manager, provider.GetRequiredService<ISimpleCacheManager<string>>());
        Assert.AreSame(manager, provider.GetRequiredService<IRedisCollectionCache<string>>());
        Assert.AreSame(manager, provider.GetRequiredService<IRedisCommandCache<string>>());
    }

    [TestMethod]
    public void AddCacheSingleton_CalledTwice_DoesNotThrow()
    {
        var services = new ServiceCollection();
        services.AddSingleton(TestCacheFactory.CreateService(TestCacheFactory.DefaultSettings()));
        services.AddCacheSingleton<string>(TestCacheFactory.DEFAULT_SETTING);
        services.AddCacheSingleton<string>(TestCacheFactory.EXPIRING_SETTING);

        using ServiceProvider provider = services.BuildServiceProvider();

        Assert.IsNotNull(provider.GetRequiredService<IRedisCollectionCache<string>>());
        Assert.IsNotNull(provider.GetRequiredService<IRedisCommandCache<string>>());
    }
}
