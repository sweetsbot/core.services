using System;

namespace Core.Common
{
    public static class CoreClaimTypes
    {
        public const string Base = "http://schemas.company.com/2019/09/security/claims";
        public const string UserName = Base + "/username";
        public const string MachineName = Base + "/machinename";
        public const string Application = Base + "/application";
        public const string Session = Base + "/sessionid";
        public const string Department = Base + "/department";
        public const string Role = Base + "/role";
    }
}