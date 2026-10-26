using JetBrains.Annotations;

namespace Idler.Common.Cache.FreeRedis.Test;

[TestClass]
[TestSubject(typeof(SimpleRedisCacheManager<>))]
public class ResolveRangeLimitTest
{
    [TestMethod]
    [DataRow(0, 0, 0, -1, DisplayName = "skip=0,take=0 不附加 LIMIT")]
    [DataRow(0, -5, 0, -1, DisplayName = "skip=0,take<0 不附加 LIMIT")]
    [DataRow(10, 0, 10, -1, DisplayName = "skip>0,take=0 返回剩余全部")]
    [DataRow(10, -1, 10, -1, DisplayName = "skip>0,take<0 返回剩余全部")]
    [DataRow(0, 3, 0, 3, DisplayName = "take>0 限制条数")]
    [DataRow(5, 3, 5, 3, DisplayName = "skip>0,take>0 分页")]
    public void ResolveRangeLimit_MapsSkipTake(int skip, int take, int expectedOffset, int expectedCount)
    {
        var (offset, count) = SimpleRedisCacheManager<string>.ResolveRangeLimit(skip, take);

        Assert.AreEqual(expectedOffset, offset);
        Assert.AreEqual(expectedCount, count);
    }

    [TestMethod]
    public void ResolveRangeLimit_TakeNotPositive_NeverProducesZeroCountWithOffset()
    {
        // FreeRedis 在 offset > 0 时会发送 LIMIT offset count，count 为 0 会让 Redis 返回空数组
        for (int skip = 1; skip <= 3; skip++)
        for (int take = -2; take <= 0; take++)
        {
            var (_, count) = SimpleRedisCacheManager<string>.ResolveRangeLimit(skip, take);
            Assert.AreNotEqual(0, count, $"skip={skip}, take={take}");
        }
    }

    [TestMethod]
    public void ResolveRangeLimit_NegativeSkip_Throws()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            SimpleRedisCacheManager<string>.ResolveRangeLimit(-1, 0));
    }
}
