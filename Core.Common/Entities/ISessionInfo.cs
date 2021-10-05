using System;

namespace Core.Entities
{
    public interface ISessionInfo
    {
        string DomainName { get; }
        string UserName { get; }
        string ApplicationName { get; }
        Version ApplicationVersion { get; }
        string MachineName { get; }
        string DepartmentName { get; }
        long? SessionId { get; }
        string[] RoleNames { get; }
        string ToBlameString();
    }
}