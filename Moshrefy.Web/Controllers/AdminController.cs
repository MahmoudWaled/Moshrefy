using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moshrefy.Application.Interfaces.IServices;
using Moshrefy.Domain.Enums;
using Moshrefy.Domain.Paramter;
using Moshrefy.Web.Models.User;

namespace Moshrefy.Web.Controllers
{
    [Authorize(Roles = nameof(RolesNames.Admin))]
    public class AdminController : Controller
    {
        private readonly IUserManagementService _userManagementService;
        private readonly ITenantContext _tenantContext;
        private readonly IMapper _mapper;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            IUserManagementService userManagementService,
            ITenantContext tenantContext,
            IMapper mapper,
            ILogger<AdminController> logger)
        {
            _userManagementService = userManagementService;
            _tenantContext = tenantContext;
            _mapper = mapper;
            _logger = logger;
        }

        // Dashboard
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // Users Management
        [HttpGet]
        public IActionResult Users()
        {
            return View();
        }

        // Get users data via AJAX
        [HttpPost]
        public async Task<IActionResult> GetUsersData()
        {
            try
            {
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 25;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int pageNumber = (skip / pageSize) + 1;

                var filterStatus = Request.Form["filterStatus"].FirstOrDefault();

                var paginationParams = new PaginationParamter
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                List<Application.DTOs.User.UserResponseDTO> usersDTO;

                if (filterStatus == "inactive")
                {
                    usersDTO = await _userManagementService.GetInactiveUsersInMyCenterAsync(paginationParams);
                }
                else if (filterStatus == "active")
                {
                    usersDTO = await _userManagementService.GetActiveUsersInMyCenterAsync(paginationParams);
                }
                else
                {
                    usersDTO = await _userManagementService.GetAllUsersInMyCenterAsync(paginationParams);
                }

                var usersVM = _mapper.Map<List<UserVM>>(usersDTO);

                // Apply search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    usersVM = usersVM.Where(u =>
                        u.Name.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ||
                        u.UserName.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ||
                        (u.Email != null && u.Email.Contains(searchValue, StringComparison.OrdinalIgnoreCase)) ||
                        (u.PhoneNumber != null && u.PhoneNumber.Contains(searchValue, StringComparison.OrdinalIgnoreCase))
                    ).ToList();
                }

                var jsonData = new
                {
                    draw = draw,
                    recordsTotal = usersVM.Count,
                    recordsFiltered = usersVM.Count,
                    data = usersVM
                };

                return Ok(jsonData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading users data");
                return StatusCode(500, new { error = "Error loading data. Please try again." });
            }
        }

        // Create User
        [HttpGet]
        public IActionResult CreateUser()
        {
            return View(new CreateUserVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(CreateUserVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Validate that only Employee or Manager roles are selected
                if (model.RoleName != RolesNames.Employee && model.RoleName != RolesNames.Manager)
                {
                    ModelState.AddModelError("RoleName", "You can only create Employee or Manager accounts.");
                    return View(model);
                }

                // Set the CenterId to the current admin's center
                model.CenterId = _tenantContext.GetCurrentCenterId();
                
                var createUserDTO = _mapper.Map<Application.DTOs.User.CreateUserDTO>(model);
                await _userManagementService.CreateUserAsync(createUserDTO);
                
                TempData["SuccessMessage"] = "Team member added successfully!";
                return RedirectToAction(nameof(Users));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return View(model);
            }
        }

        // User Details
        [HttpGet]
        public async Task<IActionResult> UserDetails(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            try
            {
                var userDTO = await _userManagementService.GetUserByIdInMyCenterAsync(id);
                var userVM = _mapper.Map<UserVM>(userDTO);
                return View(userVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading user details for ID {id}");
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Users));
            }
        }

        // Edit User
        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            try
            {
                var userDTO = await _userManagementService.GetUserByIdInMyCenterAsync(id);
                var userVM = _mapper.Map<UserVM>(userDTO);

                var updateUserVM = new UpdateUserVM
                {
                    Name = userVM.Name,
                    UserName = userVM.UserName,
                    Email = userVM.Email,
                    PhoneNumber = userVM.PhoneNumber,
                    IsActive = userVM.IsActive
                };

                ViewBag.UserId = id;
                ViewBag.UserName = userVM.Name;
                return View(updateUserVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading user for editing: {id}");
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Users));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(string id, UpdateUserVM model)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.UserId = id;
                return View(model);
            }

            try
            {
                // Set the CenterId to the current admin's center (same as current user's center)
                model.CenterId = _tenantContext.GetCurrentCenterId();
                
                var updateUserDTO = _mapper.Map<Application.DTOs.User.UpdateUserDTO>(model);
                await _userManagementService.UpdateUserAsync(id, updateUserDTO);
                
                TempData["SuccessMessage"] = "Team member updated successfully!";
                return RedirectToAction(nameof(UserDetails), new { id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating user: {id}");
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                ViewBag.UserId = id;
                return View(model);
            }
        }

        // Activate User
        [HttpPost]
        public async Task<IActionResult> ActivateUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Json(new { success = false, message = "Invalid ID" });
            }

            try
            {
                await _userManagementService.ActivateUserAsync(id);
                _logger.LogInformation($"User {id} activated");
                return Json(new { success = true, message = "Activated successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error activating user {id}");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // Deactivate User
        [HttpPost]
        public async Task<IActionResult> DeactivateUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Json(new { success = false, message = "Invalid ID" });
            }

            try
            {
                await _userManagementService.DeactivateUserAsync(id);
                _logger.LogInformation($"User {id} deactivated");
                return Json(new { success = true, message = "Deactivated successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deactivating user {id}");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // Soft Delete User
        [HttpPost]
        public async Task<IActionResult> SoftDeleteUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Json(new { success = false, message = "Invalid ID" });
            }

            try
            {
                await _userManagementService.SoftDeleteUserAsync(id);
                _logger.LogInformation($"User {id} deleted");
                return Json(new { success = true, message = "Removed successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting user {id}");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // Restore User
        [HttpPost]
        public async Task<IActionResult> RestoreUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Json(new { success = false, message = "Invalid ID" });
            }

            try
            {
                await _userManagementService.RestoreUserAsync(id);
                _logger.LogInformation($"User {id} restored");
                return Json(new { success = true, message = "Restored successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error restoring user {id}");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // Change User Role
        [HttpPost]
        public async Task<IActionResult> ChangeUserRole(string id, string newRole)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Json(new { success = false, message = "Invalid ID" });
            }

            if (string.IsNullOrEmpty(newRole))
            {
                return Json(new { success = false, message = "Please select a member type" });
            }

            try
            {
                // Validate role
                if (!Enum.TryParse<RolesNames>(newRole, out var roleEnum))
                {
                    return Json(new { success = false, message = "Invalid member type" });
                }

                // Only allow Employee and Manager roles
                if (roleEnum != RolesNames.Employee && roleEnum != RolesNames.Manager)
                {
                    return Json(new { success = false, message = "You can only assign Employee or Manager types" });
                }

                // Update user role
                await _userManagementService.UpdateUserRoleAsync(id, newRole);
                
                _logger.LogInformation($"User {id} role changed to {newRole}");
                
                return Json(new { 
                    success = true, 
                    message = $"Member type changed to {(roleEnum == RolesNames.Employee ? "Employee" : "Manager")} successfully!" 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error changing role for user {id}");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }
    }
}


