using Microsoft.AspNetCore.Authorization;
using Common.HelperLibrary.Enum;
using System.Data;

namespace Common.HelperLibrary.Authorize
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class AuthorizeRoles : AuthorizeAttribute
    {
        public AuthorizeRoles(params UserType[] roles)
        {
            Roles = string.Join(",", roles.Select(r => r.GetDescription()));
        }
    }
}
