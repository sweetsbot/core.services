using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using Core.Business.Extensions;
using Core.Common;
using Core.Entities;
using Microsoft.Extensions.Logging;

namespace Core.Business
{
    public class EncryptionConfigManager : IConfigManager
    {
        private readonly IConfigManager _configManager;
        private readonly ILogger<EncryptionConfigManager> _logger;
        private readonly IEncryptionProvider _provider;

        public EncryptionConfigManager(
            IConfigManager configManager,
            IEncryptionProvider provider,
            ILogger<EncryptionConfigManager> logger = null)
        {
            this._configManager = configManager ?? throw new ArgumentNullException(nameof(configManager));
            this._provider = provider ?? throw new ArgumentNullException(nameof(provider));
            this._logger = logger;
        }

        [DebuggerStepThrough]
        Task IConfigManager.ResetCacheAsync(ClaimsPrincipal user) => _configManager.ResetCacheAsync(user);

        [DebuggerStepThrough]
        public Task<ConfigEntrySlim> GetSettingAsync(ClaimsPrincipal user, string key) =>
            _configManager.GetSettingAsync(user, key).Decrypt(_provider, _logger);

        [DebuggerStepThrough]
        public Task<IEnumerable<ConfigEntrySlim>> GetSettingsAsync(ClaimsPrincipal user, IEnumerable<string> keys) =>
            _configManager.GetSettingsAsync(user, keys).Decrypt(_provider, _logger);

        [DebuggerStepThrough]
        public Task<IEnumerable<ConfigEntrySlim>> GetUserConfigurationAsync(ClaimsPrincipal user) =>
            _configManager.GetUserConfigurationAsync(user).Decrypt(_provider, _logger);

        [DebuggerStepThrough]
        public Task<IEnumerable<ConfigEntrySlim>> GetGroupConfigurationAsync(ClaimsPrincipal user, string groupName) =>
            _configManager.GetGroupConfigurationAsync(user, groupName).Decrypt(_provider, _logger);

        public Task<bool> AddSettingAsync(ClaimsPrincipal user, SetSetting request)
        {
            if (request.Encrypt && request.Type != SettingType.Null)
                request.Value = _provider.Encrypt(request.Value);
            return _configManager.AddSettingAsync(user, request);
        }

        [DebuggerStepThrough]
        public Task<IEnumerable<(string Key, bool Success)>> AddGroupConfigurationAsync(ClaimsPrincipal user, SetGroupSetting groupConfig) =>
            _configManager.AddGroupConfigurationAsync(user, groupConfig);
    }
}