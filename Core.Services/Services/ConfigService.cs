using System;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using Core.Business;
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
        private readonly IConfigManager _manager;

        public ConfigService(ILogger<ConfigService> logger, IConfigManager manager)
        {
            _logger = logger;
            _manager = manager;
        }

        [Authorize]
        public override async Task<Setting> GetSetting(Key request, ServerCallContext context)
        {
            var user = context.GetHttpContext().User;
            var entry = await _manager.GetSettingAsync(user, request.Value);
            return entry == null ? Setting.Null(request) : Setting.FromEntry(entry);
        }

        [Authorize]
        public override Task<BoolValue> AddSetting(SetSetting request, ServerCallContext context)
        {
//            var user = context.GetHttpContext().User;
            return Task.FromResult(new BoolValue() {Value = false});
        }

        [Authorize]
        public override async Task<ConfigBlob> GetUserConfig(Empty _, ServerCallContext context)
        {
            var user = context.GetHttpContext().User;
            var entries = await _manager.GetUserConfigurationAsync(user);
            return new ConfigBlob {Settings = {entries.Select(Setting.FromEntry)}};
        }

        [Authorize]
        public override async Task<Empty> ResetCache(Empty _, ServerCallContext context)
        {
            try
            {
                var user = context.GetHttpContext().User;
                _logger.LogDebug($"User {user.ToBlameString()} wants to reset the cache.");
                if (!user.IsInRole("Developer") && !user.IsInRole("AllTasks"))
                {
                    _logger.LogWarning($"User {user.ToBlameString()} is not permitted to reset the cache.");
                    throw new SecurityException($"User {user.ToBlameString()} is not permitted to reset the cache.");
                }
                await _manager.ResetCacheAsync(user);
                return new Empty();
            }
            catch (Exception ex) when (!(ex is RpcException))
            {
                _logger.LogError(ex, "Failed to reset cache.");
                throw new RpcException(new Status(StatusCode.PermissionDenied, ex.Message));
            }
        }

        [Authorize]
        public override async Task<ConfigBlob> GetGroupConfig(Key request, ServerCallContext context)
        {
            var user = context.GetHttpContext().User;
            _logger.LogDebug($"{user.Identity.Name} has requested a group of settings.");
            var entries = await _manager.GetGroupConfigurationAsync(user, request.Value);
            return new ConfigBlob {Settings = {entries.Select(Setting.FromEntry)}};
        }
    }
}