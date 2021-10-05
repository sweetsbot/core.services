using Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Business.Contracts
{
    public interface IConfigCache
    {
        string Prefix { get; }
        Task<TResult> GetCachedAsync<TResult>(ISessionInfo session, string keyName, Func<Task<TResult>> lookup)
            where TResult : IConfigEntry;
        Task<IEnumerable<TResult>> GetCachedAsync<TResult>(ISessionInfo session, string keyName, Func<Task<IEnumerable<TResult>>> lookup)
            where TResult : IConfigEntry;
        Task<IEnumerable<TResult>> GetVirtualGroupAsync<TResult>(ISessionInfo session, IEnumerable<string> keys, Func<Task<IEnumerable<TResult>>> lookup)
            where TResult : IConfigEntry;
        Task<IEnumerable<TResult>> GetGroupAsync<TResult>(ISessionInfo session, string groupName, Func<Task<IEnumerable<TResult>>> lookup)
            where TResult : IConfigEntry;
        Task<IEnumerable<TResult>> GetUserAsync<TResult>(ISessionInfo session, Func<Task<IEnumerable<TResult>>> lookup)
            where TResult : IConfigEntry;
        Task ResetCacheGroupsOnlyAsync();
        Task ResetCacheAsync();
    }
}
