using Moshrefy.Domain.Identity;
using System.Security.Claims;

namespace Moshrefy.Application.Interfaces.IServices
{
    public interface IJwtTokenService
    {
        string GenerateAccessToken(ApplicationUser user, IEnumerable<string> roles);
        string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
