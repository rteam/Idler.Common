using System;
using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Idler.Common.Core.Test.Extensions.Security
{
    [TestClass]
    public class AESExtensionsTest
    {
        private const string TestString = "TestString";
        private const string InvalidKey = "ThisIsAnInvalidKeyBecauseItsLengthIsMoreThan32Characters";

        [TestMethod]
        public void TestAESEncrypt()
        {
            string result = TestString.AESEncrypt();
            Assert.IsNotNull(result);
            Assert.AreNotEqual(TestString, result);

            result = TestString.AESEncrypt("");
            Assert.IsNotNull(result);
            Assert.AreNotEqual(TestString, result);

            result = TestString.AESEncrypt(InvalidKey);
            Assert.IsNotNull(result);
            Assert.AreNotEqual(TestString, result);
        }

        [TestMethod]
        public void TestAESDecrypt()
        {
            string encrypted = TestString.AESEncrypt();
            string result = encrypted.AESDecrypt();
            Assert.AreEqual(TestString, result);

            result = "err".AESDecrypt();
            Assert.AreEqual("err", result);

            result = encrypted.AESDecrypt(InvalidKey);
            Assert.AreEqual("err", result);
        }

        [TestMethod]
        public void TestAESDecryptEncryptSequence()
        {
            string encrypted = TestString.AESEncrypt();
            string result = encrypted.AESDecrypt();
            Assert.AreEqual(TestString, result);

            encrypted = TestString.AESEncrypt("");
            result = encrypted.AESDecrypt("");
            Assert.AreEqual(TestString, result);

        }
    }
}