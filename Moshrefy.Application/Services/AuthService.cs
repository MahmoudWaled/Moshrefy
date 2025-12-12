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
        private readonly ILogger<AuthService> _logger = logger;


        public async Task<AuthResponseDTO> ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO)
        {
            var user = await userManager.FindByIdAsync(resetPasswordDTO.UserId);
            if (user == null)
                throw new NoDataFoundException("User not found.");

            // Decode the token in case it comes URL encoded
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

            // After password reset, automatically sign in the user
            await signInManager.SignInAsync(user, isPersistent: false);

            return new AuthResponseDTO
            {
                Success = true,
                UserId = user.Id,
                UserName = user.UserName!,
                Roles = (await userManager.GetRolesAsync(user)).ToList()
            };
        }

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
            
            // Build reset link - adjust the base URL according to your frontend
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
                // Log the error but don't throw to prevent revealing if email exists
            }
        }

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

        public async Task<bool> ConfirmEmailAsync(string userId, string token)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Email confirmation failed: User {UserId} not found", userId);
                return false;
            }

            // Decode the token in case it comes URL encoded
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

        public async Task<bool> ResendConfirmationEmailAsync(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
                return false;

            if (user.EmailConfirmed)
                throw new BadRequestException("Email is already confirmed.");

            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = HttpUtility.UrlEncode(token);
            
            // Build confirmation link - adjust the base URL according to your frontend
            var baseUrl = configuration["AppSettings:FrontendUrl"] ?? "https://localhost:5001";
            var confirmationLink = $"{baseUrl}/Auth/ConfirmEmail?userId={user.Id}&token={encodedToken}";

            await emailService.SendEmailConfirmationAsync(user.Email!, user.UserName!, confirmationLink);

            return true;
        }

        public async Task LogoutAsync()
        {
            await signInManager.SignOutAsync();
        }

        public Task RevokeRefreshTokenAsync(string refreshToken, string userId)
        {
            throw new NotImplementedException("JWT refresh tokens are disabled. Cookie authentication is used.");
        }

        // Cookie-based authentication methods (for MVC)
        public async Task CookieLoginAsync(string userName, string password)
        {
            var user = await userManager.FindByNameAsync(userName);
            if (user == null)
                throw new UnauthorizedAccessException("Invalid username or password.");

            if (!user.IsActive || user.IsDeleted)
                throw new UnauthorizedAccessException("User account is inactive or deleted.");

            // Verify password first
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

            // Sign in with custom claims
            await signInManager.SignInWithClaimsAsync(
                user, 
                isPersistent: false,
                additionalClaims);

            _logger.LogInformation("User {UserName} logged in successfully with CenterId: {CenterId}", 
                userName, user.CenterId?.ToString() ?? "None");
        }

        public async Task CookieLogoutAsync()
        {
            await signInManager.SignOutAsync();
        }
    }
}