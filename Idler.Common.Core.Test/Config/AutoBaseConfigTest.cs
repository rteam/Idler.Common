using Idler.Common.Core.Config;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Idler.Common.Core.Test.Config;

[TestClass]
[TestSubject(typeof(AutoBaseConfig<>))]
public class AutoBaseConfigTest
{
    [TestMethod]
    public void TestFromConfig_EmptyString_ReturnsNewInstance()
    {
        // Arrange
        var autoBaseConfig = CreateAutoBaseConfigUnderTest();

        // Act
        var result = autoBaseConfig.FromConfig(string.Empty);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(TestConfig));
    }

    [TestMethod]
    public void TestFromConfig_NullString_ReturnsNewInstance()
    {
        // Arrange
        var autoBaseConfig = CreateAutoBaseConfigUnderTest();

        // Act
        var result = autoBaseConfig.FromConfig(null);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(TestConfig));
    }

    [TestMethod]
    public void TestFromConfig_JsonString_ReturnsDeserializedInstance()
    {
        // Arrange
        var autoBaseConfig = CreateAutoBaseConfigUnderTest();
        var jsonString =
            "{\"name\":\"abc\",\"key\":\"bcd\",\"num1\":\"1\",\"Num2\":\"1.1\",\"date\":\"2023-05-01\"}"; // The simplest valid JSON string.

        // Act
        var result = autoBaseConfig.FromConfig(jsonString);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(TestConfig));
        Assert.IsTrue(result.Key == "bcd");
        Assert.IsTrue(result.Name == "abc");
        Assert.IsTrue(result.Num1 == 1);
        Assert.IsTrue(result.num2 == (decimal)1.1);
        Assert.IsTrue(result.date.Year == 2023 && result.date.Month == 5 && result.date.Day == 1);
    }

    [TestMethod]
    public void TestToConfig_DefaultInstance_ReturnsJsonString()
    {
        // Arrange
        var autoBaseConfig = CreateAutoBaseConfigUnderTest();

        // Act
        var result = autoBaseConfig.ToConfig();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.StartsWith("{") && result.EndsWith("}")); // Basic JSON string validation.
    }

    private static AutoBaseConfig<TestConfig> CreateAutoBaseConfigUnderTest()
    {
        return new TestConfig() { Name = "test1", Key = "test2" };
    }
}

public class TestConfig : AutoBaseConfig<TestConfig>
{
    public string Name { get; set; }
    public string Key { get; set; }
    public int Num1 { get; set; }
    public decimal num2 { get; set; }
    public DateTime date { get; set; }
}