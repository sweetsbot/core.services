using System.Linq;
using System.Security.Claims;
using Core.Common;

namespace Core.Business
{
    public static class ClaimsPrincipalExtensions
    {
        public static string Application(this ClaimsPrincipal principal) => 
            principal.FindFirst(CoreClaimTypes.Application)?.Value ?? "Unknown";
        public static string DomainName(this ClaimsPrincipal principal) => 
            principal.Identity.Name.Split('\\')[0];
        public static string UserName(this ClaimsPrincipal principal) => 
            principal.Identity.Name.Split('\\')[1];
        public static long? SessionId(this ClaimsPrincipal principal)
        {
            var claim = principal.FindFirst(CoreClaimTypes.Session);
            if (claim.ValueType == ClaimValueTypes.Integer64 && long.TryParse(claim.Value, out var value))
                return value;
            return null;
        }
    }
}