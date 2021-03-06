using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Core.Contracts;
using Core.Entities;
using Tpl = System.Threading.Tasks;
using Dapper;
using Npgsql;

namespace Core.DataAccess
{
    public class ConfigRepository : IConfigRepository
    {
        private readonly string _connectionString;

        public ConfigRepository(string connectionString)
        {
            this._connectionString = connectionString;
        }

        protected virtual DbConnection GetConnection()
        {
            var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            return connection;
        }

        protected virtual async Tpl.Task<DbConnection> GetConnectionAsync()
        {
            var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }

        #region Weighted
        public virtual IEnumerable<ConfigEntrySlim> GetWeightedConfigEntries(string environment,
            string application,
            string domainName,
            string userName)
        {
            using (var conn = GetConnection())
            {
                var values = conn.Query<ConfigEntrySlim>(Constants.Sql.StoredProcGetConfigWeighted,
                    new
                    {
                        penvironment = environment.ToLowerInvariant(),
                        papplication = application.ToLowerInvariant(),
                        pdomainname = domainName.ToLowerInvariant(),
                        pusername = userName.ToLowerInvariant(),
                    },
                    commandTimeout: 120,
                    commandType: CommandType.StoredProcedure);
                return values.AsList();
            }
        }

        public virtual async Tpl.Task<IEnumerable<ConfigEntrySlim>> GetWeightedConfigEntriesAsync(string environment,
            string application,
            string domainName,
            string userName)
        {
            using (var conn = await GetConnectionAsync())
            {
                var values = await conn.QueryAsync<ConfigEntrySlim>(Constants.Sql.StoredProcGetConfigWeighted,
                    new
                    {
                        penvironment = environment.ToLowerInvariant(),
                        papplication = application.ToLowerInvariant(),
                        pdomainname = domainName.ToLowerInvariant(),
                        pusername = userName.ToLowerInvariant(),
                    },
                    commandTimeout: 120,
                    commandType: CommandType.StoredProcedure);
                return values.AsList();
            }
        }

        public virtual ConfigEntrySlim GetWeightedConfigEntryByKey(string environment,
            string application,
            string userName,
            string domainName,
            string configKeyName)
        {
            using (var conn = GetConnection())
            {
                var entry = conn.QuerySingleOrDefault<ConfigEntrySlim>(Constants.Sql.StoredProcGetConfigWeighted,
                    new
                    {
                        penvironment = environment.ToLowerInvariant(),
                        papplication = application.ToLowerInvariant(),
                        pdomainname = domainName.ToLowerInvariant(),
                        pusername = userName.ToLowerInvariant(),
                        pconfigkeyname = configKeyName.ToLowerInvariant(),
                    },
                    commandTimeout: 120,
                    commandType: CommandType.StoredProcedure);
                return entry;
            }
        }

        public virtual async Tpl.Task<ConfigEntrySlim> GetWeightedConfigEntryByKeyAsync(string environment,
            string application,
            string userName,
            string domainName,
            string configKeyName)
        {
            using (var conn = await GetConnectionAsync())
            {
                var entry = await conn.QuerySingleOrDefaultAsync<ConfigEntrySlim>(Constants.Sql.StoredProcGetConfigWeighted,
                    new
                    {
                        penvironment = environment.ToLowerInvariant(),
                        papplication = application.ToLowerInvariant(),
                        pdomainname = domainName.ToLowerInvariant(),
                        pusername = userName.ToLowerInvariant(),
                        pconfigkeyname = configKeyName.ToLowerInvariant(),
                    },
                    commandTimeout: 120,
                    commandType: CommandType.StoredProcedure);
                return entry;
            }
        }

        public virtual List<ConfigEntrySlim> GetWeightedConfigEntryByGroup(string environment, string application,
            string domainName, string userName,
            string groupName)
        {
            using (var conn = GetConnection())
            {
                var values = conn.Query<ConfigEntrySlim>(Constants.Sql.StoredProcGetConfigWeightedByGroup,
                    new
                    {
                        penvironment = environment.ToLowerInvariant(),
                        papplication = application.ToLowerInvariant(),
                        pdomainname = domainName.ToLowerInvariant(),
                        pusername = userName.ToLowerInvariant(),
                        pconfiggroupname = groupName.ToLowerInvariant(),
                    },
                    commandTimeout: 120,
                    commandType: CommandType.StoredProcedure);
                return values.AsList();
            }
        }

