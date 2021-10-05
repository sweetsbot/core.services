using Autofac;
using Core.Contracts;
using Core.DataAccess;

namespace Core.Business.Bootstrapping
{
    public class DataAccessModule : Autofac.Module
    {
        public string CoreConnectionString { get; set; }
        
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.Register(ctx => new ConfigRepository(CoreConnectionString))
                .As<IConfigRepository>()
                .InstancePerDependency();
        }
    }
}