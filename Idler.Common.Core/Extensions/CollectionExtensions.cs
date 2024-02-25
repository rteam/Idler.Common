using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Collections
{
    public static class CollectionExtensions
    {
        private static Random _random = new Random();

        /// <summary>
        /// 从集合中随机返回一条信息
        /// </summary>
        /// <typeparam name="T">集合中的对象数据类型</typeparam>
        /// <param name="inputList">从哪个集合返回随机数据</param>
        /// <returns></returns>
        public static T Random<T>(this IList<T> inputList)
        {
            if (inputList == null || inputList.Count == 0)
                return default(T);

            var flag = inputList.Count == 1 ? 0 : _random.Next(0, inputList.Count);
            return inputList[flag];
        }

        /// <summary>
        /// IEnumerable的循环扩展
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="action"></param>
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (T item in items)
            {
                action(item);
            }
        }

        /// <summary>
        /// 将IEnumerable转换为ConcurrentBag
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static ConcurrentBag<T> ToConcurrentBag<T>(this IEnumerable<T> items)
        {
            return items == null ? new ConcurrentBag<T>() : new ConcurrentBag<T>(items);
        }
    }
}