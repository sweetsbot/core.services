using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.Contracts
{
    public interface IConfigRepository
    {
        #region SlimConfigEntry by Weight
        IEnumerable<ConfigEntrySlim> GetWeightedConfigEntries(string environment,
            string application,
            string domainName,
            string userName);

        Task<IEnumerable<ConfigEntrySlim>> GetWeightedConfigEntriesAsync(string environment,
            string application,
            string domainName,
            string userName);

        ConfigEntrySlim GetWeightedConfigEntryByKey(string environment,
            string application,
            string domainName,
            string userName,
            string configKeyName);

        Task<ConfigEntrySlim> GetWeightedConfigEntryByKeyAsync(string environment,
            string application,
            string domainName,
            string userName,
            string configKeyName);
        List<ConfigEntrySlim> GetWeightedConfigEntryByGroup(string environment,
            string application,
            string domainName,
            string userName,
            string groupName);

        Task<IEnumerable<ConfigEntrySlim>> GetWeightedConfigEntryByGroupAsync(string environment,
            string application,
            string domainName,
            string userName,
            string groupName);
        #endregion
        
        #region ConfigEntry By Weight
        IEnumerable<ConfigEntry> GetFullWeightedConfigEntries(string environment,
            string application,
            string domainName,
            string userName);

        Task<IEnumerable<ConfigEntry>> GetFullWeightedConfigEntriesAsync(string environment,
            string application,
            string domainName,
            string userName);

        ConfigEntry GetFullWeightedConfigEntryByKey(string environment,
            string application,
            string userName,
            string domainName,
            string configKeyName);

        Task<ConfigEntry> GetFullWeightedConfigEntryByKeyAsync(string environment,
            string application,
            string userName,
            string domainName,
            string configKeyName);
        #endregion

        #region Sync
        bool ConfigKeyExists(string configKeyName);
        List<ConfigKey> GetAllConfigKeys();
        ConfigKey GetConfigKeyById(int configKeyId);
        ConfigKey GetConfigKeyByName(string configKeyName);
        ConfigEntry GetConfigEntryById(int configEntryId);
        ConfigEntry InsertOrUpdateConfigEntry(ConfigEntry entry);
        ConfigEntry UpdateConfigEntry(ConfigEntry entry);
        void DeleteConfigEntry(int configEntryId);
        void DeleteConfigEntry(ConfigEntry entry);
        #endregion
        
        #region Async
        Task<bool> ConfigKeyExistsAsync(string configKeyName);
        Task<List<ConfigKey>> GetAllConfigKeysAsync();
        Task<ConfigKey> GetConfigKeyByIdAsync(int configKeyId);
        Task<ConfigKey> GetConfigKeyByNameAsync(string configKeyName);
        Task<ConfigEntry> GetConfigEntryByIdAsync(int configEntryId);
        Task<ConfigEntry> InsertOrUpdateConfigEntryAsync(ConfigEntry entry);
        Task<ConfigEntry> UpdateConfigEntryAsync(ConfigEntry entry);
        Task DeleteConfigEntryAsync(int configEntryId);
        Task DeleteConfigEntryAsync(ConfigEntry entry);
        #endregion
    }
}