using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moshrefy.Application.DTOs.User;
using Moshrefy.Application.Interfaces.IServices;
using Moshrefy.Web.Models;

namespace Moshrefy.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, IMapper mapper, ILogger<AuthController> logger)
        {
            _authService = authService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            // Redirect if already authenticated
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
            {
                return View(loginVM);
            }

            try
            {
                var loginUserDTO = _mapper.Map<LoginUserDTO>(loginVM);

                var response = await _authService.LoginAsync(loginUserDTO);

                if (response.Success)
                {
                    // Store authentication token in cookie or session
                    HttpContext.Response.Cookies.Append("AuthToken", response.Token, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = response.ExpiresAt
                    });

                    // Store refresh token
                    if (!string.IsNullOrEmpty(response.RefreshToken))
                    {
                        HttpContext.Response.Cookies.Append("RefreshToken", response.RefreshToken, new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.Strict,
                            Expires = DateTimeOffset.UtcNow.AddDays(7)
                        });
                    }

                    // Store user info in session
                    HttpContext.Session.SetString("UserId", response.UserId);
                    HttpContext.Session.SetString("UserName", response.UserName);

                    _logger.LogInformation($"User {loginVM.UserName} logged in successfully", loginVM.UserName, DateTime.UtcNow);
                    
                    TempData["SuccessMessage"] = "Login successful!";
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Login failed. Please try again.");
                return View(loginVM);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, $"Unauthorized login attempt for user {loginVM.UserName}", loginVM.UserName);
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(loginVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during login for user {loginVM.UserName}", loginVM.UserName);
                ModelState.AddModelError(string.Empty, "An error occurred during login. Please try again later.");
                return View(loginVM);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userName = HttpContext.Session.GetString("UserName");
                
                await _authService.LogoutAsync();

                // Clear cookies
                HttpContext.Response.Cookies.Delete("AuthToken");
                HttpContext.Response.Cookies.Delete("RefreshToken");

                // Clear session
                HttpContext.Session.Clear();

                _logger.LogInformation($"User {userName} logged out successfully at {DateTime.UtcNow}", userName, DateTime.UtcNow);
                
                TempData["SuccessMessage"] = "You have been logged out successfully.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return RedirectToAction("Login");
            }
        }
    }
}