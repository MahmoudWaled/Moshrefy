using Moshrefy.Application.DTOs.Auth;
using Moshrefy.Application.DTOs.User;

namespace Moshrefy.Application.Interfaces.IServices
{
    public interface IAuthService
    {
        
        // Cookie-based authentication (for MVC)
        Task CookieLoginAsync(string userName, string password);
        Task CookieLogoutAsync();
        

        Task<AuthResponseDTO> ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO);
        Task RequestResetPasswordAsync(RequestResetPasswordDTO requestResetPasswordDTO);
        Task ChangePasswordAsync(ChangePasswordDTO changePasswordDTO, string userId);
        Task<bool> ConfirmEmailAsync(string userId, string token);
        Task<bool> ResendConfirmationEmailAsync(string email);
        Task LogoutAsync();
    }
}

