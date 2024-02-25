using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Idler.Common.Core.Mail
{
    /// <summary>
    /// 注册FluentEmail
    /// </summary>
    public static class FluentEmailExtensions
    {
        public static IServiceCollection AddFluentEmail(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<EmailSetting>(configuration.GetSection("EmailSetting"));
            services.TryAddSingleton<IEmailSender, FluentEmailSender>();
            return services;
        }
    }
}