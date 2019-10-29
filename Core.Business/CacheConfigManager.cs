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
        private readonly IConfigManager _configManager;
        private readonly ILogger<CacheConfigManager> _logger;
        private readonly IHostEnvironment _env;

        public CacheConfigManager(
            IHostEnvironment env,
            IConfigManager configManager,
            IRedisCacheClient cacheClient,
            ILogger<CacheConfigManager> logger = null)
        {
            this._env = env ?? throw new ArgumentNullException(nameof(env));
            this._configManager = configManager ?? throw new ArgumentNullException(nameof(configManager));
            this._cacheClient = cacheClient ?? throw new ArgumentNullException(nameof(cacheClient));
            this._logger = logger;
        }

        public string Prefix { get; set; } = "ConfigService";
        public string Environment => _env.EnvironmentName;
        
        public async Task<ConfigEntrySlim> GetSettingAsync(ClaimsPrincipal user, string key) =>
            await GetCachedRawAsync(user, key, () => _configManager.GetSettingAsync(user, key));

        public async Task<IEnumerable<ConfigEntrySlim>> GetGroupConfigurationAsync(ClaimsPrincipal user, string groupName) =>
            await GetCachedRawAsync(user, $"__group_{groupName}", () => _configManager.GetGroupConfigurationAsync(user, groupName));

        public async Task<IEnumerable<ConfigEntrySlim>> GetUserConfigurationAsync(ClaimsPrincipal user) =>
            await GetCachedRawAsync(user, "__user", () => _configManager.GetUserConfigurationAsync(user));

        public async Task ResetCacheAsync(ClaimsPrincipal user)
        {
            try
            {
                var keys = await _cacheClient.Db0.SearchKeysAsync(
                    BuildCacheKey(Prefix, Environment, "*"));
                var batches = keys
                    .Select((k, i) => (Key:(RedisKey) k, Index: i))
                    .GroupBy(k => k.Index / 500, k => k.Key)
                    .Select(k => k.ToArray());
                await Task.WhenAll(batches.Select(p =>
                    _cacheClient.GetDbFromConfiguration().Database.KeyDeleteAsync(p, CommandFlags.DemandMaster)));
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
        
        Task IConfigManager.AddSettingAsync(ClaimsPrincipal user, SetSetting request) => _configManager.AddSettingAsync(user, request);
        
        private static string BuildCacheKey(params object[] parts) => string.Join(":", parts).ToLowerInvariant();
        private string GetCacheKeyBase(ClaimsPrincipal c, string suffix) => 
            BuildCacheKey(Prefix, Environment, c.Application(), c.DomainName(), c.UserName(), suffix);
        private async Task<ConfigEntrySlim> GetCachedRawAsync(ClaimsPrincipal user, string name, Func<Task<ConfigEntrySlim>> lookup)
        {
            Guard.ThrowIfNull(user, nameof(user));
            Guard.ThrowIfNull(name, nameof(name));
            Guard.ThrowIfNull(lookup, nameof(lookup));

            try
            {
                var key = GetCacheKeyBase(user, name);
                _logger.LogTrace($"Looking for cache key {key}");
                var entry = await _cacheClient.Db0.GetAsync<ConfigEntrySlim>(key);
                if (entry is null)
                {
                    _logger.LogTrace($"Cache miss for cache key {key}");
                    entry = await lookup();
                    await _cacheClient.Db0.AddAsync(key, entry, When.NotExists);
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
                var key = GetCacheKeyBase(user, name);
                var keys = await _cacheClient.Db0.GetAsync<string[]>(key);
                if (keys is null)
                {
                    values = await lookup();
                    keys = values.Select(p => GetCacheKeyBase(user, p.ConfigKeyName)).ToArray();
                    await _cacheClient.Db0.AddAsync(key, keys, When.NotExists);
                    await Task.WhenAll(values.Select(p => _cacheClient.Db0.AddAsync(GetCacheKeyBase(user, p.ConfigKeyName), p, When.NotExists)));
                }
                else
                {
                    var map = await _cacheClient.Db0.GetAllAsync<ConfigEntrySlim>(keys);
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