using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Idler.Common.Core.Test.Extensions.Security
{
    [TestClass]
    public class SignatureExtensionsTest
    {
        [TestInitialize]
        public void Init()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        [TestMethod]
        public void SHA1_WithDistintValues()
        {
            Assert.IsNotNull("Hello".SHA1());
            Assert.IsNotNull("安装".SHA1());
            Assert.IsNotNull("安装".SHA1("abcs", true));
            Assert.IsNotNull("安装".SHA1("abcd", false));
            Assert.IsNotNull("".SHA1());
            Assert.IsTrue((null as string).SHA1() == string.Empty);
        }

        [TestMethod]
        public void SHA1Verify_WithDistintValues()
        {
            Assert.IsTrue("Hello".SHA1().SHA1Verify("Hello"));
            Assert.IsTrue("安装".SHA1().SHA1Verify("安装"));

            Assert.IsTrue("安装".SHA1("abc").SHA1Verify("安装", "abc"));
            Assert.IsFalse("安装".SHA1("abc").SHA1Verify("安装", "bcd"));
            Assert.IsFalse("安装".SHA1(allowNoise: false).SHA1Verify("安装"));

            Assert.IsFalse("".SHA1().SHA1Verify("Hello"));
            Assert.IsFalse((null as string).SHA1().SHA1Verify("Hello"));
        }


        [TestMethod]
        public void MD5_EmptyString()
        {
            var result = "".MD5();
            Assert.IsTrue(result == String.Empty);
        }

        [TestMethod]
        public void MD5_NullString()
        {
            var result = (null as string).MD5();
            Assert.IsTrue(result == String.Empty);
        }

        [TestMethod]
        public void MD5_WithX3()
        {
            var result = "Hello".MD5("x3");
            Assert.IsTrue(result.Length >= 48);
        }

        [TestMethod]
        public void MD5_WithDistinctValues()
        {
            Assert.IsNotNull("Hello".MD5());
            Assert.IsFalse("Hello".MD5().Equals("hello".MD5()));
            Assert.IsFalse("安装".MD5().Equals("安陵容".MD5()));
        }
    }
}