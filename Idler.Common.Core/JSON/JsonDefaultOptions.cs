using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace System.Text.Json
{
    public static class JsonDefaultOptions
    {
        /// <summary>
        /// 默认选项
        /// </summary>
        public readonly static JsonSerializerOptions Instance;

        static JsonDefaultOptions()
        {
            Instance = new JsonSerializerOptions()
            {
                NumberHandling = JsonNumberHandling.AllowReadingFromString,
                PropertyNamingPolicy = null,
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            Instance.Converters.Add(new DateTimeConverter());
        }
    }
}