using System;
using System.Text.Json;
using System.Text.Json.Serialization;

// ReSharper disable once CheckNamespace
namespace System.Text.Json
{
    public static class JsonDefaultOptions
    {
        /// <summary>
        /// 默认选项
        /// </summary>
        public static readonly JsonSerializerOptions Instance;

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