        public virtual async Tpl.Task<IEnumerable<ConfigEntrySlim>> GetWeightedConfigEntryByGroupAsync(
            string environment, string application, string domainName, string userName,
            string groupName)
        {
            using (var conn = await GetConnectionAsync())
            {
                var values = await conn.QueryAsync<ConfigEntrySlim>(Constants.Sql.StoredProcGetConfigWeightedByGroup,
                    new
                    {
                        penvironment = environment.ToLowerInvariant(),
                        papplication = application.ToLowerInvariant(),
                        pdomainname = domainName.ToLowerInvariant(),
                        pusername = userName.ToLowerInvariant(),
                        pconfiggroupname = groupName.ToLowerInvariant(),
                    },
                    commandTimeout: 120,
                    commandType: CommandType.StoredProcedure);
                return values.AsList();
            }
        }

        public IEnumerable<ConfigEntry> GetFullWeightedConfigEntries(string environment, string application,
            string domainName, string userName)
        {
            using (var conn = GetConnection())
            {
                var entry = conn.Query<ConfigEntry>(Constants.Sql.StoredProcGetConfigWeightedFull,
                    new
                    {
                        penvironment = environment.ToLowerInvariant(),
                        papplication = application.ToLowerInvariant(),
                        pdomainname = domainName.ToLowerInvariant(),
                        pusername = userName.ToLowerInvariant(),
                    },
                    commandTimeout: 120,
                    commandType: CommandType.StoredProcedure);
                return entry.AsList();
            }
        }

        public async Tpl.Task<IEnumerable<ConfigEntry>> GetFullWeightedConfigEntriesAsync(string environment,
            string application, string domainName, string userName)
        {
            using (var conn = await GetConnectionAsync())
            {
                var entry = await conn.QueryAsync<ConfigEntry>(Constants.Sql.StoredProcGetConfigWeightedFull,
                    new
                    {
                        penvironment = environment.ToLowerInvariant(),
                        papplication = application.ToLowerInvariant(),
                        pdomainname = domainName.ToLowerInvariant(),
                        pusername = userName.ToLowerInvariant(),
                    },
                    commandTimeout: 120,
                    commandType: CommandType.StoredProcedure);
                return entry.AsList();
            }
        }

        public virtual ConfigEntry GetFullWeightedConfigEntryByKey(string environment,
            string application,
            string userName,
            string domainName,
            string configKeyName)
        {
            using (var conn = GetConnection())
            {
                var entry = conn.QuerySingleOrDefault<ConfigEntry>(Constants.Sql.StoredProcGetConfigWeightedFull,
                    new
                    {
                        penvironment = environment.ToLowerInvariant(),
                        papplication = application.ToLowerInvariant(),
                        pdomainname = domainName.ToLowerInvariant(),
                        pusername = userName.ToLowerInvariant(),
                        pconfigkeyname = configKeyName.ToLowerInvariant()
                    },
                    commandTimeout: 120,
                    commandType: CommandType.StoredProcedure);
                return entry;
            }
        }

        public virtual async Tpl.Task<ConfigEntry> GetFullWeightedConfigEntryByKeyAsync(string environment,
            string application,
            string userName,
            string domainName,
            string configKeyName)
        {
            using (var conn = await GetConnectionAsync())
            {
                var entry = await conn.QuerySingleOrDefaultAsync<ConfigEntry>(Constants.Sql.StoredProcGetConfigWeightedFull,
                    new
                    {
                        penvironment = environment.ToLowerInvariant(),
                        papplication = application.ToLowerInvariant(),
                        pdomainname = domainName.ToLowerInvariant(),
                        pusername = userName.ToLowerInvariant(),
                        pconfigkeyname = configKeyName.ToLowerInvariant()
                    },
                    commandTimeout: 120,
                    commandType: CommandType.StoredProcedure);
                return entry;
            }
        }
        #endregion
        #region Sync
        public bool ConfigKeyExists(string configKeyName)
        {
            using (var conn = GetConnection())
            {
                return conn.ExecuteScalar<bool>(Constants.Sql.QueryConfigKeyExists,
                    new
                    {
                        configKeyName=configKeyName.ToLowerInvariant()
                    });
            }
        }

        public List<ConfigKey> GetAllConfigKeys()
        {
            using (var conn = GetConnection())
            {
                var entries = conn.Query<ConfigKey>(Constants.Sql.QueryGetAllConfigKeys);
                return entries.ToList();
            }
        }

