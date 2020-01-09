namespace Core.DataAccess
{
    internal static partial class Constants
    {
        public static class Sql
        {
            public const string StoredProcGetConfigWeighted = "config.spgetconfigweighted";
            public const string StoredProcGetConfigWeightedByGroup = "config.spgetconfigweightedgroup";
            public const string StoredProcGetConfigWeightedFull = "config.spgetconfigweightedfull";
            public const string StoredProcInsertConfigEntry = "config.spinsertentry";

            public const string QueryConfigKeyExists = @"
SELECT
CASE WHEN COUNT(ConfigKeyName) > 0 THEN TRUE ELSE FA
FROM config.configkey
WHERE ConfigKeyName = @configKeyName
";

            public const string QueryGetAllConfigKeys = @"
SELECT
ConfigKeyId, ConfigKeyName, Active, CreatedBy, CreatedAt
FROM config.configkey";

            public const string QueryGetConfigKeyByName = @"
SELECT
ConfigKeyId, ConfigKeyName, Active, CreatedBy, CreatedAt
FROM config.configkey
WHERE ConfigKeyName = @configKeyName";

            public const string QueryGetConfigKeyById = @"
SELECT
ConfigKeyId, ConfigKeyName, Active, CreatedBy, CreatedAt
FROM config.configkey
WHERE ConfigKeyId = @configKeyId";

            public const string QueryGetAllConfigEntries = @"
SELECT
ce.ConfigEntryId,
ce.Environment,
ce.Application,
ce.DomainName,
ce.UserName,
ce.ConfigKeyId,
c.ConfigKeyName,
ce.ConfigValue,
ce.ConfigValueTypeId AS ConfigValueType,
ce.IsEncrypted,
ce.Active,
ce.CreatedBy,
ce.CreatedAt,
ce.UpdatedBy,
ce.UpdatedAt
FROM config.configentry ce
JOIN ConfigKey c on ce.ConfigKeyId = c.ConfigKeyId";

            public const string QueryGetConfigEntryById = @"
SELECT
ce.ConfigEntryId,
ce.Environment,
ce.Application,
ce.DomainName,
ce.UserName,
ce.ConfigKeyId,
c.ConfigKeyName,
ce.ConfigValue,
ce.ConfigValueTypeId AS ConfigValueType,
ce.IsEncrypted,
ce.Active,
ce.CreatedBy,
ce.CreatedAt,
ce.UpdatedBy,
ce.UpdatedAt
FROM config.configentry ce
JOIN config.configkey c on ce.ConfigKeyId = c.ConfigKeyId
WHERE ce.ConfigEntryId = @configEntryId";


            public const string QueryUpdateConfigEntry = @"
UPDATE config.configentry SET
Environment = @environment
,Application = @application
,DomainName = @domainName
,UserName = @userName
,ConfigKeyId = @configKeyId
,ConfigValue = @configValue
,ConfigValueTypeId = @configValueTypeId
,IsEncrypted = @isEncrypted
,Active = @active
,CreatedBy = @createdBy
,CreatedAt = @createdAt
,UpdatedBy = @updatedBy
,UpdatedAt = @updatedAt
WHERE ConfigEntryId = @configEntryId";

            public const string QueryDeleteConfigEntry = @"
DELETE FROM config.configentry WHERE ConfigEntryId = @configEntryId";
        }
    }
}