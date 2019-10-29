using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Core.Common;
using Core.Entities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Core.Business
{
    public class ConfigManager : IConfigManager
    {
        private readonly IHostEnvironment _env;
        private readonly IConfigRepository _configRepository;
        private readonly ILogger<ConfigManager> _logger;

        public ConfigManager(
            IHostEnvironment env,
            IConfigRepository configRepository,
            ILogger<ConfigManager> logger = null)
        {
            this._env = env ?? throw new ArgumentNullException(nameof(env));
            this._configRepository = configRepository ?? throw new ArgumentNullException(nameof(configRepository));
            this._logger = logger;
        }

        public Task<ConfigEntrySlim> GetSettingAsync(ClaimsPrincipal user, string key) =>
            _configRepository.GetWeightedConfigEntryByKeyAsync(
                _env.EnvironmentName.ToLowerInvariant(),
                user.Application(),
                user.DomainName(),
                user.UserName(),
                key);

        Task IConfigManager.ResetCacheAsync(ClaimsPrincipal user) => Task.CompletedTask;

        public Task<IEnumerable<ConfigEntrySlim>> GetUserConfigurationAsync(ClaimsPrincipal user) =>
            _configRepository.GetWeightedConfigEntriesAsync(
                _env.EnvironmentName.ToLowerInvariant(),
                user.Application(),
                user.DomainName(),
                user.UserName());

        public Task<IEnumerable<ConfigEntrySlim>> GetGroupConfigurationAsync(ClaimsPrincipal user, string groupName) =>
            _configRepository.GetWeightedConfigEntryByGroupAsync(
                _env.EnvironmentName.ToLowerInvariant(),
                user.Application(),
                user.DomainName(),
                user.UserName(),
                groupName);
        
        public Task AddSettingAsync(ClaimsPrincipal user, SetSetting request)
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