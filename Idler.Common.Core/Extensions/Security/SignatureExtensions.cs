using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace System.Security.Cryptography
{
    public static class SignatureExtensions
    {
        private static readonly string Noise = Environment.GetEnvironmentVariable("SIGNATURE_NOISE").IsEmpty()
            ? "b2V9sE2z8"
            : Environment.GetEnvironmentVariable("SIGNATURE_NOISE"); //绝对不要修改

        private static readonly char[] BcdLookup =
            { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };

        /// <summary>
        /// SHA1签名
        /// </summary>
        /// <param name="plainText">待签名的字符串</param>
        /// <param name="customNoise">自定义签名混淆码，为空则使用公共混淆吗</param>
        /// <param name="allowNoise">是否使用混淆码</param>
        /// <returns></returns>
        public static string SHA1(this string plainText, string customNoise = "", bool allowNoise = true)
        {
            if (plainText.IsEmpty())
                return string.Empty;

            byte[] hashedBytes = Encoding.GetEncoding(936).GetBytes(plainText + GenerateNoise(customNoise, allowNoise));
            HashAlgorithm hashAlgorithm = new SHA1CryptoServiceProvider();
            hashedBytes = hashAlgorithm.ComputeHash(hashedBytes);
            StringBuilder sb = new StringBuilder();
            foreach (byte hashedByte in hashedBytes)
            {
                sb.AppendFormat("{0:x2}", hashedByte);
            }

            return sb.ToString();
        }

        /// <summary>
        /// 校验SHA1签名
        /// </summary>
        /// <param name="signature">签名</param>
        /// <param name="plainText">明文</param>
        /// <param name="customNoise">自定义签名混淆码，为空则使用公共混淆吗</param>
        /// <param name="allowNoise">是否使用混淆码</param>
        /// <returns>
        /// 签名和明文任意一项为空时返回false，否则返回对比结果。
        /// </returns>
        public static bool SHA1Verify(this string signature, string plainText, string customNoise = "",
            bool allowNoise = true)
        {
            return (!plainText.IsEmpty() && !signature.IsEmpty()) &&
                   signature == SHA1(plainText, customNoise, allowNoise);
        }

        /// <summary>
        /// 基于RSA密钥的SHA1签名
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="publicKeyPath"></param>
        /// <param name="publicKeyPassword"></param>
        /// <returns></returns>
        public static string SHA1WithRSA(this string plainText, string publicKeyPath, string publicKeyPassword)
        {
            if (plainText.IsEmpty())
                return string.Empty;

            X509Certificate2 objx5092 = publicKeyPassword.IsEmpty()
                ? new X509Certificate2(publicKeyPath)
                : new X509Certificate2(publicKeyPath, publicKeyPassword);

            RSACryptoServiceProvider rsa = objx5092.PrivateKey as RSACryptoServiceProvider;
            byte[] data = Encoding.UTF8.GetBytes(plainText);
            byte[] hashvalue = rsa.SignData(data, "SHA1");
            return BytesToHexStr(hashvalue);
        }

        /// <summary>
        /// 将签名结果转化为16进制字符串
        /// </summary>
        /// <param name="hashedBytes">签名结果的byte数字</param>
        /// <returns>16进制字符串</returns>
        private static string BytesToHexStr(byte[] hashedBytes)
        {
            StringBuilder hexHash = new StringBuilder(hashedBytes.Length * 2);
            foreach (var item in hashedBytes)
            {
                hexHash.Append(BcdLookup[(item >> 4) & 0x0f]);
                hexHash.Append(BcdLookup[item & 0x0f]);
            }

            return hexHash.ToString();
        }

        /// <summary>
        /// 使用MD5算法签名
        /// </summary>
        /// <param name="plainText">签名前的字符串</param>
        /// <param name="md5Type">加密类型，默认x2:32位，x3:48位,x4:64位</param>
        /// <returns></returns>
        public static string MD5(this string plainText, string md5Type = "x2")
        {
            if (plainText.IsEmpty())
                return string.Empty;

            byte[] sor = Encoding.UTF8.GetBytes(plainText);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(sor);
            StringBuilder sb = new StringBuilder(40);
            foreach (var item in result)
            {
                sb.Append(item.ToString(md5Type));
            }

            return sb.ToString();
        }

        private static string GenerateNoise(string customNoise, bool allowNoise = true)
        {
            return allowNoise ? (customNoise.IsEmpty() ? Noise : customNoise) : string.Empty;
        }
    }
}