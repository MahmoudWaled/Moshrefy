using Microsoft.AspNetCore.Authorization;

namespace Moshrefy.Application.Authorization.Attributes
{
    public class RequirePermissionAttribute : AuthorizeAttribute
    {
        public RequirePermissionAttribute(string entity, string action)
        {
            Policy = $"{entity}.{action}";
        }
    }
}