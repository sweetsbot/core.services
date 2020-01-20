using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public sealed class SessionInfo
    {
        public SessionInfo()
        {
            RoleNames = new List<string>();
        }
        public string DomainName { get; set; }
        public string UserName { get; set; }
        public string ApplicationName { get; set; }
        public Version ApplicationVersion { get; set; }
        public string MachineName { get; set; }
        public string DepartmentName { get; set; }
        public long? SessionId { get; set; }
        public IList<string> RoleNames { get; }

        public string ToBlameString() => $"{DomainName}\\{UserName}";
    }
}