        public ConfigKey GetConfigKeyById(int configKeyId)
        {
            using (var conn = GetConnection())
            {
                return conn.QuerySingleOrDefault<ConfigKey>(Constants.Sql.QueryGetConfigKeyById,
                    new
                    {
                        configKeyId
                    });
            }
        }

        public ConfigKey GetConfigKeyByName(string configKeyName)
        {
            using (var conn = GetConnection())
            {
                return conn.QuerySingleOrDefault<ConfigKey>(Constants.Sql.QueryGetConfigKeyByName,
                    new
                    {
                        configKeyName=configKeyName.ToLowerInvariant()
                    });
            }
        }

        public ConfigEntry GetConfigEntryById(int configEntryId)
        {
            using (var conn = GetConnection())
            {
                return conn.QuerySingleOrDefault<ConfigEntry>(Constants.Sql.QueryGetConfigEntryById,
                    new
                    {
                        configEntryId
                    });
            }
        }

        public ConfigEntry InsertOrUpdateConfigEntry(ConfigEntry entry)
        {
            using (var conn = GetConnection())
            {
                var configEntryId = conn.ExecuteScalar<int>(Constants.Sql.StoredProcInsertConfigEntry,
                    new
                    {
                        pcreatedby = entry.CreatedBy.ToLowerInvariant(),
                        penvironment = entry.Environment.ToLowerInvariant(),
                        pconfigkeyname = entry.ConfigKeyName.ToLowerInvariant(),
                        pconfigvalue = entry.ConfigValue,
                        pconfigvaluetype = entry.ConfigValueType,
                        pisencrypted = entry.IsEncrypted,
                        papplication = entry.Application.ToLowerInvariant(),
                        pdomainname = entry.DomainName.ToLowerInvariant(),
                        pusername = entry.UserName.ToLowerInvariant(),
                    },
                    commandType: CommandType.StoredProcedure);
                return conn.QuerySingle<ConfigEntry>(Constants.Sql.QueryGetConfigEntryById,
                    new
                    {
                        configEntryId
                    });
            }
        }

        public ConfigEntry UpdateConfigEntry(ConfigEntry entry)
        {
            using (var conn = GetConnection())
            {
                conn.Execute(Constants.Sql.QueryUpdateConfigEntry,
                    new
                    {
                        environment = entry.Environment.ToLowerInvariant(),
                        application = entry.Application.ToLowerInvariant(),
                        domainName = entry.DomainName.ToLowerInvariant(),
                        userName = entry.UserName.ToLowerInvariant(),
                        configKeyId = entry.ConfigKeyId,
                        configValue = entry.ConfigValue,
                        configValueTypeId = entry.ConfigValueType,
                        isEncrypted = entry.IsEncrypted,
                        active = entry.Active,
                        createdBy = entry.CreatedBy.ToLowerInvariant(),
                        createdAt = entry.CreatedAt,
                        updatedBy = entry.UpdatedBy.ToLowerInvariant(),
                        updatedAt = entry.UpdatedAt,
                        configEntryId = entry.ConfigEntryId
                    });
                return conn.QuerySingle<ConfigEntry>(Constants.Sql.QueryGetConfigEntryById,
                    new
                    {
                        configEntryId = entry.ConfigEntryId
                    });
            }
        }

        public void DeleteConfigEntry(int configEntryId)
        {
            using (var conn = GetConnection())
            {
                conn.Execute(Constants.Sql.QueryDeleteConfigEntry, new
                {
                    configEntryId
                });
            }
        }

        public void DeleteConfigEntry(ConfigEntry entry)
        {
            using (var conn = GetConnection())
            {
                conn.Execute(Constants.Sql.QueryDeleteConfigEntry, new
                {
                    configEntryId = entry.ConfigEntryId
                });
            }
        }
        #endregion
        #region Async
        public async Tpl.Task<bool> ConfigKeyExistsAsync(string configKeyName)
        {
            using (var conn = GetConnection())
            {
                return await conn.ExecuteScalarAsync<bool>(Constants.Sql.QueryConfigKeyExists,
                    new
                    {
                        configKeyName = configKeyName.ToLowerInvariant()
                    });
            }
        }

        public async Tpl.Task<List<ConfigKey>> GetAllConfigKeysAsync()
        {
            using (var conn = GetConnection())
            {
                var entries = await conn.QueryAsync<ConfigKey>(Constants.Sql.QueryGetAllConfigKeys);
                return entries.ToList();
            }
        }

