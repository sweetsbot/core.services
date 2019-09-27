using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Core.Common.Serialization
{
    public class VersionJsonConverter : JsonConverter<Version>
    {
        public override Version Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            if (string.IsNullOrEmpty(value))
            {
                return new Version(1, 0);
            }

            if (!Version.TryParse(value, out var version))
            {
                version = new Version(1, 0);
            }

            return version;
        }

        public override void Write(Utf8JsonWriter writer, Version value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}