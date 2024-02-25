using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace System
{
    public static class Validation
    {
        /// <summary>
        /// 对象不能为空否则抛出异常
        /// </summary>
        /// <param name="inputObject">被检测的对象</param>
        /// <param name="parameterName">参数名称</param>
        /// <returns></returns>
        public static object ThrowIfNull(this object inputObject, string parameterName)
        {
            if (inputObject == null)
                throw new ArgumentNullException(parameterName);

            return inputObject;
        }

        /// <summary>
        /// 从字符串转换Byte
        /// </summary>
        /// <param name="inputString">待转换的字符串</param>
        /// <returns>如果转换失败则返回0</returns>
        public static byte ByteByString(this string inputString)
        {
            return byte.TryParse(inputString, out var tByte) ? tByte : (byte)0;
        }

        /// <summary>
        /// 字符串是否能转换为Byte
        /// </summary>
        /// <param name="inputString">字符串</param>
        /// <returns></returns>
        public static bool IsByte(this string inputString)
        {
            return byte.TryParse(inputString, out _);
        }

        /// <summary>
        /// 从字符串转换Bool
        /// </summary>
        /// <param name="inputString">待转换的字符串</param>
        /// <returns>如果转换失败则返回0</returns>
        public static bool BoolByString(this string inputString)
        {
            return bool.TryParse(inputString, out var tBool) && tBool;
        }

        /// <summary>
        /// 字符串是否能转换为Bool
        /// </summary>
        /// <param name="inputString">字符串</param>
        /// <returns></returns>
        public static bool IsBool(this string inputString)
        {
            return bool.TryParse(inputString, out _);
        }

        /// <summary>
        /// 将字符串转换为日期
        /// </summary>
        /// <param name="inputString">待转换的字符串</param>
        /// <returns>如果转换失败返回DateTime.Now</returns>
        public static DateTime DateTimeByString(this string inputString)
        {
            return DateTime.TryParse(inputString, out var tDateTime) ? tDateTime : DateTime.Now;
        }

        /// <summary>
        /// 字符串是否能转换为DateTime
        /// </summary>
        /// <param name="inputString">待检测的字符串</param>
        /// <returns>如果为空返回false</returns>
        public static bool IsDateTime(this string inputString)
        {
            return DateTime.TryParse(inputString, out _);
        }
    }
}