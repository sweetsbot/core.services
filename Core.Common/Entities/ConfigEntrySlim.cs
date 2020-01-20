using System;
using System.Text.Json.Serialization;

namespace Core.Entities
{
    public sealed class ConfigEntrySlim : IConfigEntry
    {
        [JsonPropertyName("id")] public int ConfigEntryId { get; set; }
        [JsonPropertyName("keyName")] public string ConfigKeyName { get; set; }
        [JsonPropertyName("type")] public ConfigValueType ConfigValueType { get; set; }
        [JsonPropertyName("value")] public string ConfigValue { get; set; }
        [JsonPropertyName("encrypted")] public bool IsEncrypted { get; set; }
    }
}