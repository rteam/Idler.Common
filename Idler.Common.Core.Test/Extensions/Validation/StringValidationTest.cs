using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;

namespace Idler.Common.Core.Test.Extensions.Validation
{
    [TestClass]
    [TestSubject(typeof(StringValidation))]
    public class StringValidationTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestThrowIfNullString()
        {
            string nullString = null;
            nullString.ThrowIfNull("name");
        }

        [TestMethod]
        public void TestThrowIfNullString_NotNull()
        {
            string str = "Hello";
            Assert.AreEqual(str, str.ThrowIfNull("name"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestThrowIfNullIListString()
        {
            IList<string> list = new List<string>();
            list.ThrowIfNull("name");
        }

        [TestMethod]
        public void TestThrowIfNullIListString_NotNull()
        {
            IList<string> list = new List<string> { "Hello" };
            CollectionAssert.AreEqual(list as ICollection, list.ThrowIfNull("name") as ICollection);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestThrowIfNullArrayString()
        {
            string[] arr = new string[] { };
            arr.ThrowIfNull("name");
        }

        [TestMethod]
        public void TestThrowIfNullArrayString_NotNull()
        {
            string[] arr = new string[] { "Hello" };
            CollectionAssert.AreEqual(arr, arr.ThrowIfNull("name"));
        }

        [TestMethod]
        public void TestIsEmptyString()
        {
            string str = null;
            Assert.IsTrue(str.IsEmpty());

            str = "Hello";
            Assert.IsFalse(str.IsEmpty());
        }

        [TestMethod]
        public void TestIsEmptyIListString()
        {
            IList<string> list = null;
            Assert.IsTrue(list.IsEmpty());

            list = new List<string>();
            Assert.IsTrue(list.IsEmpty());

            list = new List<string> { null };
            Assert.IsTrue(list.IsEmpty());

            list = new List<string> { "Hello" };
            Assert.IsFalse(list.IsEmpty());
        }

        [TestMethod]
        public void TestIsEmptyArrayString()
        {
            string[] arr = null;
            Assert.IsTrue(arr.IsEmpty());

            arr = new string[] { };
            Assert.IsTrue(arr.IsEmpty());

            arr = new string[] { null };
            Assert.IsTrue(arr.IsEmpty());

            arr = new string[] { "Hello" };
            Assert.IsFalse(arr.IsEmpty());
        }
    }
}