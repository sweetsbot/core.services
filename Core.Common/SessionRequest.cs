using System;
using System.Text.Json.Serialization;
using Core.Common.Serialization;

namespace Core.Common
{
    public class SessionRequest
    {
        public string UserName { get; set; }
        public string DomainName { get; set; }
        public string Application { get; set; }
        [JsonConverter(typeof(VersionJsonConverter))]
        public Version ApplicationVersion { get; set; }
        public string MachineName { get; set; }
        public string Department { get; set; }
    }
}