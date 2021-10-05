using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Business.Contracts;
using Core.Contracts;
using Core.Entities;

namespace Core.Business
{
    public class CacheConfigLogic : IConfigLogic, ICacheResettable
    {
        private readonly IConfigLogic _configLogic;
        private readonly IConfigCache _cache;

        public CacheConfigLogic(
            IConfigLogic innerLogic,
            IConfigCache cache)
        {
            this._configLogic = innerLogic ?? throw new ArgumentNullException(nameof(innerLogic));
            this._cache = cache;
        }

        public async Task<ConfigEntrySlim> GetSettingAsync(ISessionInfo session, string key) =>
            await _cache.GetCachedAsync(session, key, () => _configLogic.GetSettingAsync(session, key));

        public async Task<IEnumerable<ConfigEntrySlim>> GetSettingsAsync(ISessionInfo session, IEnumerable<string> keys)
        {
            var enumerable = keys as string[] ?? keys.ToArray();
            return await _cache.GetVirtualGroupAsync(session, enumerable, () => _configLogic.GetSettingsAsync(session, enumerable));
        }

        public async Task<IEnumerable<ConfigEntrySlim>> GetGroupConfigurationAsync(ISessionInfo session, string groupName) =>
            await _cache.GetGroupAsync(session, groupName, () => _configLogic.GetGroupConfigurationAsync(session, groupName));

        public async Task<IEnumerable<ConfigEntrySlim>> GetUserConfigurationAsync(ISessionInfo session) =>
            await _cache.GetUserAsync(session, () => _configLogic.GetUserConfigurationAsync(session));

        public async Task<bool> AddSettingAsync(ISessionInfo session, SetSettingReq request)
        {
            var result = await _configLogic.AddSettingAsync(session, request);
            await _cache.ResetCacheAsync();
            return result;
        }

        public async Task<IEnumerable<(string Key, bool Success)>> AddGroupConfigurationAsync(ISessionInfo session, SetGroupSettingReq groupConfig)
        {
            try
            {
                var results = await _configLogic.AddGroupConfigurationAsync(session, groupConfig);
                await _cache.ResetCacheGroupsOnlyAsync();
                return results;
            }
            catch
            {
                return groupConfig.Keys.Select(p => (p, false)).ToArray();
            }
        }

        public async Task ResetCacheAsync(ISessionInfo session)
        {
            await _cache.ResetCacheAsync();
        }
    }
}