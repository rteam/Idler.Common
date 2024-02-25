using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace System.Text.Json
{
    public static class JsonExtensions
    {
        /// <summary>
        /// 将对象转换为Json字符串
        /// </summary>
        /// <param name="objectData">对象</param>
        /// <returns></returns>
        public static string ToJSON(this object objectData)
        {
            if (objectData == null)
                return string.Empty;

            return JsonSerializer.Serialize(objectData, JsonDefaultOptions.Instance);
        }

        /// <summary>
        /// 从Json反序列化
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="json">Json字符串</param>
        /// <returns></returns>
        public static T FromJson<T>(this string json)

        {
            if (json.IsEmpty())
                return default(T);

            return JsonSerializer.Deserialize<T>(json, JsonDefaultOptions.Instance);
        }

        /// <summary>
        /// 从Json反序列化
        /// </summary>
        /// <param name="json">Json字符串</param>
        /// <param name="objectType">对象类型</param>
        /// <returns></returns>
        public static object FromJson(this string json, Type objectType)
        {
            if (json.IsEmpty())
                return null;

            return JsonSerializer.Deserialize(json, objectType);
        }
    }
}