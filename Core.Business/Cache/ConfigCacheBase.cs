using Core.Business.Contracts;
using Core.Entities;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Business.Cache
{
    public abstract class ConfigCacheBase : IConfigCache
    {
        public ConfigCacheBase(IHostEnvironment env, string prefix = "ConfigCache")
        {
            this.Environment = env?.EnvironmentName?.ToLowerInvariant() ?? throw new ArgumentNullException(nameof(env));
            this.Prefix = string.IsNullOrEmpty(prefix) ? "ConfigCache" : prefix;
        }

        public virtual string Prefix { get; protected set; }

        protected virtual string Environment { get; set; }

        public abstract Task<TResult> GetAsync<TResult>(SessionInfo session, string keyName, Func<Task<TResult>> lookup) where TResult : IConfigEntry;
        public abstract Task<IEnumerable<TResult>> GetAsync<TResult>(SessionInfo session, string keyName, Func<Task<IEnumerable<TResult>>> lookup) where TResult : IConfigEntry;
        public abstract Task<IEnumerable<TResult>> GetGroupAsync<TResult>(SessionInfo session, string groupName, Func<Task<IEnumerable<TResult>>> lookup) where TResult : IConfigEntry;
        public abstract Task<IEnumerable<TResult>> GetUserAsync<TResult>(SessionInfo session, Func<Task<IEnumerable<TResult>>> lookup) where TResult : IConfigEntry;
        public abstract Task<IEnumerable<TResult>> GetVirtualGroupAsync<TResult>(SessionInfo session, IEnumerable<string> keys, Func<Task<IEnumerable<TResult>>> lookup) where TResult : IConfigEntry;
        public abstract Task ResetCacheAsync();
        public abstract Task ResetCacheGroupsOnlyAsync();

        protected virtual string BuildKey(params object[] parts) => string.Join(":", parts).ToLowerInvariant();
        protected virtual string BuildKey(SessionInfo session, string keyName) => BuildKey(Prefix, Environment, session.ApplicationName, session.DomainName, session.UserName, keyName);
    }
}
