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
        private readonly IEncryptionProvider _encryptor;

        public EncryptionConfigManager(
            IConfigManager configManager,
            IEncryptionProvider encryptionProvider,
            ILogger<EncryptionConfigManager> logger = null)
        {
            this._configManager = configManager ?? throw new ArgumentNullException(nameof(configManager));
            this._encryptor = encryptionProvider ?? throw new ArgumentNullException(nameof(encryptionProvider));
            this._logger = logger;
        }

        [DebuggerStepThrough]
        Task IConfigManager.ResetCacheAsync(ClaimsPrincipal user) => _configManager.ResetCacheAsync(user);

        [DebuggerStepThrough]
        public Task<ConfigEntrySlim> GetSettingAsync(ClaimsPrincipal user, string key) =>
            _configManager.GetSettingAsync(user, key).Decrypt(_encryptor, _logger);

        [DebuggerStepThrough]
        public Task<IEnumerable<ConfigEntrySlim>> GetUserConfigurationAsync(ClaimsPrincipal user) =>
            _configManager.GetUserConfigurationAsync(user).Decrypt(_encryptor, _logger);

        [DebuggerStepThrough]
        public Task<IEnumerable<ConfigEntrySlim>> GetGroupConfigurationAsync(ClaimsPrincipal user, string groupName) =>
            _configManager.GetGroupConfigurationAsync(user, groupName).Decrypt(_encryptor, _logger);

        public Task AddSettingAsync(ClaimsPrincipal user, SetSetting request)
        {
            return _configManager.AddSettingAsync(user, request);
        }
    }
}