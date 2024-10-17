using System.Reflection;
using Idler.Common.Attachments;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Idler.Common.OBS
{
    public static class OBSExtensions
    {
        /// <summary>
        /// 注册OBS服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddOBS(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<OBSSetting>(configuration.GetSection("OBSSetting"));
            services.TryAddSingleton<IOBSDomainService, OBSDomainService>();
            services.TryAddSingleton<IAttachmentDomainService, OBSAttachmentDomainService>();
            return services;
        }
    }
}