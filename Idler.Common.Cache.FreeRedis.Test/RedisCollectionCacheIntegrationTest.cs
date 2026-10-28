using System.Collections.Concurrent;
using JetBrains.Annotations;

namespace Idler.Common.Cache.FreeRedis.Test;

/// <summary>
/// 需要 Redis，通过环境变量 IDLER_TEST_REDIS 指定连接串（例如 127.0.0.1:6379），未设置时跳过
/// </summary>
[TestClass]
[TestSubject(typeof(SimpleRedisCacheManager<>))]
public class RedisCollectionCacheIntegrationTest
{
    private const string KEY = "collection";

    private TestableCacheManager manager = null!;
    private TestableCacheManager expiringManager = null!;
    private IRedisCollectionCache<string> cache = null!;
    private IRedisCollectionCache<string> expiringCache = null!;
    private IRedisCommandCache<string> commandCache = null!;
    private IRedisCommandCache<string> expiringCommandCache = null!;
    private string region = null!;

    [TestInitialize]
    public void Initialize()
    {
        if (!TestCacheFactory.RedisAvailable)
            Assert.Inconclusive($"未设置环境变量 {TestCacheFactory.REDIS_ENVIRONMENT_VARIABLE}，跳过 Redis 集成测试");

        this.region = Guid.NewGuid().ToString("N");
        this.manager = TestCacheFactory.CreateManager(TestCacheFactory.DEFAULT_SETTING);
        this.expiringManager = TestCacheFactory.CreateManager(TestCacheFactory.EXPIRING_SETTING);
        this.cache = this.manager;
        this.expiringCache = this.expiringManager;
        this.commandCache = this.manager;
        this.expiringCommandCache = this.expiringManager;
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (TestCacheFactory.RedisAvailable)
            this.manager.RemoveRegion(this.region);
    }

    private void SeedSortedSet()
    {
        this.cache.SortedSetAdd(KEY, 3, "c", this.region);
        this.cache.SortedSetAdd(KEY, 1, "a", this.region);
        this.cache.SortedSetAdd(KEY, 2, "b", this.region);
        this.cache.SortedSetAdd(KEY, 4, "d", this.region);
    }

    [TestMethod]
    public void SortedSetRangeByScore_Default_ReturnsAllAscendingWithInclusiveBounds()
    {
        this.SeedSortedSet();

        CollectionAssert.AreEqual(new[] { "a", "b", "c", "d" },
            this.cache.SortedSetRangeByScore(KEY, 1, 4, region: this.region));
        CollectionAssert.AreEqual(new[] { "b", "c" },
            this.cache.SortedSetRangeByScore(KEY, 2, 3, region: this.region));
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(-1)]
    public void SortedSetRangeByScore_SkipWithoutTake_ReturnsRemaining(int take)
    {
        this.SeedSortedSet();

        CollectionAssert.AreEqual(new[] { "c", "d" },
            this.cache.SortedSetRangeByScore(KEY, 0, 10, skip: 2, take: take, region: this.region));
    }

    [TestMethod]
    public void SortedSetRangeByScore_Take_LimitsCount()
    {
        this.SeedSortedSet();

        CollectionAssert.AreEqual(new[] { "a", "b" },
            this.cache.SortedSetRangeByScore(KEY, 0, 10, take: 2, region: this.region));
        CollectionAssert.AreEqual(new[] { "b", "c" },
            this.cache.SortedSetRangeByScore(KEY, 0, 10, skip: 1, take: 2, region: this.region));
    }

    [TestMethod]
    public void SortedSetRangeByScore_EmptyResults()
    {
        this.SeedSortedSet();

        Assert.AreEqual(0, this.cache.SortedSetRangeByScore(KEY, 10, 1, region: this.region).Length);
        Assert.AreEqual(0, this.cache.SortedSetRangeByScore("missing", 0, 10, region: this.region).Length);
    }

