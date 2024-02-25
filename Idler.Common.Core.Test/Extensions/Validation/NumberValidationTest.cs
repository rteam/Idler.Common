using System.Collections;
using JetBrains.Annotations;

namespace Idler.Common.Core.Test.Extensions.Validation;

[TestClass]
[TestSubject(typeof(NumberValidation))]
public class NumberValidationTest
{
    [TestMethod]
    public void TestIntParsing()
    {
        Assert.AreEqual(123, "123".IntByString());
        Assert.AreEqual(0, "abc".IntByString());

        var list = new List<string>() { "123", "abc", "456", "def" };
        CollectionAssert.AreEqual(new List<int>() { 123, 456 }, list.IntByString() as ICollection);

        var array = new string[] { "123", "abc", "456", "def" };
        CollectionAssert.AreEqual(new int[] { 123, 456 }, array.IntByString());

        Assert.IsTrue("123".IsInt());
        Assert.IsFalse("abc".IsInt());
    }

    [TestMethod]
    public void TestDecimalParsing()
    {
        Assert.AreEqual(123.0M, "123".DecimalByString());
        Assert.AreEqual(0, "abc".DecimalByString());

        var list = new List<string>() { "123", "abc", "456.78", "def" };
        CollectionAssert.AreEqual(new List<decimal>() { 123M, 456.78M }, list.DecimalByString() as ICollection);

        Assert.IsTrue("123".IsDecimal());
        Assert.IsFalse("abc".IsDecimal());
    }

    [TestMethod]
    public void TestLongParsing()
    {
        Assert.AreEqual(123L, "123".LongByString());
        Assert.AreEqual(0L, "abc".LongByString());

        var list = new List<string>() { "123", "abc", "456", "def" };
        CollectionAssert.AreEqual(new List<long>() { 123L, 456L }, list.longByString() as ICollection);

        Assert.IsTrue("123".IsLong());
        Assert.IsFalse("abc".IsLong());
    }
    
            [TestMethod]
        public void TestIntByString()
        {
            Assert.AreEqual(123, "123".IntByString());
            Assert.AreEqual(0, "test".IntByString());
        }

        [TestMethod]
        public void TestIntByStringList()
        {
            CollectionAssert.AreEqual(new List<int> { 123, 456 }, (ICollection?)new List<string> { "123", "456" }.IntByString());
            CollectionAssert.AreEqual(new List<int> { 123 }, (ICollection?)new List<string> { "123", "test" }.IntByString());
        }

        [TestMethod]
        public void TestIntByStringArray()
        {
            CollectionAssert.AreEqual(new int[] { 123, 456 }, new string[] { "123", "456" }.IntByString());
            CollectionAssert.AreEqual(new int[] { 123 }, new string[] { "123", "test" }.IntByString());
        }

        [TestMethod]
        public void TestIsInt()
        {
            Assert.IsTrue("123".IsInt());
            Assert.IsFalse("test".IsInt());
        }

        [TestMethod]
        public void TestDecimalByString()
        {
            Assert.AreEqual(123.45m, "123.45".DecimalByString());
            Assert.AreEqual(0, "test".DecimalByString());
        }

        [TestMethod]
        public void TestDecimalByStringList()
        {
            CollectionAssert.AreEqual(new List<decimal> { 123.45m, 456.78m }, new List<string> { "123.45", "456.78" }.DecimalByString() as ICollection);
            CollectionAssert.AreEqual(new List<decimal> { 123.45m }, (ICollection?)new List<string> { "123.45", "test" }.DecimalByString());
        }

        [TestMethod]
        public void TestIsDecimal()
        {
            Assert.IsTrue("123.45".IsDecimal());
            Assert.IsFalse("test".IsDecimal());
        }

        [TestMethod]
        public void TestLongByString()
        {
            Assert.AreEqual(1234567890123, "1234567890123".LongByString());
            Assert.AreEqual(0, "test".LongByString());
        }

        [TestMethod]
        public void TestLongByStringList()
        {
            var actual1 = new List<string> { "1234567890123", "4567890123456" }.longByString();
            var actual2 = new List<string> { "1234567890123", "test" }.longByString();
            
            CollectionAssert.AreEqual(new List<long> { 1234567890123, 4567890123456 }, actual1 as ICollection);
            CollectionAssert.AreEqual(new List<long> { 1234567890123 }, actual2 as ICollection);
        }

        [TestMethod]
        public void TestIsLong()
        {
            Assert.IsTrue("1234567890123".IsLong());
            Assert.IsFalse("test".IsLong());
        }
}