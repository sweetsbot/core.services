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
        private const string Prefix = "ConfigEntry";

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

        public CacheConfigManager(
            IHostEnvironment env,
            ConfigManager configManager,
            IRedisCacheClient cacheClient,
            ILogger<CacheConfigManager> logger = null) :
            this(env, configManager as IConfigManager, cacheClient, logger)
        {
        }

        private string GetCacheKeyBase(ClaimsPrincipal clientData, string suffix = null) =>
            $"{Prefix}:" +
            $"{_env.EnvironmentName}:{clientData.Application()}:" +
            $"{clientData.Identity.Name}{(!string.IsNullOrEmpty(suffix) ? ":" + suffix : "")}";

        private async Task<IEnumerable<TCollection>> GetCachedRawAsync<TCollection>(string cacheKey,
            Func<Task<IEnumerable<TCollection>>> lookup)
        {
            if (lookup == null) throw new ArgumentNullException(nameof(lookup));
            if (string.IsNullOrEmpty(cacheKey))
                throw new ArgumentException("Value cannot be null or empty.", nameof(cacheKey));
            try
            {
                _logger.LogTrace($"Connected to cache for key {cacheKey}");
                var entry = await _cacheClient.Db0.GetAsync<List<TCollection>>(cacheKey, CommandFlags.PreferSlave);
                if (entry != null) return entry;
                _logger.LogTrace($"Cache miss for key {cacheKey}");
                entry = (await lookup()).ToList();
                await _cacheClient.Db0.AddAsync(cacheKey, entry, TimeSpan.FromHours(24), When.NotExists,
                    CommandFlags.DemandMaster);
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

        public async Task<ConfigEntrySlim> GetRawSettingAsync(ClaimsPrincipal user, string key)
        {
            return await GetCachedRawAsync(GetCacheKeyBase(user, key),
                () => _configManager.GetRawSettingAsync(user, key));
        }

        public async Task<IEnumerable<ConfigEntrySlim>> GetRawGroupConfigurationAsync(ClaimsPrincipal user,
            string groupName)
        {
            return await GetCachedRawAsync(GetCacheKeyBase(user, groupName),
                () => _configManager.GetRawGroupConfigurationAsync(user, groupName));
        }

        public async Task<IEnumerable<ConfigEntrySlim>> GetRawUserConfigurationAsync(ClaimsPrincipal user)
        {
            return await GetCachedRawAsync(GetCacheKeyBase(user),
                () => _configManager.GetRawUserConfigurationAsync(user));
        }

        public async Task ResetCacheAsync(ClaimsPrincipal user)
        {
            try
            {
                var keys = await _cacheClient.Db0.SearchKeysAsync(
                    $"{Prefix}:{_env.EnvironmentName}:*");
                var batches = keys
                    .Select((k, i) => new {Key = (RedisKey) k, Index = i})
                    .GroupBy(k => k.Index % 500, k => k.Key)
                    .Select(k => k.ToArray());
                await Task.WhenAll(batches.Select(p =>
                    _cacheClient.Db0.Database.KeyDeleteAsync(p, CommandFlags.DemandMaster)));
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

        public Task<ConfigEntrySlim> GetSettingAsync(ClaimsPrincipal user, string key)
        {
            return GetRawSettingAsync(user, key);
        }

        public Task<IEnumerable<ConfigEntrySlim>> GetUserConfigurationAsync(ClaimsPrincipal user)
        {
            return GetRawUserConfigurationAsync(user);
        }

        public Task<IEnumerable<ConfigEntrySlim>> GetGroupConfigurationAsync(ClaimsPrincipal user, string groupName)
        {
            return GetRawGroupConfigurationAsync(user, groupName);
        }

        public Task AddSettingAsync(ClaimsPrincipal user, SetSetting request)
        {
            return _configManager.AddSettingAsync(user, request);
        }
    }
}