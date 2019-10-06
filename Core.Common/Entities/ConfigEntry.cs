using System;
using System.Text.Json.Serialization;
using Core.Common;

namespace Core.Entities
{
    public sealed class ConfigEntry : IConfigEntry
    {
        [JsonPropertyName("id")]
        public int ConfigEntryId { get; set; }

        [JsonPropertyName("env")]
        public string Environment { get; set; }

        [JsonPropertyName("app")]
        public string Application { get; set; }

        [JsonPropertyName("domain")]
        public string DomainName { get; set; }

        [JsonPropertyName("username")]
        public string UserName { get; set; }

        [JsonPropertyName("keyId")]
        public int ConfigKeyId { get; set; }

        [JsonPropertyName("keyName")]
        public string ConfigKeyName { get; set; }

        [JsonPropertyName("value")]
        public string ConfigValue { get; set; }

        [JsonPropertyName("type")]
        public ConfigValueType ConfigValueType { get; set; }

        [JsonPropertyName("encrypted")]
        public bool IsEncrypted { get; set; }

        [JsonPropertyName("active")]
        public bool Active { get; set; }

        [JsonPropertyName("createdBy")]
        public string CreatedBy { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updatedBy")]
        public string UpdatedBy { get; set; }

        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }
}