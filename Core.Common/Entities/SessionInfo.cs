using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public sealed class SessionInfo
    {
        public string DomainName { get; set; }
        public string UserName { get; set; }
        public string ApplicationName { get; set; }
        public Version ApplicationVersion { get; set; }
        public string MachineName { get; set; }
        public string DepartmentName { get; set; }
        public long? SessionId { get; set; }
        public string[] RoleNames { get; set; }

        public string ToBlameString() => $"{DomainName}\\{UserName}";
    }
}