    [TestMethod]
    public void SortedSetAdd_ReturnsNewMemberCount()
    {
        Assert.AreEqual(1, this.cache.SortedSetAdd(KEY, 1, "a", this.region));
        Assert.AreEqual(0, this.cache.SortedSetAdd(KEY, 5, "a", this.region));
        this.cache.SortedSetAdd(KEY, 2, "b", this.region);

        CollectionAssert.AreEqual(new[] { "b", "a" },
            this.cache.SortedSetRangeByScore(KEY, 0, 10, region: this.region));
    }

    [TestMethod]
    public void SortedSetRemove_ReturnsRemovedCount()
    {
        this.SeedSortedSet();

        Assert.AreEqual(1, this.cache.SortedSetRemove(KEY, "a", this.region));
        Assert.AreEqual(0, this.cache.SortedSetRemove(KEY, "a", this.region));
    }

    [TestMethod]
    public void Hash_ReadWriteDelete()
    {
        Assert.AreEqual(2, this.cache.HashSet(KEY, new Dictionary<string, string> { ["f1"] = "v1", ["f2"] = "v2" }, this.region));
        Assert.AreEqual(1, this.cache.HashSet(KEY, new Dictionary<string, string> { ["f1"] = "v1-new", ["f3"] = "v3" }, this.region));

        Assert.AreEqual("v1-new", this.cache.HashGet(KEY, "f1", this.region));
        Assert.IsNull(this.cache.HashGet(KEY, "missing", this.region));

        var all = this.cache.HashGetAll(KEY, this.region);
        Assert.AreEqual(3, all.Count);
        Assert.AreEqual("v3", all["f3"]);

        Assert.AreEqual(2, this.cache.HashDelete(KEY, new[] { "f1", "f2", "missing" }, this.region));
        Assert.AreEqual(0, this.cache.HashDelete(KEY, new[] { "f1" }, this.region));
    }

    [TestMethod]
    public void HashGetAll_MissingKey_ReturnsEmpty()
    {
        Assert.AreEqual(0, this.cache.HashGetAll("missing", this.region).Count);
    }

    [TestMethod]
    public void SetNx_CreatesKeyWithoutOverwriting()
    {
        Assert.IsTrue(this.commandCache.SetNx("string-nx", "0", this.region));
        Assert.IsFalse(this.commandCache.SetNx("string-nx", "9", this.region));

        string cacheKey = this.manager.CacheKey("string-nx", this.region);
        Assert.AreEqual("0", this.manager.RedisClient.Get<string>(cacheKey));
    }

    [TestMethod]
    public void SetNx_WithExpiresTime_SetsTtl()
    {
        Assert.IsTrue(this.expiringCommandCache.SetNx("string-nx-exp", "0", this.region));

        long ttl = this.expiringManager.RedisClient.Ttl(this.expiringManager.CacheKey("string-nx-exp", this.region));
        Assert.IsTrue(ttl > 0 && ttl <= TestCacheFactory.EXPIRES_TIME, $"ttl={ttl}");
    }

    [TestMethod]
    public void SetNx_WithoutExpiresTime_DoesNotSetTtl()
    {
        Assert.IsTrue(this.commandCache.SetNx("string-nx-no-exp", "0", this.region));

        long ttl = this.manager.RedisClient.Ttl(this.manager.CacheKey("string-nx-no-exp", this.region));
        Assert.AreEqual(-1, ttl);
    }

    [TestMethod]
    public void Incr_ReturnsIncrementedValue()
    {
        Assert.AreEqual(1, this.commandCache.Incr("string-incr-one", this.region));
        Assert.AreEqual(2, this.commandCache.Incr("string-incr-one", this.region));
        Assert.AreEqual(3, this.commandCache.Incr("string-incr-one", this.region));
    }

    [TestMethod]
    public void Incr_DoesNotSetTtl()
    {
        Assert.AreEqual(1, this.expiringCommandCache.Incr("string-incr-one-no-exp", this.region));

        long ttl = this.expiringManager.RedisClient.Ttl(this.expiringManager.CacheKey("string-incr-one-no-exp", this.region));
        Assert.AreEqual(-1, ttl);
    }

