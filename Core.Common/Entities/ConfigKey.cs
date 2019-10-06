using System;
using System.Text.Json.Serialization;

namespace Core.Entities
{
    public sealed class ConfigKey
    {
        [JsonPropertyName("id")]
        public int ConfigKeyId { get; set; }
        [JsonPropertyName("keyName")]
        public string ConfigKeyName { get; set; }
        [JsonPropertyName("active")]
        public bool Active { get; set; }
        [JsonPropertyName("createdBy")]
        public string CreatedBy { get; set; }
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }
    }
}