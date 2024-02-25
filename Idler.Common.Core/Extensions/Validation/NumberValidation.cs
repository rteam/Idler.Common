using System.Collections.Generic;
using System.Linq;

namespace System
{
    public static class NumberValidation
    {
        #region Int

        /// <summary>
        /// 从字符串转换Int
        /// </summary>
        /// <param name="inputString">待转换的字符串</param>
        /// <returns>如果转换失败则返回0</returns>
        public static int IntByString(this string inputString)
        {
            return int.TryParse(inputString, out int tInt) ? tInt : 0;
        }

        /// <summary>
        /// 将一组字符串转换为Int
        /// </summary>
        /// <param name="inputStrings">待转换为Int的字符串列表</param>
        /// <returns>转换失败的直接抛弃</returns>
        public static IList<int> IntByString(this IList<string> inputStrings)
        {
            IList<int> tList = new List<int>();

            foreach (string item in inputStrings)
                if (int.TryParse(item, out int tInt))
                    tList.Add(tInt);

            return tList;
        }

        /// <summary>
        /// 将一组字符串转换为int数组
        /// </summary>
        /// <param name="inputStrings"></param>
        /// <returns></returns>
        public static int[] IntByString(this string[] inputStrings)
        {
            if (inputStrings == null)
                return new int[] { };

            IList<int> tList = new List<int>();
            foreach (string item in inputStrings)
                if (int.TryParse(item, out int tInt))
                    tList.Add(tInt);

            return tList.ToArray<int>();
        }

        /// <summary>
        /// 字符串是否能转换为Int
        /// </summary>
        /// <param name="inputString">字符串</param>
        /// <returns></returns>
        public static bool IsInt(this string inputString)
        {
            return int.TryParse(inputString, out _);
        }

        #endregion

        #region Decimal

        /// <summary>
        /// 从字符串转换Decimal
        /// </summary>
        /// <param name="inputString">待转换的字符串</param>
        /// <returns>如果转换失败则返回0</returns>
        public static decimal DecimalByString(this string inputString)
        {
            return decimal.TryParse(inputString, out var tDecimal) ? tDecimal : 0;
        }

        /// <summary>
        /// 将一组字符串转换为Decimal
        /// </summary>
        /// <param name="inputStrings">待转换为Decimal的字符串列表</param>
        /// <returns>转换失败的直接抛弃</returns>
        public static IList<decimal> DecimalByString(this IList<string> inputStrings)
        {
            IList<decimal> tList = new List<decimal>();

            foreach (string item in inputStrings)
                if (decimal.TryParse(item, out var tDecimal))
                    tList.Add(tDecimal);

            return tList;
        }

        /// <summary>
        /// 字符串是否能转换为Decimal
        /// </summary>
        /// <param name="inputString">字符串</param>
        /// <returns></returns>
        public static bool IsDecimal(this string inputString)
        {
            return decimal.TryParse(inputString, out _);
        }

        #endregion
        
        #region Long

        /// <summary>
        /// 从字符串转换long
        /// </summary>
        /// <param name="inputString">待转换的字符串</param>
        /// <returns>如果转换失败则返回0</returns>
        public static long LongByString(this string inputString)
        {
            return long.TryParse(inputString, out var tLong) ? tLong : 0;
        }

        /// <summary>
        /// 将一组字符串转换为long
        /// </summary>
        /// <param name="inputStrings">待转换为long的字符串列表</param>
        /// <returns>转换失败的直接抛弃</returns>
        public static IList<long> longByString(this IList<string> inputStrings)
        {
            IList<long> tList = new List<long>();

            foreach (string Item in inputStrings)
            {
                if (long.TryParse(Item, out var tLong))
                    tList.Add(tLong);
            }

            return tList;
        }

        /// <summary>
        /// 字符串是否能转换为long
        /// </summary>
        /// <param name="inputString">字符串</param>
        /// <returns></returns>
        public static bool IsLong(this string inputString)
        {
            return long.TryParse(inputString, out _);
        }

        #endregion
    }
}