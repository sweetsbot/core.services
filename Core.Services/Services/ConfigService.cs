using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using Core.Contracts;
using Core.Entities;
using Core.Extensions;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Core.Services
{
    public class ConfigService : Config.ConfigBase
    {
        private readonly ILogger<ConfigService> _logger;
        private readonly IConfigLogic _configLogic;
        private readonly ICacheResettable[] _resettableItems;

        public ConfigService(ILogger<ConfigService> logger, 
            IEnumerable<ICacheResettable> resettableItems,
            IConfigLogic configLogic)
        {
            _logger = logger;
            _resettableItems = resettableItems.Where(p => p is IConfigLogic).ToArray();
            _configLogic = configLogic;
        }

        [Authorize]
        public override async Task<GetSettingRes> GetSetting(GetSettingReq request, ServerCallContext context)
        {
            var session = context.GetHttpContext().User.AsSessionInfo();
            var entries = await _configLogic.GetSettingsAsync(session, request.KeyNames);
            return entries == null ? new GetSettingRes() : new GetSettingRes { Settings = { entries.AsSettings().ToArray() } };
        }

        [Authorize]
        public override async Task<SetSettingRes> AddSetting(SetSettingReq request, ServerCallContext context)
        {
            try
            {
                var session = context.GetHttpContext().User.AsSessionInfo();
                await _configLogic.AddSettingAsync(session, request);
                return new SetSettingRes { Success = true };
            }
            catch (Exception ex) when (ex is not RpcException)
            {
                _logger.LogError(ex, "Failed to add an entry to config.");
                throw new RpcException(new Status(StatusCode.PermissionDenied, ex.Message));
            }
        }

        [Authorize]
        public override async Task<GetUserConfigRes> GetUserConfig(GetUserConfigReq userConfig, ServerCallContext context)
        {
            var session = context.GetHttpContext().User.AsSessionInfo();
            var entries = await _configLogic.GetUserConfigurationAsync(session);
            return new GetUserConfigRes { Config = new ConfigEntries { Settings = { entries.Select(Setting.FromEntry) } } };
        }

        [Authorize(Roles = "Developer,AllTasks")]
        public override async Task<ResetCacheRes> ResetCache(ResetCacheReq _, ServerCallContext context)
        {
            try
            {
                var session = context.GetHttpContext().User.AsSessionInfo();
                _logger.LogInformation($"User {session.ToBlameString()} is resetting the cache.");
                await Task.WhenAll(_resettableItems.Select(p => p.ResetCacheAsync(session)).ToArray());
                return new ResetCacheRes();
            }
            catch (SecurityException ex)
            {
                _logger.LogError(ex, "Failed to reset cache.");
                throw new RpcException(new Status(StatusCode.PermissionDenied, ex.Message));
            }
            catch (Exception ex) when (ex is not RpcException)
            {
                _logger.LogError(ex, "Failed to reset cache.");
                throw new RpcException(new Status(StatusCode.Aborted, ex.Message));
            }
        }

        [Authorize]
        public override async Task<GetGroupConfigRes> GetGroupConfig(GetGroupConfigReq request, ServerCallContext context)
        {
            var session = context.GetHttpContext().User.AsSessionInfo();
            _logger.LogDebug($"{session.UserName} has requested a group of settings.");
            var entries = await _configLogic.GetGroupConfigurationAsync(session, request.GroupName);
            return new GetGroupConfigRes { Config = new ConfigEntries { Settings = { entries.Select(Setting.FromEntry) } } };
        }

        [Authorize]
        public override async Task<SetGroupSettingRes> AddGroupSetting(SetGroupSettingReq request, ServerCallContext context)
        {
            var session = context.GetHttpContext().User.AsSessionInfo();
            SetGroupSettingRes result = null;
            try
            {
                var keyStatus = await _configLogic.AddGroupConfigurationAsync(session, request).ContinueWith(p => p.Result.ToArray());
                result = new SetGroupSettingRes
                {
                    Success = !keyStatus.Any() || keyStatus.All(p => p.Success),
                    GroupName = request.GroupName,
                    Results = { keyStatus.Select(p => new SetGroupSettingKeyRes { Key = p.Key, Success = p.Success }) }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "failed to set group settings.");
                result = new SetGroupSettingRes
                {
                    Success = false,
                    GroupName = request.GroupName
                };
            }

            return result;
        }
    }
}