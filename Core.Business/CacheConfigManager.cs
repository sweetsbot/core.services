using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Core.Business.Extensions;
using Core.Common;
using Core.Entities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace Core.Business
{
    public class CacheConfigManager : IConfigManager
    {
        private readonly IRedisCacheClient _cacheClient;
        private readonly IConfigManager _innerManager;
        private readonly ILogger<CacheConfigManager> _logger;
        private readonly IHostEnvironment _env;

        public CacheConfigManager(
            IHostEnvironment env,
            IConfigManager innerManager,
            IRedisCacheClient cacheClient,
            ILogger<CacheConfigManager> logger = null)
        {
            this._env = env ?? throw new ArgumentNullException(nameof(env));
            this._innerManager = innerManager ?? throw new ArgumentNullException(nameof(innerManager));
            this._cacheClient = cacheClient ?? throw new ArgumentNullException(nameof(cacheClient));
            this._logger = logger;
        }

        public string Prefix { get; set; } = "ConfigService";
        private string Environment => _env.EnvironmentName;
        private IRedisDatabase Cache => _cacheClient.GetDbFromConfiguration();
        
        public async Task<ConfigEntrySlim> GetSettingAsync(ClaimsPrincipal user, string key) =>
            await GetCachedRawAsync(user, key, () => _innerManager.GetSettingAsync(user, key));

        public async Task<IEnumerable<ConfigEntrySlim>> GetGroupConfigurationAsync(ClaimsPrincipal user, string groupName) =>
            await GetCachedRawAsync(user, $"__group_{groupName}", () => _innerManager.GetGroupConfigurationAsync(user, groupName));

        public async Task<IEnumerable<ConfigEntrySlim>> GetUserConfigurationAsync(ClaimsPrincipal user) =>
            await GetCachedRawAsync(user, "__user", () => _innerManager.GetUserConfigurationAsync(user));

        public async Task ResetCacheAsync(ClaimsPrincipal user)
        {
            try
            {
                var keys = await Cache.SearchKeysAsync(BuildCacheKey(Prefix, Environment, "*"));
                var batches = keys
                    .Select((k, i) => (Index: i, Key:(RedisKey) k))
                    .GroupBy(t => t.Index / 500, t => t.Key)
                    .Select(k => k.ToArray())
                    .Select(p => Cache.Database.KeyDeleteAsync(p, CommandFlags.DemandMaster));
                await Task.WhenAll(batches);
            }
            catch (Exception ex)
            {
                if (ex is AggregateException aggEx && aggEx.InnerExceptions.Any(p => p is RedisConnectionException))
                {
                    ex = aggEx.InnerExceptions.First(p => p is RedisConnectionException);
                }

                _logger?.LogError(ex, $"Failed to use cache due to {ex.GetType()}::{ex}");
            }
        }
        Task IConfigManager.AddSettingAsync(ClaimsPrincipal user, SetSetting request) => _innerManager.AddSettingAsync(user, request);
        private static string BuildCacheKey(params object[] parts) => string.Join(":", parts).ToLowerInvariant();
        private string BuildCacheKey(ClaimsPrincipal c, string suffix) => BuildCacheKey(Prefix, Environment, c.Application(), c.DomainName(), c.UserName(), suffix);
        private async Task<ConfigEntrySlim> GetCachedRawAsync(ClaimsPrincipal user, string name, Func<Task<ConfigEntrySlim>> lookup)
        {
            Guard.ThrowIfNull(user, nameof(user));
            Guard.ThrowIfNull(name, nameof(name));
            Guard.ThrowIfNull(lookup, nameof(lookup));

            try
            {
                var key = BuildCacheKey(user, name);
                _logger.LogTrace($"Looking for cache key {key}");
                var entry = await Cache.GetAsync<ConfigEntrySlim>(key);
                if (entry is null)
                {
                    _logger.LogTrace($"Cache miss for cache key {key}");
                    entry = await lookup();
                    await Cache.AddAsync(key, entry, When.NotExists);
                }
                return entry;
            }
            catch (Exception ex)
            {
                if (ex is AggregateException aggEx && aggEx.InnerExceptions.Any(p => p is RedisConnectionException))
                {
                    ex = aggEx.InnerExceptions.First(p => p is RedisConnectionException);
                }

                _logger?.LogError(ex, $"Failed to use cache due to {ex.GetType()}::{ex}");
                return await lookup();
            }
        }
        private async Task<IEnumerable<ConfigEntrySlim>> GetCachedRawAsync(ClaimsPrincipal user, string name, Func<Task<IEnumerable<ConfigEntrySlim>>> lookup)
        {
            Guard.ThrowIfNull(user, nameof(user));
            Guard.ThrowIfNull(name, nameof(name));
            Guard.ThrowIfNull(lookup, nameof(lookup));

            try
            {
                IEnumerable<ConfigEntrySlim> values = null;
                var key = BuildCacheKey(user, name);
                var keys = await Cache.GetAsync<string[]>(key);
                if (keys is null)
                {
                    values = await lookup();
                    keys = values.Select(p => BuildCacheKey(user, p.ConfigKeyName)).ToArray();
                    await Cache.AddAsync(key, keys, When.NotExists);
                    await Task.WhenAll(values.Select(p => Cache.AddAsync(BuildCacheKey(user, p.ConfigKeyName), p, When.NotExists)));
                }
                else
                {
                    var map = await Cache.GetAllAsync<ConfigEntrySlim>(keys);
                    values = map.Values;
                }
                return values;
            }
            catch (Exception ex)
            {
                if (ex is AggregateException aggEx && aggEx.InnerExceptions.Any(p => p is RedisConnectionException))
                {
                    ex = aggEx.InnerExceptions.First(p => p is RedisConnectionException);
                }
                _logger?.LogError(ex, $"Failed to use cache due to {ex.GetType()}::{ex}");
                return await lookup();
            }
        }
    }
}