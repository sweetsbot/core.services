using Core.Entities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Core.Business.Cache
{
    public class RedisConfigCache : ConfigCacheBase
    {
        private readonly IRedisCacheClient cacheClient;
        private readonly ILogger<RedisConfigCache> logger;

        private IRedisDatabase Cache => cacheClient.GetDbFromConfiguration();

        public RedisConfigCache(IHostEnvironment env, IRedisCacheClient cacheClient, ILogger<RedisConfigCache> logger = null, string prefix = "ConfigCache") : base(env, prefix)
        {
            this.cacheClient = cacheClient ?? throw new ArgumentNullException(nameof(cacheClient));
            this.logger = logger;
        }

        public override async Task<TResult> GetAsync<TResult>(SessionInfo session, string keyName, Func<Task<TResult>> lookup)
        {
            Guard.ThrowIfNull(session, nameof(session));
            Guard.ThrowIfNull(keyName, nameof(keyName));
            Guard.ThrowIfNull(lookup, nameof(lookup));

            try
            {
                var key = BuildKey(session, keyName);
                logger.LogTrace($"Looking for cache key {key}");
                var entry = await Cache.GetAsync<TResult>(key);
                if (entry is null)
                {
                    logger.LogTrace($"Cache miss for cache key {key}");
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

                logger?.LogError(ex, $"Failed to use cache due to {ex.GetType()}::{ex}");
                return await lookup();
            }
        }

        public override async Task<IEnumerable<TResult>> GetAsync<TResult>(SessionInfo session, string keyName, Func<Task<IEnumerable<TResult>>> lookup)
        {
            Guard.ThrowIfNull(session, nameof(session));
            Guard.ThrowIfNull(keyName, nameof(keyName));
            Guard.ThrowIfNull(lookup, nameof(lookup));

            try
            {
                IEnumerable<TResult> values = null;
                var key = BuildKey(session, keyName);
                var keys = await Cache.GetAsync<string[]>(key);
                if (keys is null)
                {
                    values = await lookup();
                    keys = values.Select(p => BuildKey(session, p.ConfigKeyName)).ToArray();
                    await Cache.AddAsync(key, keys, When.NotExists);
                    await Task.WhenAll(values.Select(p => Cache.AddAsync(BuildKey(session, p.ConfigKeyName), p, When.NotExists)));
                }
                else
                {
                    var map = await Cache.GetAllAsync<TResult>(keys);
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
                logger?.LogError(ex, $"Failed to use cache due to {ex.GetType()}::{ex}");
                return await lookup();
            }
        }

        public override async Task ResetCacheAsync()
        {
            try
            {
                var keys = await Cache.SearchKeysAsync(BuildKey(Prefix, Environment, "*"));
                var batches = keys
                    .Select((k, i) => (Index: i, Key: (RedisKey)k))
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

                logger?.LogError(ex, $"Failed to reset cache due to {ex.GetType()}::{ex}");
            }
        }

        public override async Task ResetCacheGroupsOnlyAsync()
        {
            try
            {
                var search1 = Cache.SearchKeysAsync(BuildKey(Prefix, Environment, "*", "__group_*"));
                var search2 = Cache.SearchKeysAsync(BuildKey(Prefix, Environment, "*", "__user"));
                var results = await Task.WhenAll(search1, search2);
                var batches = results.SelectMany(p => p)
                    .Select((k, i) => (Index: i, Key: (RedisKey)k))
                    .GroupBy(p => p.Index / 300, p => p.Key)
                    .Select(p => Cache.Database.KeyDeleteAsync(p.ToArray(), CommandFlags.DemandMaster));
                await Task.WhenAll(batches);
            }
            catch (Exception ex)
            {
                if (ex is AggregateException aggEx && aggEx.InnerExceptions.Any(e => e is RedisException))
                {
                    ex = aggEx.InnerExceptions.First(e => e is RedisException);
                }

                logger?.LogError(ex, $"Failed to use cache due to {ex.GetType()}::{ex}");
            }
        }

        public override Task<IEnumerable<TResult>> GetGroupAsync<TResult>(SessionInfo session, string groupName, Func<Task<IEnumerable<TResult>>> lookup) =>
            GetAsync(session, $"__group_{groupName}", lookup);

        public override Task<IEnumerable<TResult>> GetUserAsync<TResult>(SessionInfo session, Func<Task<IEnumerable<TResult>>> lookup) =>
            GetAsync(session, "__user", lookup);

        public override Task<IEnumerable<TResult>> GetVirtualGroupAsync<TResult>(SessionInfo session, IEnumerable<string> keys, Func<Task<IEnumerable<TResult>>> lookup)
        {
            using var sha256 = SHA256.Create();
            var providedKeys = keys.ToArray();
            var onDemandGroupName = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(string.Join("|", providedKeys))));
            return GetAsync(session, $"__group_od{onDemandGroupName}", lookup);
        }
    }
}
