using Idler.Common.Attachments;
using Idler.Common.Core.Upload;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Idler.Common.Attachments.OSS
{
    public static class OSSExtensions
    {
        /// <summary>
        /// 注册OSS服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddOBS(this IServiceCollection services, ConfigurationManager configuration)
        {
            configuration.AddJsonFile("Config/UploadConfig.json", optional: false)
                .AddJsonFile("Config/OSSSetting.json");
            services.Configure<OSSSetting>(configuration.GetSection("OSSSetting"));
            services.Configure<UploadConfig>(configuration.GetSection("UploadConfig"));
            services.TryAddSingleton<IAttachmentDomainService, OSSAttachmentDomainService>();
            return services;
        }
    }
}