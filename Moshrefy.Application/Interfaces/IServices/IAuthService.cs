using Moshrefy.Application.DTOs.Auth;
using Moshrefy.Application.DTOs.User;

namespace Moshrefy.Application.Interfaces.IServices
{
    public interface IAuthService
    {
        Task<AuthResponseDTO> LoginAsync(LoginUserDTO loginUserDTO);
        Task<AuthResponseDTO> ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO);
        Task RequestResetPasswordAsync(RequestResetPasswordDTO requestResetPasswordDTO);
        Task ChangePasswordAsync(ChangePasswordDTO changePasswordDTO, string userId);
        Task<bool> ConfirmEmailAsync(string userId, string token);
        Task<bool> ResendConfirmationEmailAsync(string email);
        Task<string> RefreshTokenAsync(string refreshToken);
        Task LogoutAsync();
        Task RevokeRefreshTokenAsync(string refreshToken, string userId);
    }
}

