using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moshrefy.Application.DTOs.Auth;
using Moshrefy.Application.DTOs.User;
using Moshrefy.Application.Interfaces.IServices;
using Moshrefy.Application.Interfaces.IUnitOfWork;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Exceptions;
using Moshrefy.Domain.Identity;
using System.Security.Claims;
using System.Web;

namespace Moshrefy.Application.Services
{
    public class AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IUnitOfWork unitOfWork,
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration,
        IEmailService emailService,
        ILogger<AuthService> logger) : IAuthService
    {
        #region Fields

        private readonly ILogger<AuthService> _logger = logger;

        #endregion

        #region Password Management

        // Request password reset
        public async Task RequestResetPasswordAsync(RequestResetPasswordDTO requestResetPasswordDTO)
        {
            var user = await userManager.FindByEmailAsync(requestResetPasswordDTO.Email);
            if (user == null)
            {
                // Don't reveal that the user doesn't exist for security
                return;
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = HttpUtility.UrlEncode(token);
            
            var baseUrl = configuration["AppSettings:FrontendUrl"] ?? "https://localhost:5001";
            var resetLink = $"{baseUrl}/Auth/ResetPassword?userId={user.Id}&token={encodedToken}";

            try
            {
                await emailService.SendPasswordResetEmailAsync(user.Email!, user.UserName!, resetLink);
                _logger.LogInformation("Password reset email sent successfully to {Email}", user.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send password reset email to {Email}", user.Email);
            }
        }

        // Reset password
        public async Task<AuthResponseDTO> ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO)
        {
            var user = await userManager.FindByIdAsync(resetPasswordDTO.UserId);
            if (user == null)
                throw new NoDataFoundException("User not found.");

            var decodedToken = HttpUtility.UrlDecode(resetPasswordDTO.Token);
            
            _logger.LogInformation("Attempting password reset for user {UserId}", resetPasswordDTO.UserId);

            var result = await userManager.ResetPasswordAsync(user, decodedToken, resetPasswordDTO.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError("Password reset failed for user {UserId}: {Errors}", resetPasswordDTO.UserId, errors);
                throw new FailedException($"Password reset failed: {errors}");
            }

            _logger.LogInformation("Password reset successful for user {UserId}", resetPasswordDTO.UserId);

            // Automatically sign in the user after successful password reset
            await signInManager.SignInAsync(user, isPersistent: false);

            return new AuthResponseDTO
            {
                Success = true,
                UserId = user.Id,
                UserName = user.UserName!,
                Roles = (await userManager.GetRolesAsync(user)).ToList()
            };
        }

        // Change password
        public async Task ChangePasswordAsync(ChangePasswordDTO changePasswordDTO, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                throw new NoDataFoundException("User not found.");

            var result = await userManager.ChangePasswordAsync(user, changePasswordDTO.CurrentPassword, changePasswordDTO.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new FailedException($"Password change failed: {errors}");
            }
        }

        #endregion

        #region Email Confirmation

        // Confirm email
        public async Task<bool> ConfirmEmailAsync(string userId, string token)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Email confirmation failed: User {UserId} not found", userId);
                return false;
            }

            var decodedToken = HttpUtility.UrlDecode(token);
            
            _logger.LogInformation("Attempting email confirmation for user {UserId}", userId);

            var result = await userManager.ConfirmEmailAsync(user, decodedToken);
            
            if (result.Succeeded)
            {
                _logger.LogInformation("Email confirmed successfully for user {UserId}", userId);
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError("Email confirmation failed for user {UserId}: {Errors}", userId, errors);
            }
            
            return result.Succeeded;
        }

        // Resend confirmation email
        public async Task<bool> ResendConfirmationEmailAsync(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
                return false;

            if (user.EmailConfirmed)
                throw new BadRequestException("Email is already confirmed.");

            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = HttpUtility.UrlEncode(token);
            
            var baseUrl = configuration["AppSettings:FrontendUrl"] ?? "https://localhost:5001";
            var confirmationLink = $"{baseUrl}/Auth/ConfirmEmail?userId={user.Id}&token={encodedToken}";

            await emailService.SendEmailConfirmationAsync(user.Email!, user.UserName!, confirmationLink);

            return true;
        }

        #endregion

        #region Cookie Authentication

        // Cookie login
        public async Task CookieLoginAsync(string userName, string password)
        {
            var user = await userManager.FindByNameAsync(userName);
            if (user == null)
                throw new UnauthorizedAccessException("Invalid username or password.");

            if (!user.IsActive || user.IsDeleted)
                throw new UnauthorizedAccessException("User account is inactive or deleted.");

            var passwordCheck = await userManager.CheckPasswordAsync(user, password);
            if (!passwordCheck)
            {
                await userManager.AccessFailedAsync(user);
                throw new UnauthorizedAccessException("Invalid username or password.");
            }

            // Add custom claims including CenterId
            var additionalClaims = new List<Claim>();
            
            if (user.CenterId.HasValue)
            {
                additionalClaims.Add(new Claim("CenterId", user.CenterId.Value.ToString()));
            }

            await signInManager.SignInWithClaimsAsync(
                user, 
                isPersistent: false,
                additionalClaims);

            _logger.LogInformation($"User {user.UserName} logged in successfully with CenterId: {user.CenterId}", 
                userName, user.CenterId?.ToString() ?? "None");
        }

        // Cookie logout
        public async Task CookieLogoutAsync()
        {
            await signInManager.SignOutAsync();
        }

        // Standard logout
        public async Task LogoutAsync()
        {
            await signInManager.SignOutAsync();
        }

        #endregion

    }
}