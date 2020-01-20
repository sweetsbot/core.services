using System;
using System.Linq;
using System.Security.Claims;
using Core.Entities;

namespace Core.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string Application(this ClaimsPrincipal principal) =>
            principal.FindFirst(CoreClaimTypes.Application)?.Value ?? "Unknown Application";
        public static string DomainName(this ClaimsPrincipal principal) =>
            principal.FindFirst(CoreClaimTypes.DomainName)?.Value ?? "Unknown Domain";
        public static string UserName(this ClaimsPrincipal principal) =>
            principal.FindFirst(CoreClaimTypes.UserName)?.Value ?? "Unknown UserName";

        public static string Department(this ClaimsPrincipal principal) =>
            principal.FindFirst(CoreClaimTypes.Department)?.Value;
        public static string MachineName(this ClaimsPrincipal principal) =>
            principal.FindFirst(CoreClaimTypes.MachineName)?.Value;

        public static string ToBlameString(this ClaimsPrincipal principal)
        {
            var domain = principal.FindFirst(CoreClaimTypes.DomainName);
            var username = principal.FindFirst(CoreClaimTypes.UserName);
            return string.IsNullOrEmpty(domain?.Value) ? username.Value : $"{domain.Value}\\{username.Value}";
        }
        public static Version ApplicationVersion(this ClaimsPrincipal principal)
        {
            var claim = principal.FindFirst(CoreClaimTypes.ApplicationVersion);
            if (claim != null && Version.TryParse(claim.Value, out var value))
                return value;
            return new Version(1, 0);
        }

        public static string[] GetRoles(this ClaimsPrincipal principal)
        {
            var claim = principal.FindAll(CoreClaimTypes.Role);
            if (claim == null) claim = Enumerable.Empty<Claim>();
            return claim.Select(p => p?.Value).ToArray();
        }

        public static long? SessionId(this ClaimsPrincipal principal)
        {
            var claim = principal.FindFirst(CoreClaimTypes.Session);
            if (claim != null && claim.ValueType == ClaimValueTypes.Integer64 && long.TryParse(claim.Value, out var value))
                return value;
            return null;
        }

        public static SessionInfo AsSessionInfo(this ClaimsPrincipal principal) =>
            new SessionInfo
            {
                ApplicationName = principal.Application(),
                ApplicationVersion = principal.ApplicationVersion(),
                SessionId = principal.SessionId(),
                DepartmentName = principal.Department(),
                DomainName = principal.DomainName(),
                UserName = principal.UserName(),
                RoleNames = principal.GetRoles(),
                MachineName = principal.MachineName()
            };

    }
}