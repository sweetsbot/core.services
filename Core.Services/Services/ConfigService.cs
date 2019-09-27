using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Common;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Core.Services
{
    public class ConfigService : Config.ConfigBase
    {
        private readonly ILogger<ConfigService> _logger;

        public ConfigService(ILogger<ConfigService> logger)
        {
            _logger = logger;
        }
        
        [Authorize]
        public override Task<Setting> GetSetting(Key request, ServerCallContext context)
        {
            var user = context.GetHttpContext().User;
            _logger.LogDebug($"{user.Identity.Name} has requested a setting.");
            return Task.FromResult(new Setting()
            {
                Key = request.Value,
                Value = $"{request.Value}.value",
                Type = SettingType.String
            });
        }

        [Authorize]
        public override Task<BoolValue> AddSetting(SetSetting request, ServerCallContext context)
        {
            var user = context.GetHttpContext().User;
            _logger.LogDebug($"{user.Identity.Name} has added a setting.");
            return Task.FromResult(new BoolValue() {Value = false});
        }

        [Authorize]
        public override Task<ConfigBlob> GetUserConfig(Empty request, ServerCallContext context)
        {
            var user = context.GetHttpContext().User;
            var name = user.Identity.Name;
            _logger.LogDebug($"{name} has requested a group of settings for their user.");
            return Task.FromResult(new ConfigBlob()
            {
                Settings =
                {
                    new Setting {Key = $"setting.key.{name}.1", Value = "setting.value.1", Type = SettingType.String},
                    new Setting {Key = $"setting.key.{name}.2", Value = "setting.value.2", Type = SettingType.String},
                    new Setting {Key = $"setting.key.{name}.3", Value = "setting.value.3", Type = SettingType.String},
                    new Setting {Key = $"setting.key.{name}.4", Value = "setting.value.4", Type = SettingType.String},
                }
            });
        }

        [Authorize]
        public override Task<Empty> ResetCache(Empty request, ServerCallContext context)
        {
            var user = context.GetHttpContext().User;
            _logger.LogDebug($"{user.Identity.Name} has requested a cache reset.");
            return Task.FromResult(new Empty());
        }

        [Authorize]
        public override Task<ConfigBlob> GetGroupConfig(Key request, ServerCallContext context)
        {
            var user = context.GetHttpContext().User;
            _logger.LogDebug($"{user.Identity.Name} has requested a group of settings.");
            return Task.FromResult(new ConfigBlob()
            {
                Settings =
                {
                    new Setting {Key = $"setting.key.{request.Value}.1", Value = "setting.value.1", Type = SettingType.String},
                    new Setting {Key = $"setting.key.{request.Value}.2", Value = "setting.value.2", Type = SettingType.String},
                    new Setting {Key = $"setting.key.{request.Value}.3", Value = "setting.value.3", Type = SettingType.String},
                    new Setting {Key = $"setting.key.{request.Value}.4", Value = "setting.value.4", Type = SettingType.String},
                }
            });
        }
    }
}