using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Core.Common;
using Core.Entities;

namespace Core.Business
{
    public interface IConfigManager
    {
        /// <summary>
        /// Resets the whole cache of the manager
        /// </summary>
        /// <param name="user">The ClaimsPrincipal with Application, DomainName, and UserName claims</param>
        /// <returns></returns>
        Task ResetCacheAsync(ClaimsPrincipal user);
        Task<ConfigEntrySlim> GetSettingAsync(ClaimsPrincipal user, string key);
        Task<IEnumerable<ConfigEntrySlim>> GetUserConfigurationAsync(ClaimsPrincipal user);
        Task<IEnumerable<ConfigEntrySlim>> GetGroupConfigurationAsync(ClaimsPrincipal user, string groupName);
        Task AddSettingAsync(ClaimsPrincipal user, SetSetting request);
    }
}