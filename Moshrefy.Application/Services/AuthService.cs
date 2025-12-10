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
        IJwtTokenService jwtTokenService,
        IUnitOfWork unitOfWork,
        ITokenBlacklistService tokenBlacklistService,
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration,
        IEmailService emailService,
        ILogger<AuthService> logger) : IAuthService
    {
        private readonly ILogger<AuthService> _logger = logger;

        public async Task<AuthResponseDTO> LoginAsync(LoginUserDTO loginUserDTO)
        {
            var user = await userManager.FindByNameAsync(loginUserDTO.UserName);
            if (user == null)
                throw new UnauthorizedAccessException("Invalid username or password.");

            if (!user.IsActive || user.IsDeleted)
                throw new UnauthorizedAccessException("User account is inactive or deleted.");

            var result = await signInManager.CheckPasswordSignInAsync(user, loginUserDTO.Password, lockoutOnFailure: true);

            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    throw new UnauthorizedAccessException("Account is locked out.");
                }
                throw new UnauthorizedAccessException("Invalid username or password.");
            }

            var roles = await userManager.GetRolesAsync(user);
            var accessToken = jwtTokenService.GenerateAccessToken(user, roles);
            var refreshToken = jwtTokenService.GenerateRefreshToken();

            // Store refresh token
            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7) // 7 days expiry
            };

            await unitOfWork.RefreshTokens.AddAsync(refreshTokenEntity);
            await unitOfWork.SaveChangesAsync();

            var tokenExpiryMinutes = int.Parse(configuration["Jwt:TokenExpiryMinutes"] ?? "60");

            return new AuthResponseDTO
            {
                Success = true,
                Token = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(tokenExpiryMinutes),
                UserId = user.Id,
                UserName = user.UserName!,
                Roles = roles
            };
        }

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

            var roles = await userManager.GetRolesAsync(user);
            var accessToken = jwtTokenService.GenerateAccessToken(user, roles);
            var refreshToken = jwtTokenService.GenerateRefreshToken();

            // Store refresh token
            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            await unitOfWork.RefreshTokens.AddAsync(refreshTokenEntity);
            await unitOfWork.SaveChangesAsync();
            var tokenExpiryMinutes = int.Parse(configuration["Jwt:TokenExpiryMinutes"] ?? "60");

            return new AuthResponseDTO
            {
                Success = true,
                Token = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(tokenExpiryMinutes),
                UserId = user.Id,
                UserName = user.UserName!,
                Roles = roles
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
            var baseUrl = configuration["AppSettings:FrontendUrl"] ?? "https://localhost:3000";
            var resetLink = $"{baseUrl}/reset-password?userId={user.Id}&token={encodedToken}";

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
            var baseUrl = configuration["AppSettings:FrontendUrl"] ?? "https://localhost:3000";
            var confirmationLink = $"{baseUrl}/confirm-email?userId={user.Id}&token={encodedToken}";

            await emailService.SendEmailConfirmationAsync(user.Email!, user.UserName!, confirmationLink);

            return true;
        }

        public async Task<string> RefreshTokenAsync(string refreshToken)
        {
            var storedToken = await unitOfWork.RefreshTokens.GetByTokenAsync(refreshToken);

            if (storedToken == null || storedToken.ExpiresAt < DateTime.UtcNow)
                throw new UnauthorizedAccessException("Invalid or expired refresh token.");

            var user = await userManager.FindByIdAsync(storedToken.UserId);
            if (user == null || !user.IsActive || user.IsDeleted)
                throw new UnauthorizedAccessException("User not found or inactive.");

            var roles = await userManager.GetRolesAsync(user);
            var newAccessToken = jwtTokenService.GenerateAccessToken(user, roles);

            return newAccessToken;
        }

        public async Task LogoutAsync()
        {
            // Get the current access token from the request
            var token = httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString()?.Replace("Bearer ", "");

            if (!string.IsNullOrEmpty(token))
            {
                // Get token expiration time
                var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);
                var expiration = jwtToken.ValidTo - DateTime.UtcNow;

                // Add token to blacklist until it expires
                await tokenBlacklistService.BlacklistTokenAsync(token, expiration);
            }

            // Get user ID and revoke all refresh tokens
            var userId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                await unitOfWork.RefreshTokens.DeleteByUserIdAsync(userId);
                await unitOfWork.SaveChangesAsync();
            }

            await signInManager.SignOutAsync();
        }

        public async Task RevokeRefreshTokenAsync(string refreshToken, string userId)
        {
            var storedToken = await unitOfWork.RefreshTokens.GetByTokenAsync(refreshToken);

            if (storedToken == null)
                throw new NoDataFoundException("Refresh token not found.");

            if (storedToken.UserId != userId)
                throw new UnauthorizedAccessException("This token does not belong to the current user.");

            await unitOfWork.RefreshTokens.DeleteByTokenAsync(refreshToken);
            await unitOfWork.SaveChangesAsync();
        }
    }
}