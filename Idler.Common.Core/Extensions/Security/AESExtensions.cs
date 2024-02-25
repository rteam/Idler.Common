using System;
using System.Linq;
using System.Text;

namespace System.Security.Cryptography
{
    /// <summary>
    /// AES加密助手
    /// </summary>
    public static class AESExtensions
    {
        /// <summary>
        /// 公钥
        /// </summary>
        private static string DefaultKey
        {
            get
            {
                string key = Environment.GetEnvironmentVariable("PUBLIC_KEY") ?? "4c9asd9bnga3dp02kfsad932kfhffku8";
                return key;
            }
        }

        /// <summary>
        /// AES解密函数
        /// </summary>
        /// <param name="cipherText">要解密的字符串</param>
        /// <param name="privateKey">私钥(最高32字节,对应256位AES)，为空则使用公钥</param>
        /// <returns>解密后的结果</returns>
        public static string AESDecrypt(this string cipherText, string privateKey = "")
        {
            try
            {
                byte[] keyArray = GetKeyArray(privateKey, DefaultKey);
                byte[] toEncryptArray = Convert.FromBase64String(cipherText);

                RijndaelManaged rDel = new RijndaelManaged
                    { Key = keyArray, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 };

                ICryptoTransform cTransform = rDel.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                return UTF8Encoding.UTF8.GetString(resultArray);
            }
            catch
            {
                return "err";
            }
        }

        /// <summary>
        /// AES加密函数
        /// </summary>
        /// <param name="plainText">要加密的字符串</param>
        /// <param name="privateKey">私钥(最高32字节,对应256位AES)，为空则使用公钥</param>
        /// <returns>加密结果</returns>
        public static string AESEncrypt(this string plainText, string privateKey = "")
        {
            byte[] keyArray = GetKeyArray(privateKey, DefaultKey);
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(plainText);

            RijndaelManaged rDel = new RijndaelManaged
                { Key = keyArray, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 };

            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        private static byte[] GetKeyArray(string initialKey, string defaultKey)
        {
            string finalKey = string.IsNullOrWhiteSpace(initialKey) ? defaultKey : initialKey;
            if (finalKey.Length > 32)
                finalKey = finalKey.Substring(0, 32);
            else if (finalKey.Length < 32)
                finalKey = finalKey.PadRight(32, '0');

            return UTF8Encoding.UTF8.GetBytes(finalKey);
        }
    }
}