using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moshrefy.Application.DTOs.Auth;
using Moshrefy.Application.DTOs.User;
using Moshrefy.Application.Interfaces.IServices;
using Moshrefy.Domain.Exceptions;
using Moshrefy.Domain.Identity;

namespace Moshrefy.Application.Services
{
    // Service for managing the profile of the currently authenticated user.
    public class UserProfileService(
        UserManager<ApplicationUser> _userManager,
        IMapper _mapper,
        ITenantContext _tenantContext) : IUserProfileService
    {

        public async Task<UserResponseDTO> GetMyProfileAsync()
        {
            var userId = _tenantContext.GetCurrentUserId();
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                throw new NotFoundException<string>(nameof(ApplicationUser), "id", userId);

            return _mapper.Map<UserResponseDTO>(user);
        }

        public async Task UpdateMyProfileAsync(UpdateUserProfileDTO updateUserProfileDTO)
        {
            if (updateUserProfileDTO == null)
                throw new BadRequestException("UpdateUserProfileDTO cannot be null.");

            var userId = _tenantContext.GetCurrentUserId();
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                throw new NotFoundException<string>(nameof(ApplicationUser), "id", userId);

            if (!string.IsNullOrEmpty(updateUserProfileDTO.Name))
            {
                user.Name = updateUserProfileDTO.Name;
            }

            if (!string.IsNullOrEmpty(updateUserProfileDTO.UserName))
            {
                var existingUser = await _userManager.FindByNameAsync(updateUserProfileDTO.UserName);
                if (existingUser != null && existingUser.Id != userId)
                    throw new ConflictException($"A user with username {updateUserProfileDTO.UserName} already exists.");

                user.UserName = updateUserProfileDTO.UserName;
            }

            if (!string.IsNullOrEmpty(updateUserProfileDTO.Email))
            {
                var existingUser = await _userManager.FindByEmailAsync(updateUserProfileDTO.Email);
                if (existingUser != null && existingUser.Id != userId)
                    throw new ConflictException($"A user with email {updateUserProfileDTO.Email} already exists.");

                user.Email = updateUserProfileDTO.Email;
            }

            if (!string.IsNullOrEmpty(updateUserProfileDTO.PhoneNumber))
            {
                user.PhoneNumber = updateUserProfileDTO.PhoneNumber;
            }

            user.ModifiedById = userId;
            user.ModifiedAt = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new BadRequestException($"Profile update failed: {errors}");
            }
        }

        public async Task ChangeMyPasswordAsync(ChangePasswordDTO changePasswordDTO)
        {
            if (changePasswordDTO == null)
                throw new BadRequestException("ChangePasswordDTO cannot be null.");

            var userId = _tenantContext.GetCurrentUserId();
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                throw new NotFoundException<string>(nameof(ApplicationUser), "id", userId);

            var result = await _userManager.ChangePasswordAsync(
                user,
                changePasswordDTO.CurrentPassword,
                changePasswordDTO.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new FailedException($"Password change failed: {errors}");
            }
        }
    }
}