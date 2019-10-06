using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IEncryptionProvider _encryptionProvider;
        private readonly IConfigRepository _configRepository;
        private readonly ILogger<ConfigManager> _logger;


        public ConfigManager(
            IHostEnvironment env,
            IEncryptionProvider encryptionProvider,
            IConfigRepository configRepository,
            ILogger<ConfigManager> logger = null)
        {
            this._env = env ?? throw new ArgumentNullException(nameof(env));
            this._encryptionProvider = encryptionProvider ?? throw new ArgumentNullException(nameof(encryptionProvider));
            this._configRepository = configRepository ?? throw new ArgumentNullException(nameof(configRepository));
            this._logger = logger;
        }

        public Task<ConfigEntrySlim> GetSettingAsync(ClaimsPrincipal user, string key)
        {
            return GetRawSettingAsync(user, key);
        }

        public Task ResetCacheAsync(ClaimsPrincipal user)
        {
            return Task.CompletedTask;
        }

        public Task<IEnumerable<ConfigEntrySlim>> GetUserConfigurationAsync(ClaimsPrincipal user)
        {
            return GetRawUserConfigurationAsync(user);
        }

        public Task<IEnumerable<ConfigEntrySlim>> GetGroupConfigurationAsync(ClaimsPrincipal user, string groupName)
        {
            return GetRawGroupConfigurationAsync(user, groupName);
        }

        public async Task<ConfigEntrySlim> GetRawSettingAsync(ClaimsPrincipal user, string key)
        {
            var entry = await _configRepository.GetWeightedConfigEntryByKeyAsync(
                _env.EnvironmentName.ToLowerInvariant(),
                user.Application(),
                user.DomainName(),
                user.UserName(),
                key);
            DecryptValue(entry);
            return entry;
        }

        public Task<IEnumerable<ConfigEntrySlim>> GetRawGroupConfigurationAsync(ClaimsPrincipal user,
            string groupName)
        {
            return _configRepository.GetWeightedConfigEntryByGroupAsync(
                _env.EnvironmentName.ToLowerInvariant(),
                user.Application(),
                user.DomainName(),
                user.UserName(),
                groupName).ContinueWith(p => p.Result.Select(DecryptValue));
        }

        public Task<IEnumerable<ConfigEntrySlim>> GetRawUserConfigurationAsync(ClaimsPrincipal user)
        {
            return _configRepository.GetWeightedConfigEntriesAsync(
                    _env.EnvironmentName.ToLowerInvariant(),
                    user.Application(),
                    user.DomainName(),
                    user.UserName())
                .ContinueWith(p => p.Result.Select(DecryptValue));
        }

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

        private ConfigEntrySlim DecryptValue(ConfigEntrySlim entry)
        {
            if (entry == null || !entry.IsEncrypted) return entry;
            try
            {
                entry.ConfigValue = _encryptionProvider.Decrypt(entry.ConfigValue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while decrypting value.");
            }

            return entry;
        }
    }
}