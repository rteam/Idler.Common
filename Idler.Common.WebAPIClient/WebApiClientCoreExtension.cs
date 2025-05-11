using System;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using WebApiClientCore.Serialization.JsonConverters;

namespace Idler.Common.WebAPIClient;

public static class WebApiClientCoreExtension
{
    /// <summary>
    /// 简易注册API
    /// </summary>
    /// <param name="services"></param>
    /// <param name="baseUrl"></param>
    /// <typeparam name="TAPIValue"></typeparam>
    /// <returns></returns>
    public static IServiceCollection BaseAddAPI<TAPIValue>(this IServiceCollection services, string baseUrl)
        where TAPIValue : class
    {
        var builder = services.AddHttpApi<TAPIValue>()
            .ConfigureHttpApi(t =>
            {
                t.HttpHost = new Uri(baseUrl);
                t.UseParameterPropertyValidate = false;
                t.UseReturnValuePropertyValidate = false;
                t.JsonSerializeOptions.WriteIndented = false;
                t.JsonSerializeOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            })
            .ConfigureHttpApi(o =>
            {
                o.JsonSerializeOptions.Converters.Add(new JsonDateTimeConverter("yyyy-MM-dd HH:mm:ss"));
            });

        return services;
    }
}