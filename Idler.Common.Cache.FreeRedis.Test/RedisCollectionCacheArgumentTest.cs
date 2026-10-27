using JetBrains.Annotations;

namespace Idler.Common.Cache.FreeRedis.Test;

/// <summary>
/// 参数校验发生在访问 Redis 之前，无需 Redis 即可运行
/// </summary>
[TestClass]
[TestSubject(typeof(SimpleRedisCacheManager<>))]
public class RedisCollectionCacheArgumentTest
{
    private IRedisCollectionCache<string> cache = null!;

    [TestInitialize]
    public void Initialize()
    {
        this.cache = TestCacheFactory.CreateManager();
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("  ")]
    public void EmptyKey_Throws(string key)
    {
        Assert.ThrowsException<ArgumentNullException>(() => this.cache.SortedSetAdd(key, 1, "m"));
        Assert.ThrowsException<ArgumentNullException>(() => this.cache.SortedSetRangeByScore(key, 0, 1));
        Assert.ThrowsException<ArgumentNullException>(() => this.cache.SortedSetRemove(key, "m"));
        Assert.ThrowsException<ArgumentNullException>(() => this.cache.HashGetAll(key));
        Assert.ThrowsException<ArgumentNullException>(() => this.cache.HashGet(key, "f"));
        Assert.ThrowsException<ArgumentNullException>(() =>
            this.cache.HashSet(key, new Dictionary<string, string> { ["f"] = "v" }));
        Assert.ThrowsException<ArgumentNullException>(() => this.cache.HashDelete(key, new[] { "f" }));
    }

    [TestMethod]
    public void NullMemberOrField_Throws()
    {
        Assert.ThrowsException<ArgumentNullException>(() => this.cache.SortedSetAdd("k", 1, null!));
        Assert.ThrowsException<ArgumentNullException>(() => this.cache.SortedSetRemove("k", null!));
        Assert.ThrowsException<ArgumentNullException>(() => this.cache.HashGet("k", null!));
    }

    [TestMethod]
    public void NullCollections_Throw()
    {
        Assert.ThrowsException<ArgumentNullException>(() => this.cache.HashSet("k", null!));
        Assert.ThrowsException<ArgumentNullException>(() => this.cache.HashDelete("k", null!));
    }

    [TestMethod]
    public void HashDelete_FieldsContainNull_Throws()
    {
        Assert.ThrowsException<ArgumentException>(() => this.cache.HashDelete("k", new[] { "f", null! }));
    }

    [TestMethod]
    public void NegativeSkip_Throws()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            this.cache.SortedSetRangeByScore("k", 0, 1, skip: -1));
    }

    [TestMethod]
    public void EmptyCollections_ReturnZeroWithoutCallingRedis()
    {
        // 未设置 IDLER_TEST_REDIS 时连接串指向不可达地址，访问 Redis 会抛异常
        Assert.AreEqual(0, this.cache.HashSet("k", new Dictionary<string, string>()));
        Assert.AreEqual(0, this.cache.HashDelete("k", Array.Empty<string>()));
    }

    [TestMethod]
    public void Constructor_SettingNotFound_Throws()
    {
        Assert.ThrowsException<InvalidOperationException>(() =>
            new SimpleRedisCacheManager<string>("Missing", TestCacheFactory.CreateService(TestCacheFactory.DefaultSettings())));
    }

    [TestMethod]
    public void Constructor_SettingsNull_Throws()
    {
        Assert.ThrowsException<InvalidOperationException>(() =>
            new SimpleRedisCacheManager<string>(TestCacheFactory.DEFAULT_SETTING, TestCacheFactory.CreateService(null!)));
    }
}
