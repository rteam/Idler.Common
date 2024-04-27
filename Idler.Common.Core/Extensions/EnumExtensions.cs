using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace System;

/// <summary>
/// 枚举扩展
/// </summary>
public static class EnumExtensions
{
    private static ConcurrentDictionary<Type, IDictionary<string, string>> _enumToDictionaryCache =
        new ConcurrentDictionary<Type, IDictionary<string, string>>();

    /// <summary>
    /// 将一个枚举对象转换为字典
    /// </summary>
    /// <typeparam name="TEnum">枚举对象类型</typeparam>
    /// <returns></returns>
    public static IDictionary<string, string> EnumToDictionary<TEnum>() where TEnum : Enum
    {
        var type = typeof(TEnum);

        return _enumToDictionaryCache.GetOrAdd(type, BuildEnumDictionary);
    }

    /// <summary>
    /// 构建枚举字典
    /// </summary>
    /// <param name="enumType">枚举类型</param>
    /// <returns></returns>
    private static IDictionary<string, string> BuildEnumDictionary(Type enumType)
    {
        var fields = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
        var dictionary = new Dictionary<string, string>();

        foreach (var field in fields)
        {
            var descriptionAttribute = field.GetCustomAttribute<DescriptionAttribute>();
            var key = field.Name;
            var value = descriptionAttribute != null ? descriptionAttribute.Description : "No description";

            dictionary.Add(key, value);
        }

        return dictionary;
    }
}