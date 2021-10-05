using Autofac;
using Core.Contracts;
using Core.Encryption;

namespace Core.Business.Bootstrapping
{
    public class BusinessModule : Autofac.Module
    {
        public string SecretKey { get; set; }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(ctx => new AesEncryptionProvider(SecretKey))
                .As<IEncryptionProvider>()
                .InstancePerDependency();
            builder.RegisterType<Cache.RedisConfigCache>()
                .As<Contracts.IConfigCache>()
                .InstancePerDependency();
            builder.RegisterType<ConfigLogic>()
                .Named<IConfigLogic>("DatabaseConfig")
                .InstancePerDependency();
            builder.RegisterType<CacheConfigLogic>()
                .WithParameter(
                    (info, context) => info.ParameterType == typeof(IConfigLogic),
                    (info, context) => context.ResolveNamed<IConfigLogic>("DatabaseConfig"))
                .Named<IConfigLogic>("CachedConfig")
                .As<ICacheResettable>()
                .InstancePerDependency();
            builder.RegisterType<EncryptionConfigLogic>()
                .WithParameter(
                    (info, context) => info.ParameterType == typeof(IConfigLogic),
                    (info, context) => context.ResolveNamed<IConfigLogic>("CachedConfig"))
                .AsImplementedInterfaces()
                .InstancePerDependency();
        }
    }
}