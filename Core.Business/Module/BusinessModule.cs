using Autofac;
using Core.Contracts;
using Core.Encryption;

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
                .Named<IConfigManager>("DatabaseConfig")
                .InstancePerDependency();
            builder.RegisterType<CacheConfigManager>()
                .WithParameter(
                    (info, context) => info.ParameterType == typeof(IConfigManager),
                    (info, context) => context.ResolveNamed<IConfigManager>("DatabaseConfig"))
                .Named<IConfigManager>("CachedConfig")
                .InstancePerDependency();
            builder.RegisterType<EncryptionConfigManager>()
                .WithParameter(
                    (info, context) => info.ParameterType == typeof(IConfigManager),
                    (info, context) => context.ResolveNamed<IConfigManager>("CachedConfig"))
                .AsImplementedInterfaces()
                .InstancePerDependency();
        }
    }
}