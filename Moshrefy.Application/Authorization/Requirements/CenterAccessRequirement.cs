using Microsoft.AspNetCore.Authorization;

namespace Moshrefy.Application.Authorization.Requirements
{
    public class CenterAccessRequirement(string entity, string action) : IAuthorizationRequirement
    {
        public string Action { get; } = action;
        public string Entity { get; } = entity;
    }
}