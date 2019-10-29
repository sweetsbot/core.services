using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Core.Common;
using Core.Entities;

namespace Core.Business
{
    public interface IConfigManager
    {
        Task ResetCacheAsync(ClaimsPrincipal user);
        Task<ConfigEntrySlim> GetSettingAsync(ClaimsPrincipal user, string key);
        Task<IEnumerable<ConfigEntrySlim>> GetUserConfigurationAsync(ClaimsPrincipal user);
        Task<IEnumerable<ConfigEntrySlim>> GetGroupConfigurationAsync(ClaimsPrincipal user, string groupName);
        Task AddSettingAsync(ClaimsPrincipal user, SetSetting request);
    }
}