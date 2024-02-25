using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Autofac;
using Autofac.Configuration;

namespace Idler.Common.DependencyInjection
{
    public static class AutofacExtensions
    {
        /// <summary>
        /// 注册Autofac
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static void AddAutofac(this ContainerBuilder builder)
        {
            var config = new ConfigurationBuilder();
            config.AddJsonFile("Autofac.json");
            builder.RegisterModule(new ConfigurationModule(config.Build()));
        }
    }
}