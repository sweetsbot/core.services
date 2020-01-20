using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Core.Contracts;
using Core.Entities;
using Core.Extensions;
using Microsoft.Extensions.Hosting;

namespace Core.Business
{
    public class ConfigManager : IConfigManager
    {
        private readonly string environment;
        private readonly IConfigRepository repository;

        public ConfigManager(
            IHostEnvironment env,
            IConfigRepository configRepository)
        {
            this.environment = env?.EnvironmentName.ToLowerInvariant() ?? throw new ArgumentNullException(nameof(env));
            this.repository = configRepository ?? throw new ArgumentNullException(nameof(configRepository));
        }

        
        public async Task<ConfigEntrySlim> GetSettingAsync(ClaimsPrincipal user, string key) =>
            await repository.GetWeightedConfigEntryByKeyAsync(
                environment,
                user.Application(),
                user.DomainName(),
                user.UserName(),
                key);

        public async Task<IEnumerable<ConfigEntrySlim>> GetSettingsAsync(ClaimsPrincipal user, IEnumerable<string> keys) => 
            await GetUserConfigurationAsync(user).ContinueWith(p => p.Result.Where(s => keys.Contains(s.ConfigKeyName, StringComparer.OrdinalIgnoreCase)));

        Task IConfigManager.ResetCacheAsync(ClaimsPrincipal user) => Task.CompletedTask;

        public async Task<IEnumerable<ConfigEntrySlim>> GetUserConfigurationAsync(ClaimsPrincipal user) =>
            await repository.GetWeightedConfigEntriesAsync(
                environment,
                user.Application(),
                user.DomainName(),
                user.UserName());

        public async Task<IEnumerable<ConfigEntrySlim>> GetGroupConfigurationAsync(ClaimsPrincipal user, string groupName) =>
            await repository.GetWeightedConfigEntryByGroupAsync(
                environment,
                user.Application(),
                user.DomainName(),
                user.UserName(),
                groupName);

        public Task<IEnumerable<(string Key, bool Success)>> AddGroupConfigurationAsync(ClaimsPrincipal user, SetGroupSetting groupConfig)
        {
            return Task.FromResult<IEnumerable<(string Key, bool Success)>>(new ValueTuple<string, bool>[0]);
        }

        public async Task<bool> AddSettingAsync(ClaimsPrincipal user, SetSetting request)
        {
            var entry = new ConfigEntry
            {
                ConfigKeyName = request.Key,
                IsEncrypted = request.Encrypt,
                ConfigValue = request.Type == SettingType.Null ? null : request.Value,
                ConfigValueType = request.Type.ToConfigValueType(),
                Application = string.IsNullOrEmpty(request.Application) ? null : request.Application,
                DomainName = string.IsNullOrEmpty(request.Domain) ? null : request.Domain,
                Environment = string.IsNullOrEmpty(request.Environment) ? environment : request.Environment,
                UserName = string.IsNullOrEmpty(request.UserName) ? null : request.UserName,
                Active = true,
                CreatedBy = user.ToBlameString(),
                UpdatedBy = user.ToBlameString(),
            };
            try
            {
                entry = await repository.InsertOrUpdateConfigEntryAsync(entry);
                return entry != null;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }
    }
}