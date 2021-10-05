using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Contracts;
using Core.Entities;
using Core.Extensions;
using Microsoft.Extensions.Hosting;

namespace Core.Business
{
    public class ConfigLogic : IConfigLogic
    {
        private readonly string _environment;
        private readonly IConfigRepository _repository;

        public ConfigLogic(
            IHostEnvironment env,
            IConfigRepository configRepository)
        {
            this._environment = env?.EnvironmentName.ToLowerInvariant() ?? throw new ArgumentNullException(nameof(env));
            this._repository = configRepository ?? throw new ArgumentNullException(nameof(configRepository));
        }
        
        public async Task<ConfigEntrySlim> GetSettingAsync(ISessionInfo user, string key) =>
            await _repository.GetWeightedConfigEntryByKeyAsync(
                _environment,
                user.ApplicationName,
                user.DomainName,
                user.UserName,
                key.ToLowerInvariant());

        public async Task<IEnumerable<ConfigEntrySlim>> GetSettingsAsync(ISessionInfo user, IEnumerable<string> keys) => 
            await GetUserConfigurationAsync(user).ContinueWith(p => p.Result.Where(s => keys.Contains(s.ConfigKeyName, StringComparer.OrdinalIgnoreCase)));

        public async Task<IEnumerable<ConfigEntrySlim>> GetUserConfigurationAsync(ISessionInfo user) =>
            await _repository.GetWeightedConfigEntriesAsync(
                _environment,
                user.ApplicationName,
                user.DomainName,
                user.UserName);

        public async Task<IEnumerable<ConfigEntrySlim>> GetGroupConfigurationAsync(ISessionInfo user, string groupName) =>
            await _repository.GetWeightedConfigEntryByGroupAsync(
                _environment,
                user.ApplicationName,
                user.DomainName,
                user.UserName,
                groupName.ToLowerInvariant());

        public Task<IEnumerable<(string Key, bool Success)>> AddGroupConfigurationAsync(ISessionInfo user, SetGroupSettingReq groupConfig)
        {
            return Task.FromResult<IEnumerable<(string Key, bool Success)>>(Array.Empty<(string, bool)>());
        }

        public async Task<bool> AddSettingAsync(ISessionInfo user, SetSettingReq request)
        {
            var entry = new ConfigEntry
            {
                ConfigKeyName = request.Key,
                IsEncrypted = request.Encrypt,
                ConfigValue = request.Type == SettingType.Null ? null : request.Value,
                ConfigValueType = request.Type.ToConfigValueType(),
                Application = string.IsNullOrEmpty(request.Application) ? null : request.Application,
                DomainName = string.IsNullOrEmpty(request.Domain) ? null : request.Domain,
                Environment = string.IsNullOrEmpty(request.Environment) ? _environment : request.Environment,
                UserName = string.IsNullOrEmpty(request.UserName) ? null : request.UserName,
                Active = true,
                CreatedBy = user.ToBlameString(),
                UpdatedBy = user.ToBlameString()
            };

            try
            {
                entry = await _repository.InsertOrUpdateConfigEntryAsync(entry);
                return entry != null;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }
    }
}