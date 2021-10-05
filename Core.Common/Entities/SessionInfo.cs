using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public sealed class SessionInfo : IEquatable<SessionInfo>, ISessionInfo
    {
        public string DomainName { get; set; }
        public string UserName { get; set; }
        public string ApplicationName { get; set; }
        public Version ApplicationVersion { get; set; }
        public string MachineName { get; set; }
        public string DepartmentName { get; set; }
        public long? SessionId { get; set; }
        public string[] RoleNames { get; set; }

        public override bool Equals(object obj) => Equals(obj as SessionInfo);

        public bool Equals(SessionInfo other)
        {
            return other != null &&
                   DomainName == other.DomainName &&
                   UserName == other.UserName &&
                   ApplicationName == other.ApplicationName &&
                   EqualityComparer<Version>.Default.Equals(ApplicationVersion, other.ApplicationVersion) &&
                   MachineName == other.MachineName &&
                   DepartmentName == other.DepartmentName &&
                   SessionId == other.SessionId &&
                   EqualityComparer<string[]>.Default.Equals(RoleNames, other.RoleNames);
        }

        public override int GetHashCode()
        {
            var hashCode = -904422506;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(DomainName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(UserName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ApplicationName);
            hashCode = hashCode * -1521134295 + EqualityComparer<Version>.Default.GetHashCode(ApplicationVersion);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(MachineName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(DepartmentName);
            hashCode = hashCode * -1521134295 + SessionId.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string[]>.Default.GetHashCode(RoleNames);
            return hashCode;
        }

        public string ToBlameString() => $"{DomainName}\\{UserName}";
    }
}
