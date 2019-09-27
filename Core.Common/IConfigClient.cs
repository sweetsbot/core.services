using System;
using System.Threading;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace Core.Common
{
    public interface IConfigClient
    {
        Setting GetSetting(Key request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default);
        Setting GetSetting(Key request, CallOptions options);
        AsyncUnaryCall<Setting> GetSettingAsync(Key request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default);
        AsyncUnaryCall<Setting> GetSettingAsync(Key request, CallOptions options);
        ConfigBlob GetGroupConfig(Key request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default);
        ConfigBlob GetGroupConfig(Key request, CallOptions options);
        AsyncUnaryCall<ConfigBlob> GetGroupConfigAsync(Key request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default);
        AsyncUnaryCall<ConfigBlob> GetGroupConfigAsync(Key request, CallOptions options);
        ConfigBlob GetUserConfig(Empty request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default);
        ConfigBlob GetUserConfig(Empty request, CallOptions options);
        AsyncUnaryCall<ConfigBlob> GetUserConfigAsync(Empty request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default);
        AsyncUnaryCall<ConfigBlob> GetUserConfigAsync(Empty request, CallOptions options);
        Empty ResetCache(Empty request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default);
        Empty ResetCache(Empty request, CallOptions options);
        AsyncUnaryCall<Empty> ResetCacheAsync(Empty request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default);
        AsyncUnaryCall<Empty> ResetCacheAsync(Empty request, CallOptions options);
        BoolValue AddSetting(SetSetting request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default);
        BoolValue AddSetting(SetSetting request, CallOptions options);
        AsyncUnaryCall<BoolValue> AddSettingAsync(SetSetting request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default);
        AsyncUnaryCall<BoolValue> AddSettingAsync(SetSetting request, CallOptions options);
        Setting GetSetting(string key, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default);
        Setting GetSetting(string key, CallOptions options);
        AsyncUnaryCall<Setting> GetSettingAsync(string key, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default);
        AsyncUnaryCall<Setting> GetSettingAsync(string key, CallOptions options);
        ConfigBlob GetGroupConfig(string groupName, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default);
        ConfigBlob GetGroupConfig(string groupName, CallOptions options);
        AsyncUnaryCall<ConfigBlob> GetGroupConfigAsync(string groupName, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default);
        AsyncUnaryCall<ConfigBlob> GetGroupConfigAsync(string groupName, CallOptions options);
        ConfigBlob GetUserConfig(Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default);
        ConfigBlob GetUserConfig(CallOptions options);
        AsyncUnaryCall<ConfigBlob> GetUserConfigAsync(Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default);
        AsyncUnaryCall<ConfigBlob> GetUserConfigAsync(CallOptions options);
        Empty ResetCache(Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default);
        Empty ResetCache(CallOptions options);
        AsyncUnaryCall<Empty> ResetCacheAsync(Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default);
        AsyncUnaryCall<Empty> ResetCacheAsync(CallOptions options);
    }
}