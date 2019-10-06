using Autofac;
using Core.Common;
using Core.Encryption;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.Core.Abstractions;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Core.Implementations;
using StackExchange.Redis.Extensions.Newtonsoft;

namespace Core.Business.Module
{
    public class BusinessModule : Autofac.Module
    {
        public string SecretKey { get; set; }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.Register(ctx => new AesEncryptionProvider(SecretKey))
                .As<IEncryptionProvider>()
                .InstancePerDependency();
            builder.RegisterType<ConfigManager>()
                .Named<IConfigManager>("RawAccess")
                .InstancePerDependency();
            builder.RegisterType<CacheConfigManager>()
                .WithParameter(
                    (info, context) => info.ParameterType == typeof(IConfigManager),
                    (info, context) => context.ResolveNamed<IConfigManager>("RawAccess"))
                .As<IConfigManager>()
                .InstancePerDependency();
        }
    }
}