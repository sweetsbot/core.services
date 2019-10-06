using Autofac;
using Core.Common;
using Core.DataAccess;

namespace Core.Business.Module
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