using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace System
{
    /// <summary>
    /// Object的扩展方法.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// 转换数据类型
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="obj">待转换对象</param>
        /// <returns>转换结果</returns>
        public static T As<T>(this object obj)
            where T : class
        {
            return obj as T;
        }

        /// <summary>
        /// 通过Convert转换对象类型
        /// </summary>
        /// <param name="obj">待转换对象</param>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>转换结果</returns>
        public static T To<T>(this object obj)
            where T : struct
        {
            if (typeof(T) == typeof(Guid))
            {
                return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(obj.ToString());
            }

            return (T)Convert.ChangeType(obj, typeof(T), CultureInfo.InvariantCulture);
        }
    }
}