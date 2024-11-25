using System.Reflection;
using Idler.Common.Attachments;
using Idler.Common.Core.Upload;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Idler.Common.Attachments.OBS
{
    public static class OBSExtensions
    {
        /// <summary>
        /// 注册OBS服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddOBS(this IServiceCollection services, ConfigurationManager configuration)
        {
            configuration.AddJsonFile("Config/UploadConfig.json", optional: false)
                .AddJsonFile("Config/OBSSetting.json");
            services.Configure<UploadConfig>(configuration.GetSection("UploadConfig"));
            services.Configure<OBSSetting>(configuration.GetSection("OBSSetting"));
            services.TryAddSingleton<IOBSDomainService, OBSDomainService>();
            services.TryAddSingleton<IAttachmentDomainService, OBSAttachmentDomainService>();
            return services;
        }
    }
}