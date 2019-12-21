using System;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using Core.Business;
using Core.Business.Extensions;
using Core.Common;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Core.Services
{
    public class ConfigService : Config.ConfigBase
    {
        private readonly ILogger<ConfigService> _logger;
        private readonly IConfigManager _configManager;

        public ConfigService(ILogger<ConfigService> logger, IConfigManager configManager)
        {
            _logger = logger;
            _configManager = configManager;
        }

        [Authorize]
        public override async Task<Setting> GetSetting(Key request, ServerCallContext context)
        {
            var user = context.GetHttpContext().User;
            var entry = await _configManager.GetSettingAsync(user, request.Value);
            return entry == null ? Setting.Null(request) : Setting.FromEntry(entry);
        }

        [Authorize]
        public override async Task<ConfigBlob> GetSettings(Keys request, ServerCallContext context)
        {
            var user = context.GetHttpContext().User;
            var entries = await _configManager.GetSettingsAsync(user, request.Values);
            return new ConfigBlob {Settings = {entries.Select(Setting.FromEntry)}};
        }

        [Authorize]
        public override async Task<BoolValue> AddSetting(SetSetting request, ServerCallContext context)
        {
            try
            {
                var user = context.GetHttpContext().User;
                await _configManager.AddSettingAsync(user, request);
                return new BoolValue {Value = true};
            }
            catch (Exception ex) when (!(ex is RpcException))
            {
                _logger.LogError(ex, "Failed to add an entry to config.");
                throw new RpcException(new Status(StatusCode.PermissionDenied, ex.Message));
            }
        }

        [Authorize]
        public override async Task<ConfigBlob> GetUserConfig(Empty _, ServerCallContext context)
        {
            var user = context.GetHttpContext().User;
            var entries = await _configManager.GetUserConfigurationAsync(user);
            return new ConfigBlob {Settings = {entries.Select(Setting.FromEntry)}};
        }

        [Authorize(Roles = "Developer,AllTasks")]
        public override async Task<Empty> ResetCache(Empty _, ServerCallContext context)
        {
            try
            {
                var user = context.GetHttpContext().User;
                _logger.LogInformation($"User {user.ToBlameString()} is resetting the cache.");
                await _configManager.ResetCacheAsync(user);
                return new Empty();
            }
            catch (SecurityException ex)
            {
                _logger.LogError(ex, "Failed to reset cache.");
                throw new RpcException(new Status(StatusCode.PermissionDenied, ex.Message));
            }
            catch (Exception ex) when (!(ex is RpcException))
            {
                _logger.LogError(ex, "Failed to reset cache.");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        [Authorize]
        public override async Task<ConfigBlob> GetGroupConfig(Key request, ServerCallContext context)
        {
            var user = context.GetHttpContext().User;
            _logger.LogDebug($"{user.Identity.Name} has requested a group of settings.");
            var entries = await _configManager.GetGroupConfigurationAsync(user, request.Value);
            return new ConfigBlob {Settings = {entries.Select(Setting.FromEntry)}};
        }

        [Authorize]
        public override async Task<SetGroupSettingResult> AddGroupSetting(SetGroupSetting request, ServerCallContext context)
        {
            var user = context.GetHttpContext().User;
            SetGroupSettingResult result = null;
            try
            {
                var keyStatus = (await _configManager.AddGroupConfigurationAsync(user, request)).ToArray();
                result = new SetGroupSettingResult
                {
                    Success = !keyStatus.Any() || keyStatus.All(p => p.Success),
                    GroupName = request.GroupName,
                    Results = {keyStatus.Select(p => new SetGroupSettingKeyResult {Key = p.Key, Success = p.Success})}
                };
            }
            catch
            {
                result = new SetGroupSettingResult
                {
                    Success = false,
                    GroupName = request.GroupName
                };
            }

            return result;
        }
    }
}