using System.Collections;
using System.Collections.Generic;
using Autofac;
using Idler.Common.Authorization.Caches;
using Idler.Common.Authorization.Config;
using Idler.Common.Authorization.Services;
using Idler.Common.Cache;
using Idler.Common.Cache.FreeRedis;
using Idler.Common.Core;
using Idler.Common.Core.Config;
using Module = Autofac.Module;

namespace Idler.Common.Authorization
{
    public class AuthorizationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<SimpleRedisCacheManager<string>>()
                .As<ISimpleCacheManager<string>>()
                .WithParameter("settingKey", "NotOverdue")
                .SingleInstance();

            builder
                .RegisterType<SimpleRedisCacheManager<IList<CoreRoleValue>>>()
                .As<ISimpleCacheManager<IList<CoreRoleValue>>>()
                .WithParameter("settingKey", "NotOverdue")
                .SingleInstance();

            builder
                .RegisterType<SimpleRedisCacheManager<RoleScopeCacheValue>>()
                .As<ISimpleCacheManager<RoleScopeCacheValue>>()
                .WithParameter("settingKey", "NotOverdue")
                .SingleInstance();

            builder
                .RegisterType<SimpleRedisCacheManager<IList<CorePermissionValue>>>()
                .As<ISimpleCacheManager<IList<CorePermissionValue>>>()
                .WithParameter("settingKey", "NotOverdue")
                .SingleInstance();

            builder
                .RegisterType<SimpleRedisCacheManager<IList<CoreResourceValue>>>()
                .As<ISimpleCacheManager<IList<CoreResourceValue>>>()
                .WithParameter("settingKey", "NotOverdue")
                .SingleInstance();

            builder.RegisterType<LocalConfigurationAccessHelper<AuthorizationConfig>>()
                .As<IConfigAccessHelper<AuthorizationConfig>>()
                .WithParameter("ConfigName", "AuthorizationConfig")
                .SingleInstance();
            
            builder.RegisterType<LocalConfigurationAccessHelper<AuthorizationBasicData>>()
                .As<IConfigAccessHelper<AuthorizationBasicData>>()
                .WithParameter("ConfigName", "AuthorizationBasicData")
                .SingleInstance();
            
            builder.RegisterType<AuthorizationService>().As<IAuthorizationService>().AsImplementedInterfaces();
            // builder.RegisterType<ICoreUserRoleRepository>().As<CoreUserRoleRepository>();
        }
    }
}