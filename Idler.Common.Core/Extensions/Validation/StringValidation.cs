using System.Collections.Generic;

namespace System
{
    public static class StringValidation
    {
        #region String

        /// <summary>
        /// 字符串不能为空否则抛出异常
        /// </summary>
        /// <param name="inputString">被检测的字符串</param>
        /// <param name="parameterName">参数名称</param>
        /// <returns></returns>
        public static string ThrowIfNull(this string inputString, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(inputString))
                throw new ArgumentNullException(parameterName);

            return inputString;
        }

        /// <summary>
        /// 一组字符串中存在空的字符串则抛出异常
        /// </summary>
        /// <param name="inputStrings">被检测的字符串组</param>
        /// <param name="parameterName">参数名称</param>
        /// <returns></returns>
        public static IList<string> ThrowIfNull(this IList<string> inputStrings, string parameterName)
        {
            if (inputStrings == null || inputStrings.Count == 0)
                throw new ArgumentNullException(parameterName);

            return inputStrings;
        }

        /// <summary>
        /// 一组字符串中存在空的字符串则抛出异常
        /// </summary>
        /// <param name="inputStrings">被检测的字符串组</param>
        /// <param name="parameterName">参数名称</param>
        /// <returns></returns>
        public static string[] ThrowIfNull(this string[] inputStrings, string parameterName)
        {
            if (inputStrings == null || inputStrings.Length == 0)
                throw new ArgumentNullException(parameterName);

            return inputStrings;
        }

        /// <summary>
        /// 字符串是否为空
        /// </summary>
        /// <param name="inputString">被检测的字符串</param>
        /// <returns></returns>
        public static bool IsEmpty(this string inputString)
        {
            return string.IsNullOrWhiteSpace(inputString);
        }

        /// <summary>
        /// 检测一组字符串中是否存在空的字符串
        /// </summary>
        /// <param name="inputStrings">被检测的字符串组</param>
        /// <returns></returns>
        public static bool IsEmpty(this IList<string> inputStrings)
        {
            if (inputStrings == null || inputStrings.Count == 0)
                return true;

            foreach (string item in inputStrings)
            {
                if (item.IsEmpty())
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 检测一组字符串中是否存在空的字符串
        /// </summary>
        /// <param name="inputStrings">被检测的字符串组</param>
        /// <returns></returns>
        public static bool IsEmpty(this string[] inputStrings)
        {
            if (inputStrings == null || inputStrings.Length == 0)
                return true;

            foreach (string item in inputStrings)
            {
                if (item.IsEmpty())
                    return true;
            }

            return false;
        }

        #endregion
    }
}