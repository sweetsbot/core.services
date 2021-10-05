using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using Core.Business.Extensions;
using Core.Contracts;
using Core.Entities;
using Microsoft.Extensions.Logging;

namespace Core.Business
{
    public class EncryptionConfigLogic : IConfigLogic
    {
        private readonly IConfigLogic _configLogic;
        private readonly ILogger<EncryptionConfigLogic> _logger;
        private readonly IEncryptionProvider _provider;

        public EncryptionConfigLogic(
            IConfigLogic configLogic,
            IEncryptionProvider provider,
            ILogger<EncryptionConfigLogic> logger = null)
        {
            this._configLogic = configLogic ?? throw new ArgumentNullException(nameof(configLogic));
            this._provider = provider ?? throw new ArgumentNullException(nameof(provider));
            this._logger = logger;
        }

        [DebuggerStepThrough]
        public Task<ConfigEntrySlim> GetSettingAsync(ISessionInfo session, string key) =>
            _configLogic.GetSettingAsync(session, key).Decrypt(_provider, _logger);

        [DebuggerStepThrough]
        public Task<IEnumerable<ConfigEntrySlim>> GetSettingsAsync(ISessionInfo session, IEnumerable<string> keys) =>
            _configLogic.GetSettingsAsync(session, keys).Decrypt(_provider, _logger);

        [DebuggerStepThrough]
        public Task<IEnumerable<ConfigEntrySlim>> GetUserConfigurationAsync(ISessionInfo session) =>
            _configLogic.GetUserConfigurationAsync(session).Decrypt(_provider, _logger);

        [DebuggerStepThrough]
        public Task<IEnumerable<ConfigEntrySlim>> GetGroupConfigurationAsync(ISessionInfo session, string groupName) =>
            _configLogic.GetGroupConfigurationAsync(session, groupName).Decrypt(_provider, _logger);

        public Task<bool> AddSettingAsync(ISessionInfo session, SetSettingReq request)
        {
            if (request.Encrypt && request.Type != SettingType.Null)
                request.Value = _provider.Encrypt(request.Value);
            return _configLogic.AddSettingAsync(session, request);
        }

        [DebuggerStepThrough]
        public Task<IEnumerable<(string Key, bool Success)>> AddGroupConfigurationAsync(ISessionInfo session, SetGroupSettingReq groupConfig) =>
            _configLogic.AddGroupConfigurationAsync(session, groupConfig);
    }
}