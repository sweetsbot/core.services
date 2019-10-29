using System;
using System.Security.Claims;
using Core.Common;

namespace Core.Business.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string Application(this ClaimsPrincipal principal) => 
            principal.FindFirst(CoreClaimTypes.Application)?.Value ?? "Unknown";
        public static string DomainName(this ClaimsPrincipal principal) => 
            principal.FindFirst(CoreClaimTypes.DomainName)?.Value ?? "Unknown";
        public static string UserName(this ClaimsPrincipal principal) => 
            principal.FindFirst(CoreClaimTypes.UserName)?.Value ?? "Unknown";
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
            return new Version(1,0);
        }
        public static long? SessionId(this ClaimsPrincipal principal)
        {
            var claim = principal.FindFirst(CoreClaimTypes.Session);
            if (claim != null && claim.ValueType == ClaimValueTypes.Integer64 && long.TryParse(claim.Value, out var value))
                return value;
            return null;
        }
    }
}