using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.Contracts
{
    public interface IConfigLogic
    {
        /// <summary>
        /// Resets the whole cache of the manager
        /// </summary>
        /// <param name="session">The ISessionInfo with Application, DomainName, and UserName</param>
        /// <returns></returns>
        Task<ConfigEntrySlim> GetSettingAsync(ISessionInfo session, string key);
        Task<IEnumerable<ConfigEntrySlim>> GetSettingsAsync(ISessionInfo session, IEnumerable<string> keys);
        Task<IEnumerable<ConfigEntrySlim>> GetUserConfigurationAsync(ISessionInfo session);
        Task<IEnumerable<ConfigEntrySlim>> GetGroupConfigurationAsync(ISessionInfo session, string groupName);
        Task<bool> AddSettingAsync(ISessionInfo session, SetSettingReq request);
        Task<IEnumerable<(string Key, bool Success)>> AddGroupConfigurationAsync(ISessionInfo session, SetGroupSettingReq groupConfig);
    }

    public interface ICacheResettable
    {
        Task ResetCacheAsync(ISessionInfo session);
    }
}