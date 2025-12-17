using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moshrefy.Application.Interfaces.IServices;
using Moshrefy.Web.Models;

namespace Moshrefy.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IAuthService authService,
            ILogger<AuthController> logger)
        {
            _authService = authService;
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
                // Use IAuthService to handle cookie-based login
                await _authService.CookieLoginAsync(loginVM.UserName, loginVM.Password);

                _logger.LogInformation($"User {loginVM.UserName} logged in successfully at {DateTime.UtcNow}");
                return RedirectToAction("Index", "Home");
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, $"Unauthorized login attempt for user {loginVM.UserName}");
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(loginVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during login for user {loginVM.UserName}");
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
                var userName = User.Identity?.Name;
                
                await _authService.CookieLogoutAsync();

                _logger.LogInformation($"User {userName} logged out successfully at {DateTime.UtcNow}");
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return RedirectToAction("Login");
            }
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}