        public async Tpl.Task<ConfigKey> GetConfigKeyByIdAsync(int configKeyId)
        {
            using (var conn = GetConnection())
            {
                return await conn.QuerySingleOrDefaultAsync<ConfigKey>(Constants.Sql.QueryGetConfigKeyById,
                    new
                    {
                        configKeyId
                    });
            }
        }

        public async Tpl.Task<ConfigKey> GetConfigKeyByNameAsync(string configKeyName)
        {
            using (var conn = GetConnection())
            {
                return await conn.QuerySingleOrDefaultAsync<ConfigKey>(Constants.Sql.QueryGetConfigKeyByName,
                    new
                    {
                        configKeyName = configKeyName.ToLowerInvariant()
                    });
            }
        }

        public async Tpl.Task<ConfigEntry> GetConfigEntryByIdAsync(int configEntryId)
        {
            using (var conn = GetConnection())
            {
                return await conn.QuerySingleOrDefaultAsync<ConfigEntry>(Constants.Sql.QueryGetConfigEntryById,
                    new
                    {
                        configEntryId
                    });
            }
        }

        public async Tpl.Task<ConfigEntry> InsertOrUpdateConfigEntryAsync(ConfigEntry entry)
        {
            using (var conn = await GetConnectionAsync())
            {
                var configEntryId = await conn.ExecuteScalarAsync<int>(Constants.Sql.StoredProcInsertConfigEntry,
                    new
                    {
                        pcreatedby = entry.CreatedBy.ToLowerInvariant(),
                        penvironment = entry.Environment.ToLowerInvariant(),
                        pconfigkeyname = entry.ConfigKeyName.ToLowerInvariant(),
                        pconfigvalue = entry.ConfigValue,
                        pconfigvaluetype = entry.ConfigValueType,
                        pisencrypted = entry.IsEncrypted,
                        papplication = entry.Application.ToLowerInvariant(),
                        pdomainname = entry.DomainName.ToLowerInvariant(),
                        pusername = entry.UserName.ToLowerInvariant(),
                    },
                    commandType: CommandType.StoredProcedure);
                return await conn.QuerySingleAsync<ConfigEntry>(Constants.Sql.QueryGetConfigEntryById,
                    new
                    {
                        configEntryId
                    });
            }
        }

        public async Tpl.Task<ConfigEntry> UpdateConfigEntryAsync(ConfigEntry entry)
        {
            using (var conn = GetConnection())
            {
                await conn.ExecuteAsync(Constants.Sql.QueryUpdateConfigEntry,
                    new
                    {
                        environment = entry.Environment.ToLowerInvariant(),
                        application = entry.Application.ToLowerInvariant(),
                        domainName = entry.DomainName.ToLowerInvariant(),
                        userName = entry.UserName.ToLowerInvariant(),
                        configKeyId = entry.ConfigKeyId,
                        configValue = entry.ConfigValue,
                        configValueTypeId = entry.ConfigValueType,
                        isEncrypted = entry.IsEncrypted,
                        active = entry.Active,
                        createdBy = entry.CreatedBy.ToLowerInvariant(),
                        createdAt = entry.CreatedAt,
                        updatedBy = entry.UpdatedBy.ToLowerInvariant(),
                        updatedAt = entry.UpdatedAt,
                        configEntryId = entry.ConfigEntryId
                    });
                return await conn.QuerySingleAsync<ConfigEntry>(Constants.Sql.QueryGetConfigEntryById,
                    new
                    {
                        configEntryId = entry.ConfigEntryId
                    });
            }
        }

        public async Tpl.Task DeleteConfigEntryAsync(int configEntryId)
        {
            using (var conn = GetConnection())
            {
                await conn.ExecuteAsync(Constants.Sql.QueryDeleteConfigEntry, new
                {
                    configEntryId
                });
            }
        }

        public async Tpl.Task DeleteConfigEntryAsync(ConfigEntry entry)
        {
            using (var conn = GetConnection())
            {
                await conn.ExecuteAsync(Constants.Sql.QueryDeleteConfigEntry, new
                {
                    configEntryId = entry.ConfigEntryId
                });
            }
        }
        #endregion

        public virtual Tpl.Task InsertOrUpdateGroup(string groupName)
        {
            return Tpl.Task.CompletedTask;
        }
    }
}