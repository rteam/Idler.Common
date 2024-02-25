using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace System
{
    /// <summary>
    /// 字符串的扩展方法
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// 在字符串中匹配
        /// </summary>
        /// <param name="targetString">被匹配的字符串</param>
        /// <param name="keyword">匹配关键字</param>
        /// <param name="separator">分隔符，默认为,</param>
        /// <returns></returns>
        public static bool MatchString(this string targetString, string keyword, string separator = ",")
        {
            if (targetString.IsEmpty())
                return false;

            return targetString.GetSearchString(separator)
                .IndexOf(keyword.GetSearchString(separator), StringComparison.OrdinalIgnoreCase) > -1;
        }

        /// <summary>
        /// 返回被指定字符包裹的字符串
        /// </summary>
        /// <param name="inputString">要格式化的字符串</param>
        /// <param name="separator">分隔符，默认为,</param>
        /// <returns></returns>
        public static string GetSearchString(this string inputString, string separator = ",")
        {
            if (inputString.IsEmpty())
                return string.Empty;

            return inputString.SetStartChar(separator).SetEndChar(separator);
        }

        /// <summary>
        /// 清理被自定字符包裹的字符串
        /// </summary>
        /// <param name="inputString">要格式化的字符串</param>
        /// <param name="cleanChar">要清理的字符</param>
        /// <returns></returns>
        public static string CleanSearchString(this string inputString, string cleanChar = ",")
        {
            if (inputString.IsEmpty())
            {
                return string.Empty;
            }
            else if (inputString.StartsWith(cleanChar) && inputString.EndsWith(cleanChar))
            {
                int CharLen = cleanChar.Length;
                int InputStringLen = inputString.Length;
                if (CharLen * 2 == InputStringLen)
                {
                    return string.Empty;
                }
                else
                {
                    return inputString.Substring(CharLen, InputStringLen - CharLen * 2);
                }
            }
            else
            {
                return inputString;
            }
        }

        /// <summary>
        /// 拆分一个字符串到Guid列表
        /// </summary>
        /// <param name="inputString">待拆分的字符串</param>
        /// <param name="separator">分隔符（可选，默认为","）</param>
        /// <returns></returns>
        public static IList<Guid> SplitToGuidList(this string inputString, string separator = ",")
        {
            if (inputString.IsEmpty() || separator.IsEmpty())
                return new List<Guid>();

            return Regex.Split(inputString, separator).ToList().GuidByString();
        }

        /// <summary>
        /// 拆分一个字符串到Guid数组
        /// </summary>
        /// <param name="inputString">待拆分的字符串</param>
        /// <param name="separator">分隔符（可选，默认为“，”）</param>
        /// <returns></returns>
        public static Guid[] SplitToGuidArray(this string inputString, string separator = ",")
        {
            if (inputString.IsEmpty() || separator.IsEmpty())
                return new Guid[] { };

            return inputString.Split(',').GuidByString();
        }

        /// <summary>
        /// 拆分一个字符串到int数组
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static IList<int> SplitToIntList(this string inputString, string separator = ",")
        {
            if (inputString.IsEmpty() || separator.IsEmpty())
                return new List<int>();

            return Regex.Split(inputString, separator).ToList().IntByString();
        }

        /// <summary>
        /// 拆分一个字符串到Int数组
        /// </summary>
        /// <param name="inputString">要拆分的字符串</param>
        /// <param name="separator">分隔符</param>
        /// <returns></returns>
        public static int[] SplitToIntArray(this string inputString, string separator = ",")
        {
            if (inputString.IsEmpty() || separator.IsEmpty())
                return new int[] { };

            return Regex.Split(inputString, separator).ToArray().IntByString();
        }

        /// <summary>
        /// 设置结尾字符
        /// </summary>
        /// <param name="inputString">要设置的字符串</param>
        /// <param name="endChar">结尾字符为</param>
        /// <returns>
        /// 返回处理结果，如果结尾已包括该字符则直接返回，否则附加。
        /// </returns>
        public static string SetEndChar(this string inputString, string endChar)
        {
            if (inputString.EndsWith(endChar))
                return inputString;

            return inputString + endChar;
        }

        /// <summary>
        /// 设置起始字符
        /// </summary>
        /// <param name="inputString">要设置的字符串</param>
        /// <param name="startChar">起始字符为</param>
        /// <returns>
        /// 返回处理结果，如果开始位置已包括该字符则直接返回，否则附加。
        /// </returns>
        public static string SetStartChar(this string inputString, string startChar)
        {
            if (inputString.StartsWith(startChar))
                return inputString;

            return startChar + inputString;
        }
    }
}