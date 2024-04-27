using System.Text.Json;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Idler.Common.Core.Test.JSON;

[TestClass]
[TestSubject(typeof(JsonExtensions))]
public class JsonExtensionsTest
{
    [TestMethod]
    public void ToJSON_NullObject_ReturnsEmptyString()
    {
        object testObject = null;
        var result = testObject.ToJSON();
        Assert.AreEqual(string.Empty, result);
    }

    [TestMethod]
    public void ToJSON_NonNullObject_ReturnsSerializedString()
    {
        object testObject = new { Name = "Test", Age = 42 };
        var result = testObject.ToJSON();
        Assert.AreEqual("{\"Name\":\"Test\",\"Age\":42}", result);
    }

    [TestMethod]
    public void FromJson_StringGenerics_ReturnsObject()
    {
        var jsonString = "{\"Name\":\"Test\",\"Age\":42}";
        var result = jsonString.FromJson<Person>();
        Assert.IsNotNull(result);
        Assert.AreEqual("Test", result.Name);
        Assert.AreEqual(42, result.Age);
    }

    [TestMethod]
    public void FromJson_StringGenerics_EmptyString_ReturnsDefault()
    {
        var jsonString = "";
        var result = jsonString.FromJson<Person>();
        Assert.AreEqual(default, result);
    }

    [TestMethod]
    public void FromJson_StringType_ReturnsObject()
    {
        var jsonString = "{\"Name\":\"Test\",\"Age\":42}";
        var result = jsonString.FromJson(typeof(Person)) as Person;
        Assert.IsNotNull(result);
        Assert.AreEqual("Test", result.Name);
        Assert.AreEqual(42, result.Age);
    }

    [TestMethod]
    public void FromJson_StringType_EmptyString_ReturnsNull()
    {
        var jsonString = "";
        var result = jsonString.FromJson(typeof(Person));
        Assert.IsNull(result);
    }

    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}