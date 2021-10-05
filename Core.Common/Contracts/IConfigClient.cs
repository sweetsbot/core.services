using System;
using System.Threading;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Core.Entities;
namespace Core.Contracts
{
    public interface IConfigClient
    {
        //Setting GetSetting(Key request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default);
        //Setting GetSetting(Key request, CallOptions options);
        //AsyncUnaryCall<Setting> GetSettingAsync(Key request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default);
        //AsyncUnaryCall<Setting> GetSettingAsync(Key request, CallOptions options);
        //ConfigEntries GetGroupConfig(Key request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default);
        //ConfigEntries GetGroupConfig(Key request, CallOptions options);
        //AsyncUnaryCall<ConfigEntries> GetGroupConfigAsync(Key request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default);
        //AsyncUnaryCall<ConfigEntries> GetGroupConfigAsync(Key request, CallOptions options);
        //ConfigEntries GetUserConfig(Empty request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default);
        //ConfigEntries GetUserConfig(Empty request, CallOptions options);
        //AsyncUnaryCall<ConfigEntries> GetUserConfigAsync(Empty request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default);
        //AsyncUnaryCall<ConfigEntries> GetUserConfigAsync(Empty request, CallOptions options);
        //Empty ResetCache(Empty request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default);
        //Empty ResetCache(Empty request, CallOptions options);
        //AsyncUnaryCall<Empty> ResetCacheAsync(Empty request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default);
        //AsyncUnaryCall<Empty> ResetCacheAsync(Empty request, CallOptions options);
        //SetSettingRes AddSetting(SetSettingReq request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default);
        //SetSettingRes AddSetting(SetSettingReq request, CallOptions options);
        //AsyncUnaryCall<SetSettingRes> AddSettingAsync(SetSettingReq request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default);
        //AsyncUnaryCall<SetSettingRes> AddSettingAsync(SetSettingReq request, CallOptions options);
        //Setting GetSetting(string key, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default);
        //Setting GetSetting(string key, CallOptions options);
        //AsyncUnaryCall<Setting> GetSettingAsync(string key, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default);
        //AsyncUnaryCall<Setting> GetSettingAsync(string key, CallOptions options);
        //ConfigEntries GetGroupConfig(string groupName, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default);
        //ConfigEntries GetGroupConfig(string groupName, CallOptions options);
        //AsyncUnaryCall<ConfigEntries> GetGroupConfigAsync(string groupName, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default);
        //AsyncUnaryCall<ConfigEntries> GetGroupConfigAsync(string groupName, CallOptions options);
        //ConfigEntries GetUserConfig(Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default);
        //ConfigEntries GetUserConfig(CallOptions options);
        //AsyncUnaryCall<ConfigEntries> GetUserConfigAsync(Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default);
        //AsyncUnaryCall<ConfigEntries> GetUserConfigAsync(CallOptions options);
        //Empty ResetCache(Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default);
        //Empty ResetCache(CallOptions options);
        //AsyncUnaryCall<Empty> ResetCacheAsync(Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default);
        //AsyncUnaryCall<Empty> ResetCacheAsync(CallOptions options);
    }
}