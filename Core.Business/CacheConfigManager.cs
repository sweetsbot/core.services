using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Claims;
using System.Threading.Tasks;
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

        public string Prefix { get; set; } = "ConfigEntry";
        public string Environment => _env.EnvironmentName;
        
        public Task<ConfigEntrySlim> GetSettingAsync(ClaimsPrincipal user, string key) =>
            GetCachedRawAsync(GetCacheKeyBase(user, key), () => _configManager.GetSettingAsync(user, key));

        public Task<IEnumerable<ConfigEntrySlim>> GetGroupConfigurationAsync(ClaimsPrincipal user, string groupName) =>
            GetCachedRawAsync(GetCacheKeyBase(user, groupName), () => _configManager.GetGroupConfigurationAsync(user, groupName));

        public Task<IEnumerable<ConfigEntrySlim>> GetUserConfigurationAsync(ClaimsPrincipal user) =>
            GetCachedRawAsync(GetCacheKeyBase(user), () => _configManager.GetUserConfigurationAsync(user));

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
        
        private static string BuildCacheKey(params object[] parts) => string.Join(":", parts);
        private string GetCacheKeyBase(ClaimsPrincipal c) => 
            BuildCacheKey(Prefix, Environment, c.Application(), c.DomainName(), c.UserName());
        private string GetCacheKeyBase(ClaimsPrincipal c, string suffix) => 
            BuildCacheKey(Prefix, Environment, c.Application(), c.DomainName(), c.UserName(), suffix);
        private async Task<IEnumerable<TCollection>> GetCachedRawAsync<TCollection>(string cacheKey,
            Func<Task<IEnumerable<TCollection>>> lookup)
            where TCollection : IConfigEntry
        {
            if (lookup == null) throw new ArgumentNullException(nameof(lookup));
            if (string.IsNullOrEmpty(cacheKey))
                throw new ArgumentException("Value cannot be null or empty.", nameof(cacheKey));
            try
            {
                _logger.LogTrace($"Connected to cache for key {cacheKey}");
                var keys = await _cacheClient.Db0.GetAsync<List<string>>(cacheKey, CommandFlags.PreferSlave);
                if (keys != null)
                {
                    var entries = await _cacheClient.Db0.GetAllAsync<TCollection>(keys);
                    return entries.Values;
                }

                _logger.LogTrace($"Cache miss for key {cacheKey}");
                var rawEntries = (await lookup()).ToList();
                await _cacheClient.Db0.AddAsync(cacheKey, rawEntries.Select(p => p.ConfigKeyName), TimeSpan.FromHours(24), When.NotExists,
                    CommandFlags.DemandMaster);
                return rawEntries;
            }
            catch (Exception ex)
            {
                if (ex is AggregateException aggEx && aggEx.InnerExceptions.Any(p => p is RedisConnectionException))
                {
                    ex = aggEx.InnerExceptions.First(p => p is RedisConnectionException);
                }

                _logger?.LogError(ex, $"Failed to use cache due to {ex.GetType()}::{ex}");
                var entry = await lookup();
                return entry;
            }
        }

        private async Task<TResult> GetCachedRawAsync<TResult>(string cacheKey, Func<Task<TResult>> lookup)
        {
            if (typeof(TResult).IsInterface || typeof(TResult).IsAbstract)
                throw new ArgumentException("Cannot deserialize interface types via cache.");
            if (lookup == null) throw new ArgumentNullException(nameof(lookup));
            if (string.IsNullOrEmpty(cacheKey))
                throw new ArgumentException("Value cannot be null or empty.", nameof(cacheKey));
            try
            {
                _logger.LogTrace($"Connected to cache for key {cacheKey}");
                var entry = await _cacheClient.Db0.GetAsync<TResult>(cacheKey, CommandFlags.PreferSlave);
                if (entry != null) return entry;
                _logger.LogTrace($"Cache miss for key {cacheKey}");
                entry = await lookup();
                await _cacheClient.Db0.AddAsync(cacheKey, entry, TimeSpan.FromHours(24), When.NotExists);
                return entry;
            }
            catch (Exception ex)
            {
                if (ex is AggregateException aggEx && aggEx.InnerExceptions.Any(p => p is RedisConnectionException))
                {
                    ex = aggEx.InnerExceptions.First(p => p is RedisConnectionException);
                }

                _logger?.LogError(ex, $"Failed to use cache due to {ex.GetType()}::{ex}");
                var entry = await lookup();
                return entry;
            }
        }
    }
}