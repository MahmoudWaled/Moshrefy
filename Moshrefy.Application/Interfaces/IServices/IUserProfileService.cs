using Moshrefy.Application.DTOs.Auth;
using Moshrefy.Application.DTOs.User;

namespace Moshrefy.Application.Interfaces.IServices
{
    // Service for managing the profile of the currently authenticated user.
    public interface IUserProfileService
    {
        Task<UserResponseDTO> GetMyProfileAsync();
        Task UpdateMyProfileAsync(UpdateUserProfileDTO updateUserProfileDTO);
        Task ChangeMyPasswordAsync(ChangePasswordDTO changePasswordDTO);
    }
}