using System;
using Microsoft.Extensions.DependencyInjection;

namespace Idler.Common.Core.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// 建立对象
        /// </summary>
        /// <param name="services"></param>
        /// <param name="name">名称</param>
        /// <typeparam name="TType"></typeparam>
        public static TType Resolve<TType>(this IServiceProvider services, string name = "")
        {
            if (services == null)
                return default(TType);

            return services.Resolve<TType>(name);
        }
    }
}