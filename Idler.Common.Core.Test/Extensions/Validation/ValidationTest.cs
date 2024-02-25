using JetBrains.Annotations;
using System;

namespace Idler.Common.Core.Test.Extensions.Validation
{
    [TestClass]
    [TestSubject(typeof(System.Validation))]
    public class ValidationTest
    {
        [TestMethod]
        public void ThrowIfNull_NullInput_ThrowsException()
        {
            object nullObj = null;

            Assert.ThrowsException<ArgumentNullException>(() => nullObj.ThrowIfNull(nameof(nullObj)));
        }

        [TestMethod]
        public void ThrowIfNull_ValidInput_NoException()
        {
            object nonNullObj = "Test";

            Assert.IsNotNull(nonNullObj.ThrowIfNull(nameof(nonNullObj)));
        }

        [DataTestMethod]
        [DataRow("128", (byte)128)]
        [DataRow("127", (byte)127)]
        [DataRow("0", (byte)0)]
        [DataRow("255", (byte)255)]
        [DataRow("256", (byte)0)]
        [DataRow("-1", (byte)0)]
        public void ByteByString_ConvertingVariousStrings(string input, byte expected)
        {
            Assert.AreEqual(expected, input.ByteByString());
        }

        [DataTestMethod]
        [DataRow("256", false)]
        [DataRow("127", true)]
        [DataRow("0", true)]
        [DataRow("-1", false)]
        public void IsByte_CheckingValidity(string input, bool expected)
        {
            Assert.AreEqual(expected, input.IsByte());
        }

        [DataTestMethod]
        [DataRow("true", true)]
        [DataRow("True", true)]
        [DataRow("false", false)]
        [DataRow("False", false)]
        [DataRow("impulse", false)]
        public void BoolByString_ConvertingVariousStrings(string input, bool expected)
        {
            Assert.AreEqual(expected, input.BoolByString());
        }

        [DataTestMethod]
        [DataRow("true", true)]
        [DataRow("True", true)]
        [DataRow("false", true)]
        [DataRow("False", true)]
        [DataRow("impulse", false)]
        public void IsBool_CheckingValidity(string input, bool expected)
        {
            Assert.AreEqual(expected, input.IsBool());
        }

        [TestMethod]
        public void DateTimeByString_NotParsable_ReturnsDateTimeNow()
        {
            string unparsable = "nottadate";

            Assert.AreEqual(DateTime.Now.ToShortDateString(), unparsable.DateTimeByString().ToShortDateString());
        }

        [DataTestMethod]
        [DataRow("01/01/2000", true)]
        [DataRow("2000-01-01", true)]
        [DataRow("2000/1/1", true)]
        [DataRow("notadate", false)]
        public void IsDateTime_CheckingValidity(string input, bool expected)
        {
            Assert.AreEqual(expected, input.IsDateTime());
        }
    }
}