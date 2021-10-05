using System;
using System.Threading;
using Core.Entities;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

// ReSharper disable once CheckNamespace
namespace Core.Services
{
    public static partial class Config {
        public partial class ConfigClient : Contracts.IConfigClient
        {
            //public Setting GetSetting(string key, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default)
            //{
            //    return GetSetting(new GetSettingReq {KeyName = key}, new CallOptions(headers, deadline, cancellationToken));
            //}

            //public Setting GetSetting(string key, CallOptions options)
            //{
            //    return GetSetting(new Key {Value = key}, options);
            //}

            //public AsyncUnaryCall<Setting> GetSettingAsync(string key, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default)
            //{
            //    return GetSettingAsync(new Key {Value = key}, new CallOptions(headers, deadline, cancellationToken));
            //}

            //public AsyncUnaryCall<Setting> GetSettingAsync(string key, CallOptions options)
            //{
            //    return GetSettingAsync(new Key {Value = key}, options);
            //}

            //public ConfigEntries GetGroupConfig(string groupName, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default)
            //{
            //    return GetGroupConfig(new Key {Value = groupName}, new CallOptions(headers, deadline, cancellationToken));
            //}

            //public ConfigEntries GetGroupConfig(string groupName, CallOptions options)
            //{
            //    return GetGroupConfig(new Key {Value = groupName}, options);
            //}

            //public AsyncUnaryCall<ConfigEntries> GetGroupConfigAsync(string groupName, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default)
            //{
            //    return GetGroupConfigAsync(new Key {Value = groupName}, new CallOptions(headers, deadline, cancellationToken));
            //}

            //public AsyncUnaryCall<ConfigEntries> GetGroupConfigAsync(string groupName, CallOptions options)
            //{
            //    return GetGroupConfigAsync(new Key {Value = groupName}, options);
            //}

            //public ConfigEntries GetUserConfig(Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default)
            //{
            //    return GetUserConfig(new Empty(), new CallOptions(headers, deadline, cancellationToken));
            //}

            //public ConfigEntries GetUserConfig(CallOptions options)
            //{
            //    return GetUserConfig(new Empty(), options);
            //}

            //public AsyncUnaryCall<ConfigEntries> GetUserConfigAsync(Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default)
            //{
            //    return GetUserConfigAsync(new Empty(), new CallOptions(headers, deadline, cancellationToken));
            //}

            //public AsyncUnaryCall<ConfigEntries> GetUserConfigAsync(CallOptions options)
            //{
            //    return GetUserConfigAsync(new Empty(), options);
            //}

            //public Empty ResetCache(Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default)
            //{
            //    return ResetCache(new Empty(), new CallOptions(headers, deadline, cancellationToken));
            //}

            //public Empty ResetCache(CallOptions options)
            //{
            //    return ResetCache(new Empty(), options);
            //}

            //public AsyncUnaryCall<Empty> ResetCacheAsync(Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default)
            //{
            //    return ResetCacheAsync(new Empty(), new CallOptions(headers, deadline, cancellationToken));
            //}

            //public AsyncUnaryCall<Empty> ResetCacheAsync(CallOptions options)
            //{
            //    return ResetCacheAsync(new Empty(), options);
            //}
        }
    }
}