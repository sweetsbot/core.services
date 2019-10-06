using Autofac;
using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.Core.Abstractions;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Core.Implementations;
using StackExchange.Redis.Extensions.Newtonsoft;

namespace Core.Business.Module
{
    public class RedisModule : Autofac.Module
    {
        public RedisConfiguration RedisConfiguration { get; set; }
        
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterInstance(RedisConfiguration)
                .SingleInstance();
            builder.RegisterType<NewtonsoftSerializer>()
                .As<ISerializer>()
                .SingleInstance();
            builder.RegisterType<RedisCacheClient>()
                .As<IRedisCacheClient>()
                .SingleInstance();
            builder.RegisterType<RedisCacheConnectionPoolManager>()
                .As<IRedisCacheConnectionPoolManager>()
                .SingleInstance();
            builder.RegisterType<RedisDefaultCacheClient>()
                .As<IRedisDefaultCacheClient>()
                .SingleInstance();
        }
    }
}