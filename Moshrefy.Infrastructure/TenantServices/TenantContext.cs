using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moshrefy.Application.Interfaces.IServices;
using Moshrefy.Domain.Enums;
using Moshrefy.Domain.Identity;
using System.Security.Claims;

namespace Moshrefy.infrastructure.TenantServices
{
    public class TenantContext : ITenantContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;

        public TenantContext(IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public string GetCurrentUserId()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User is not authenticated.");

            return userId;
        }

        public int? GetCurrentCenterId()
        {
            var centerIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirstValue("CenterId");

            if (string.IsNullOrEmpty(centerIdClaim))
                return null;

            return int.Parse(centerIdClaim);
        }

        public bool IsSuperAdmin()
        {
            return _httpContextAccessor.HttpContext?.User?.IsInRole(RolesNames.SuperAdmin.ToString()) ?? false;
        }

        public bool IsAdmin()
        {
            return _httpContextAccessor.HttpContext?.User?.IsInRole(RolesNames.Admin.ToString()) ?? false;
        }

        public async Task<string[]> GetCurrentUserRolesAsync()
        {
            var userId = GetCurrentUserId();
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                throw new UnauthorizedAccessException("User not found.");

            var roles = await _userManager.GetRolesAsync(user);
            return roles.ToArray();
        }
    }
}