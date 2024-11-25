using Idler.Common.Core.Mail;
using Idler.Common.Core.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Idler.Common.ServiceBus
{
    /// <summary>
    /// 注册FluentEmail
    /// </summary>
    public static class ServiceBusDaprExtensions
    {
        public static IServiceCollection AddServiceBus(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DaprServiceBusSetting>(configuration.GetSection("ServiceBusSetting"));
            services.TryAddSingleton<IServiceBus, ServiceBus>();
            return services;
        }
    }
}