using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Core.Business.Contracts;
using Core.Contracts;
using Core.Entities;
using Core.Extensions;
using Microsoft.Extensions.Logging;

namespace Core.Business
{
    public class CacheConfigManager : IConfigManager
    {
        private readonly IConfigManager configManager;
        private readonly IConfigCache cache;

        public CacheConfigManager(
            IConfigManager innerManager,
            IConfigCache cache)
        {
            this.configManager = innerManager ?? throw new ArgumentNullException(nameof(innerManager));
            this.cache = cache;
        }

        public async Task<ConfigEntrySlim> GetSettingAsync(ClaimsPrincipal user, string key) =>
            await cache.GetAsync(user.AsSessionInfo(), key, () => configManager.GetSettingAsync(user, key));

        public async Task<IEnumerable<ConfigEntrySlim>> GetSettingsAsync(ClaimsPrincipal user, IEnumerable<string> keys)
        {
            keys = keys.ToArray();
            return await cache.GetVirtualGroupAsync(user.AsSessionInfo(), keys, () => configManager.GetSettingsAsync(user, keys));
        }

        public async Task<IEnumerable<ConfigEntrySlim>> GetGroupConfigurationAsync(ClaimsPrincipal user, string groupName) =>
            await cache.GetGroupAsync(user.AsSessionInfo(), groupName, () => configManager.GetGroupConfigurationAsync(user, groupName));

        public async Task<IEnumerable<ConfigEntrySlim>> GetUserConfigurationAsync(ClaimsPrincipal user) =>
            await cache.GetUserAsync(user.AsSessionInfo(), () => configManager.GetUserConfigurationAsync(user));

        public async Task<bool> AddSettingAsync(ClaimsPrincipal user, SetSetting request)
        {
            var result = await configManager.AddSettingAsync(user, request);
            await cache.ResetCacheAsync();
            return result;
        }

        public async Task<IEnumerable<(string Key, bool Success)>> AddGroupConfigurationAsync(ClaimsPrincipal user, SetGroupSetting groupConfig)
        {
            try
            {
                var results = await configManager.AddGroupConfigurationAsync(user, groupConfig);
                await cache.ResetCacheGroupsOnlyAsync();
                return results;
            }
            catch
            {
                return groupConfig.Keys.Select(p => (p, false)).ToArray();
            }
        }

        public async Task ResetCacheAsync(ClaimsPrincipal user)
        {
            await cache.ResetCacheAsync();
            await configManager.ResetCacheAsync(user);
        }
    }
}