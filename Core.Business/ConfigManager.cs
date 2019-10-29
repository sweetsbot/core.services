using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Core.Business.Extensions;
using Core.Common;
using Core.Entities;
using Microsoft.Extensions.Hosting;

namespace Core.Business
{
    public class ConfigManager : IConfigManager
    {
        private readonly IHostEnvironment _env;
        private readonly IConfigRepository _configRepository;

        public ConfigManager(
            IHostEnvironment env,
            IConfigRepository configRepository)
        {
            this._env = env ?? throw new ArgumentNullException(nameof(env));
            this._configRepository = configRepository ?? throw new ArgumentNullException(nameof(configRepository));
        }

        public async Task<ConfigEntrySlim> GetSettingAsync(ClaimsPrincipal user, string key) =>
            await _configRepository.GetWeightedConfigEntryByKeyAsync(
                _env.EnvironmentName.ToLowerInvariant(),
                user.Application(),
                user.DomainName(),
                user.UserName(),
                key);

        Task IConfigManager.ResetCacheAsync(ClaimsPrincipal user) => Task.CompletedTask;

        public async Task<IEnumerable<ConfigEntrySlim>> GetUserConfigurationAsync(ClaimsPrincipal user) =>
            await _configRepository.GetWeightedConfigEntriesAsync(
                _env.EnvironmentName.ToLowerInvariant(),
                user.Application(),
                user.DomainName(),
                user.UserName());

        public async Task<IEnumerable<ConfigEntrySlim>> GetGroupConfigurationAsync(ClaimsPrincipal user, string groupName) =>
            await _configRepository.GetWeightedConfigEntryByGroupAsync(
                _env.EnvironmentName.ToLowerInvariant(),
                user.Application(),
                user.DomainName(),
                user.UserName(),
                groupName);
        
        public  Task AddSettingAsync(ClaimsPrincipal user, SetSetting request)
        {
//            if (!await _configRepository.ConfigKeyExistsAsync(request.Key))
//            {
//                var key = new ConfigKey
//                {
//                    ConfigKeyName = request.Key, Active = true, CreatedAt = DateTime.UtcNow,
//                    CreatedBy = user.Identity.Name
//                };
//                // TODO: Insert setting or update value
////                await configDal.InsertConfigKeyAsync();
//            }
            return Task.CompletedTask;
        }
    }
}