    [TestMethod]
    public void Incr_IsIsolatedByRegion()
    {
        string firstRegion = $"{this.region}-first";
        string secondRegion = $"{this.region}-second";

        Assert.AreEqual(1, this.commandCache.Incr("string-incr-one-region", firstRegion));
        Assert.AreEqual(1, this.commandCache.Incr("string-incr-one-region", secondRegion));
        Assert.AreEqual(2, this.commandCache.Incr("string-incr-one-region", firstRegion));

        this.manager.Remove("string-incr-one-region", firstRegion);
        this.manager.Remove("string-incr-one-region", secondRegion);
    }

    [TestMethod]
    public void Incr_Concurrent_ReturnsUniqueSequence()
    {
        var values = new ConcurrentBag<long>();

        Parallel.For(0, 100, new ParallelOptions { MaxDegreeOfParallelism = 16 }, _ =>
            values.Add(this.commandCache.Incr("string-incr-one-concurrent", this.region)));

        CollectionAssert.AreEqual(Enumerable.Range(1, 100).Select(t => (long)t).ToArray(), values.OrderBy(t => t).ToArray());
    }

    [TestMethod]
    public void IncrBy_ReturnsIncrementedValue()
    {
        Assert.AreEqual(5, this.commandCache.IncrBy("string-incr", 5, this.region));
        Assert.AreEqual(8, this.commandCache.IncrBy("string-incr", 3, this.region));
        Assert.AreEqual(6, this.commandCache.IncrBy("string-incr", -2, this.region));
    }

    [TestMethod]
    public void IncrBy_DoesNotSetTtl()
    {
        Assert.AreEqual(5, this.expiringCommandCache.IncrBy("string-incr-no-exp", 5, this.region));

        long ttl = this.expiringManager.RedisClient.Ttl(this.expiringManager.CacheKey("string-incr-no-exp", this.region));
        Assert.AreEqual(-1, ttl);
    }

    [TestMethod]
    public void IncrBy_IsIsolatedByRegion()
    {
        string firstRegion = $"{this.region}-first";
        string secondRegion = $"{this.region}-second";

        Assert.AreEqual(1, this.commandCache.IncrBy("string-region", 1, firstRegion));
        Assert.AreEqual(1, this.commandCache.IncrBy("string-region", 1, secondRegion));
        Assert.AreEqual(3, this.commandCache.IncrBy("string-region", 2, firstRegion));

        this.manager.Remove("string-region", firstRegion);
        this.manager.Remove("string-region", secondRegion);
    }

    [TestMethod]
    public void IncrBy_Concurrent_ReturnsUniqueSequence()
    {
        var values = new ConcurrentBag<long>();

        Parallel.For(0, 100, new ParallelOptions { MaxDegreeOfParallelism = 16 }, _ =>
            values.Add(this.commandCache.IncrBy("string-concurrent", 2, this.region)));

        CollectionAssert.AreEqual(Enumerable.Range(1, 100).Select(t => (long)t * 2).ToArray(), values.OrderBy(t => t).ToArray());
    }

    [TestMethod]
    public void Writes_WithExpiresTime_SetTtl()
    {
        this.expiringCache.SortedSetAdd("zset", 1, "a", this.region);
        this.expiringCache.HashSet("hash", new Dictionary<string, string> { ["f"] = "v" }, this.region);

        foreach (string key in new[] { "zset", "hash" })
        {
            long ttl = this.expiringManager.RedisClient.Ttl(this.expiringManager.CacheKey(key, this.region));
            Assert.IsTrue(ttl > 0 && ttl <= TestCacheFactory.EXPIRES_TIME, $"{key} ttl={ttl}");
        }
    }

    [TestMethod]
    public void Writes_WithoutExpiresTime_DoNotSetTtl()
    {
        this.cache.SortedSetAdd("zset", 1, "a", this.region);
        this.cache.HashSet("hash", new Dictionary<string, string> { ["f"] = "v" }, this.region);

        foreach (string key in new[] { "zset", "hash" })
            Assert.AreEqual(-1, this.manager.RedisClient.Ttl(this.manager.CacheKey(key, this.region)), key);
    }
}
