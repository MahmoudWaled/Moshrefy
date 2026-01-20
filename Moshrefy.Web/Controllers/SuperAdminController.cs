using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moshrefy.Application.DTOs.Center;
using Moshrefy.Application.Interfaces.IServices;
using Moshrefy.Application.Services;
using Moshrefy.Domain.Enums;
using Moshrefy.Domain.Paramter;
using Moshrefy.Web.Models.Center;
using Moshrefy.Web.Models.Statistics;
using Moshrefy.Web.Extensions;
using Moshrefy.Application.DTOs.Common;
using System;

namespace Moshrefy.Web.Controllers
{
    [Authorize(Roles = nameof(RolesNames.SuperAdmin))]
    public class SuperAdminController : Controller
    {
        #region Dependencies

        private readonly ISuperAdminService _superAdminService;
        private readonly IMapper _mapper;
        private readonly ILogger<SuperAdminController> _logger;
        private readonly ICenterService _centerService;
        public SuperAdminController(ISuperAdminService superAdminService, IMapper mapper ,ILogger<SuperAdminController> logger , ICenterService centerService )
        {
            _superAdminService = superAdminService;
            _mapper = mapper;
            _logger = logger;
            _centerService = centerService;
        }

        #endregion

        #region Dashboard

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var statsDTO = await _superAdminService.GetSystemStatisticsAsync();
            var statsVM = _mapper.Map<SystemStatisticsVM>(statsDTO);
            return View(statsVM);
        }

        [HttpGet]
        public async Task<IActionResult> DeletedItems()
        {
            var statsDTO = await _superAdminService.GetSystemStatisticsAsync();
            var statsVM = _mapper.Map<SystemStatisticsVM>(statsDTO);
            return View(statsVM);
        }

        #endregion

        #region Center Management

        // List all centers
        [HttpGet]
        public IActionResult Centers()
        {
            return View();
        }

        // DataTables - Get centers data
        [HttpPost]
        public async Task<IActionResult> GetCentersData()
        {
            try
            {
                var centers = await _centerService.GetNonDeletedAsync(new PaginationParamter { PageSize = 100 });
                var centersVM = _mapper.Map<List<CenterVM>>(centers);
                
                return Ok(new { data = centersVM });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading centers data");
                return StatusCode(500, new { error = "Error loading data. Please try again." });
            }
        }

        // Advanced search page
        [HttpGet]
        public IActionResult AdvancedSearchCenters()
        {
            return View("SearchCenters");
        }

        // DataTables - Advanced center search
        //[HttpPost]
        //public async Task<IActionResult> SearchCentersData(string? centerName, string? email, string? createdByName, string? adminName)
        //{
        //    try
        //    {
        //        var request = Request.GetDataTableRequest();
                
        //        // Override with method parameters if provided
        //        if (!string.IsNullOrEmpty(centerName)) request.CenterName = centerName;
        //        if (!string.IsNullOrEmpty(email)) request.Email = email;
        //        if (!string.IsNullOrEmpty(createdByName)) request.CreatedByName = createdByName;
        //        if (!string.IsNullOrEmpty(adminName)) request.AdminName = adminName;
                
        //        var response = await _superAdminService.GetCentersDataTableAsync(request);
        //        var centersVM = _mapper.Map<List<CenterVM>>(response.Data);

        //        return Ok(new
        //        {
        //            draw = response.Draw,
        //            recordsTotal = response.RecordsTotal,
        //            recordsFiltered = response.RecordsFiltered,
        //            data = centersVM
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error loading centers search data");
        //        return StatusCode(500, new { error = "Error loading data. Please try again." });
        //    }
        //}

        // View center details
        [HttpGet]
        public async Task<IActionResult> CenterDetails(int centerId)
        {
            if (centerId <= 0)
            {
                return NotFound();
            }

            var centerDTO = await _centerService.GetByIdAsync(centerId);
            var centerVM = _mapper.Map<CenterVM>(centerDTO);
            return View(centerVM);
        }

        // Create center - GET
        [HttpGet]
        public IActionResult CreateCenter()
        {
            return View();
        }

        // Create center - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCenter(CreateCenterVM createCenterVM)
        {
            if (!ModelState.IsValid)
            {
                return View(createCenterVM);
            }

            try
            {
                var createCenterDTO = _mapper.Map<CreateCenterDTO>(createCenterVM);
                await _centerService.CreateAsync(createCenterDTO);
                TempData["SuccessMessage"] = "Center created successfully!";
                return RedirectToAction(nameof(Centers));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error creating center: {ex.Message}";
                return View(createCenterVM);
            }
        }

        // Edit center - GET
        [HttpGet]
        public async Task<IActionResult> EditCenter(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            try
            {
                var centerDTO = await _centerService.GetByIdAsync(id);
                var centerVM = _mapper.Map<CenterVM>(centerDTO);
                
                var updateCenterVM = new UpdateCenterVM
                {
                    Name = centerVM.Name,
                    Address = centerVM.Address,
                    Description = centerVM.Description,
                    Email = centerVM.Email,
                    Phone = centerVM.Phone,
                    IsActive = centerVM.IsActive
                };

                ViewBag.CenterId = id;
                return View(updateCenterVM);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading center: {ex.Message}";
                return RedirectToAction(nameof(Centers));
            }
        }

        // Edit center - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCenter(int id, UpdateCenterVM updateCenterVM)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(updateCenterVM);
            }

            try
            {
                var updateCenterDTO = _mapper.Map<Application.DTOs.Center.UpdateCenterDTO>(updateCenterVM);
                await _centerService.UpdateAsync(id, updateCenterDTO);
                TempData["SuccessMessage"] = "Center updated successfully!";
                return RedirectToAction(nameof(CenterDetails), new { centerId = id });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating center: {ex.Message}";
                return View(updateCenterVM);
            }
        }

        // Activate center
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateCenter(int id)
        {
            if (id <= 0)
            {
                return Json(new { success = false, message = "Invalid center ID" });
            }

            try
            {
                await _centerService.ActivateAsync(id);
                _logger.LogInformation($"Center with ID {id} activated by SuperAdmin.", id);
                return Json(new { success = true, message = "Center activated successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error activating center with ID {id}.", id);
                return Json(new { success = false, message = $"Error activating center: {ex.Message}" });
            }
        }

        // Deactivate center
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateCenter(int id)
        {
            if (id <= 0)
            {
                return Json(new { success = false, message = "Invalid center ID" });
            }

            try
            {
                await _centerService.DeactivateAsync(id);
                _logger.LogInformation($"Center with ID {id} deactivated by SuperAdmin.", id);
                return Json(new { success = true, message = "Center deactivated successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deactivating center with ID {id}.", id);
                return Json(new { success = false, message = $"Error deactivating center: {ex.Message}" });
            }
        }

        // Soft delete center
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoftDeleteCenter(int id)
        {
            if (id <= 0)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.ContentType?.Contains("application/json") == true)
                {
                    return Json(new { success = false, message = "Invalid center ID" });
                }
                return NotFound();
            }

            try
            {
                await _centerService.SoftDeleteAsync(id);
                _logger.LogInformation($"Center with ID {id} soft deleted by SuperAdmin.", id);
                
                // Check if AJAX request
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.ContentType?.Contains("application/json") == true || Request.Headers["Accept"].ToString().Contains("application/json"))
                {
                    return Json(new { success = true, message = "Center moved to trash! You can restore it later." });
                }
                
                TempData["SuccessMessage"] = "Center moved to trash! You can restore it later.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error soft deleting center with ID {id}.", id);
                
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.ContentType?.Contains("application/json") == true || Request.Headers["Accept"].ToString().Contains("application/json"))
                {
                    return Json(new { success = false, message = $"Error deleting center: {ex.Message}" });
                }
                
                TempData["ErrorMessage"] = $"Error deleting center: {ex.Message}";
            }

            return RedirectToAction(nameof(Centers));
        }

        // Hard delete center (permanent)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HardDeleteCenter(int id)
        {
            if (id <= 0)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.ContentType?.Contains("application/json") == true)
                {
                    return Json(new { success = false, message = "Invalid center ID" });
                }
                return NotFound();
            }

            try
            {
                await _centerService.HardDeleteAsync(id);
                _logger.LogWarning($"Center with ID {id} permanently deleted by SuperAdmin.", id);
                
                // Check if AJAX request
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.ContentType?.Contains("application/json") == true || Request.Headers["Accept"].ToString().Contains("application/json"))
                {
                    return Json(new { success = true, message = "Center permanently deleted!" });
                }
                
                TempData["SuccessMessage"] = "Center permanently deleted!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error permanently deleting center with ID {id}.", id);
                
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.ContentType?.Contains("application/json") == true || Request.Headers["Accept"].ToString().Contains("application/json"))
                {
                    return Json(new { success = false, message = $"Error permanently deleting center: {ex.Message}" });
                }
                
                TempData["ErrorMessage"] = $"Error permanently deleting center: {ex.Message}";
            }

            return RedirectToAction(nameof(Centers));
        }

        // Restore center
        [HttpPost]
        public async Task<IActionResult> RestoreCenter(int id)
        {
            if (id <= 0)
            {
                return Json(new { success = false, message = "Invalid center ID" });
            }

            try
            {
                await _centerService.RestoreAsync(id);
                _logger.LogInformation($"Center with ID {id} restored by SuperAdmin.", id);
                return Json(new { success = true, message = "Center restored successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error restoring center with ID {id}.", id);
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        #endregion

        #region User Management

        // List all users
        [HttpGet]
        public IActionResult Users()
        {
            return View();
        }

        // DataTables - Get users data
        [HttpPost]
        public async Task<IActionResult> GetUsersData()
        {
            try
            {
                var request = Request.GetDataTableRequest();
                var response = await _superAdminService.GetUsersDataTableAsync(request);
                var usersVM = _mapper.Map<List<Models.User.UserVM>>(response.Data);

                return Ok(new
                {
                    draw = response.Draw,
                    recordsTotal = response.RecordsTotal,
                    recordsFiltered = response.RecordsFiltered,
                    data = usersVM
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading users data for DataTables");
                return StatusCode(500, new { error = "Error loading data. Please try again." });
            }
        }

        // Create center admin - GET
        [HttpGet]
        public async Task<IActionResult> CreateCenterAdmin()
        {
            var createUserVM = new Models.User.CreateUserVM
            {
                Centers = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                    await GetActiveCentersForDropdown(), 
                    "Value", 
                    "Text"
                ),
                RoleName = RolesNames.Admin
            };
            return View(createUserVM);
        }

        // Create center admin - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCenterAdmin(Models.User.CreateUserVM createUserVM)
        {
            if (!ModelState.IsValid)
            {
                createUserVM.Centers = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                    await GetActiveCentersForDropdown(),
                    "Value",
                    "Text",
                    createUserVM.CenterId
                );
                return View(createUserVM);
            }

            if (!createUserVM.CenterId.HasValue)
            {
                ModelState.AddModelError("CenterId", "Please select a center");
                createUserVM.Centers = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                    await GetActiveCentersForDropdown(),
                    "Value",
                    "Text"
                );
                return View(createUserVM);
            }

            try
            {
                var createUserDTO = _mapper.Map<Application.DTOs.User.CreateUserDTO>(createUserVM);
                await _superAdminService.CreateCenterAdminAsync(createUserVM.CenterId.Value, createUserDTO);
                TempData["SuccessMessage"] = "Center admin created successfully!";
                return RedirectToAction(nameof(Users));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating center admin");
                TempData["ErrorMessage"] = $"Error creating admin: {ex.Message}";
                createUserVM.Centers = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                    await GetActiveCentersForDropdown(),
                    "Value",
                    "Text",
                    createUserVM.CenterId
                );
                return View(createUserVM);
            }
        }

        // Create user for center - GET
        [HttpGet]
        public async Task<IActionResult> CreateUserForCenter(int? centerId = null)
        {
            var createUserVM = new Models.User.CreateUserVM
            {
                Centers = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                    await GetActiveCentersForDropdown(),
                    "Value",
                    "Text",
                    centerId?.ToString()
                ),
                CenterId = centerId
            };

            // If centerId is provided, get center name for display
            if (centerId.HasValue && centerId.Value > 0)
            {
                try
                {
                    var centerDTO = await _centerService.GetByIdAsync(centerId.Value);
                    ViewBag.CenterName = centerDTO.Name;
                    ViewBag.PreSelectedCenter = true;
                }
                catch
                {
                    // Continue without pre-selection
                }
            }

            return View(createUserVM);
        }

        // Create user for center - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUserForCenter(Models.User.CreateUserVM createUserVM, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                createUserVM.Centers = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                    await GetActiveCentersForDropdown(),
                    "Value",
                    "Text",
                    createUserVM.CenterId
                );
                return View(createUserVM);
            }

            if (!createUserVM.CenterId.HasValue)
            {
                ModelState.AddModelError("CenterId", "Please select a center");
                createUserVM.Centers = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                    await GetActiveCentersForDropdown(),
                    "Value",
                    "Text"
                );
                return View(createUserVM);
            }

            try
            {
                var createUserDTO = _mapper.Map<Application.DTOs.User.CreateUserDTO>(createUserVM);
                await _superAdminService.CreateUserForCenterAsync(createUserVM.CenterId.Value, createUserDTO);
                TempData["SuccessMessage"] = "User created successfully!";

                // Redirect based on return URL or referrer
                if (!string.IsNullOrEmpty(returnUrl) && returnUrl.Contains("CenterUsers"))
                {
                    return Redirect(returnUrl);
                }
                else if (createUserVM.CenterId.HasValue)
                {
                    var referer = Request.Headers["Referer"].ToString();
                    if (referer.Contains("CenterUsers"))
                    {
                        return RedirectToAction(nameof(CenterUsers), new { centerId = createUserVM.CenterId.Value });
                    }
                }

                return RedirectToAction(nameof(Users));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user for center");
                TempData["ErrorMessage"] = $"Error creating user: {ex.Message}";
                createUserVM.Centers = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                    await GetActiveCentersForDropdown(),
                    "Value",
                    "Text",
                    createUserVM.CenterId
                );
                return View(createUserVM);
            }
        }

        // Restore user
        [HttpPost]
        public async Task<IActionResult> RestoreUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Json(new { success = false, message = "Invalid user ID" });
            }

            try
            {
                await _superAdminService.RestoreUserAsync(id);
                _logger.LogInformation($"User with ID {id} restored by SuperAdmin.");
                return Json(new { success = true, message = "User restored successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error restoring user with ID {id}.");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // Activate user
        [HttpPost]
        public async Task<IActionResult> ActivateUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Json(new { success = false, message = "Invalid user ID" });
            }

            try
            {
                await _superAdminService.ActivateUserAsync(id);
                _logger.LogInformation($"User with ID {id} activated by SuperAdmin.");
                return Json(new { success = true, message = "User activated successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error activating user with ID {id}.");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // Deactivate user
        [HttpPost]
        public async Task<IActionResult> DeactivateUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Json(new { success = false, message = "Invalid user ID" });
            }

            try
            {
                await _superAdminService.DeactivateUserAsync(id);
                _logger.LogInformation($"User with ID {id} deactivated by SuperAdmin.");
                return Json(new { success = true, message = "User deactivated successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deactivating user with ID {id}.");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // Soft delete user
        [HttpPost]
        public async Task<IActionResult> SoftDeleteUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Json(new { success = false, message = "Invalid user ID" });
            }

            try
            {
                await _superAdminService.SoftDeleteUserAsync(id);
                _logger.LogInformation($"User with ID {id} soft deleted by SuperAdmin.");
                return Json(new { success = true, message = "User moved to trash!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error soft deleting user with ID {id}.");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // Hard delete user (permanent)
        [HttpPost]
        public async Task<IActionResult> HardDeleteUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Json(new { success = false, message = "Invalid user ID" });
            }

            try
            {
                await _superAdminService.DeleteUserAsync(id);
                _logger.LogWarning($"User with ID {id} permanently deleted by SuperAdmin.");
                return Json(new { success = true, message = "User permanently deleted!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error permanently deleting user with ID {id}.");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // View user details
        [HttpGet]
        public async Task<IActionResult> UserDetails(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            try
            {
                var userDTO = await _superAdminService.GetUserByIdAsync(id);
                var userVM = _mapper.Map<Models.User.UserVM>(userDTO);
                return View(userVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading user details for ID {id}");
                TempData["ErrorMessage"] = $"Error loading user: {ex.Message}";
                return RedirectToAction(nameof(Users));
            }
        }

        // Edit user - GET
        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            try
            {
                var userDTO = await _superAdminService.GetUserByIdAsync(id);
                var userVM = _mapper.Map<Models.User.UserVM>(userDTO);

                var updateUserVM = new Models.User.UpdateUserVM
                {
                    Name = userVM.Name,
                    UserName = userVM.UserName,
                    Email = userVM.Email,
                    PhoneNumber = userVM.PhoneNumber,
                    IsActive = userVM.IsActive,
                    Centers = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                        await GetActiveCentersForDropdown(),
                        "Value",
                        "Text",
                        userDTO.CenterName
                    )
                };

                ViewBag.UserId = id;
                return View(updateUserVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading user for editing: {id}");
                TempData["ErrorMessage"] = $"Error loading user: {ex.Message}";
                return RedirectToAction(nameof(Users));
            }
        }

        // Edit user - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(string id, Models.User.UpdateUserVM updateUserVM)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                updateUserVM.Centers = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                    await GetActiveCentersForDropdown(),
                    "Value",
                    "Text",
                    updateUserVM.CenterId
                );
                ViewBag.UserId = id;
                return View(updateUserVM);
            }

            try
            {
                var updateUserDTO = _mapper.Map<Application.DTOs.User.UpdateUserDTO>(updateUserVM);
                await _superAdminService.UpdateUserInAnyCenterAsync(id, updateUserDTO);
                TempData["SuccessMessage"] = "User updated successfully!";
                return RedirectToAction(nameof(UserDetails), new { id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating user: {id}");
                TempData["ErrorMessage"] = $"Error updating user: {ex.Message}";
                updateUserVM.Centers = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                    await GetActiveCentersForDropdown(),
                    "Value",
                    "Text",
                    updateUserVM.CenterId
                );
                ViewBag.UserId = id;
                return View(updateUserVM);
            }
        }

        // Search users page
        [HttpGet]
        public IActionResult SearchUsers()
        {
            return View();
        }

        // Search user by email (AJAX)
        [HttpPost]
        public async Task<IActionResult> SearchUserByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return Json(new { success = false, message = "Email is required" });
            }

            try
            {
                var userDTO = await _superAdminService.GetUserByEmailAsync(email);
                var userVM = _mapper.Map<Models.User.UserVM>(userDTO);
                return Json(new { success = true, user = userVM });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error searching user by email: {email}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Search user by username (AJAX)
        [HttpPost]
        public async Task<IActionResult> SearchUserByUsername(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return Json(new { success = false, message = "Username is required" });
            }

            try
            {
                var userDTO = await _superAdminService.GetUserByUsernameAsync(username);
                var userVM = _mapper.Map<Models.User.UserVM>(userDTO);
                return Json(new { success = true, user = userVM });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error searching user by username: {username}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Inactive users page
        [HttpGet]
        public IActionResult InactiveUsers()
        {
            return View();
        }

        // DataTables - Get inactive users data
        [HttpPost]
        public async Task<IActionResult> GetInactiveUsersData()
        {
            try
            {
                // Parse DataTables parameters
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                var sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
                var sortColumnName = Request.Form[$"columns[{sortColumnIndex}][name]"].FirstOrDefault();
                var sortDirection = Request.Form["order[0][dir]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 25;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int pageNumber = (skip / pageSize) + 1;

                var paginationParams = new PaginationParamter
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                int totalRecords = await _superAdminService.GetInactiveUsersCountAsync();
                int filteredRecords = totalRecords;

                var usersDTO = await _superAdminService.GetInactiveUsersAsync(paginationParams);
                var usersVM = _mapper.Map<List<Models.User.UserVM>>(usersDTO);

                // Apply search filter
                if (!string.IsNullOrEmpty(searchValue))
                {
                    usersVM = usersVM.Where(u =>
                        u.Name.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ||
                        u.UserName.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ||
                        (u.Email != null && u.Email.Contains(searchValue, StringComparison.OrdinalIgnoreCase)) ||
                        (u.PhoneNumber != null && u.PhoneNumber.Contains(searchValue, StringComparison.OrdinalIgnoreCase)) ||
                        (u.CenterName != null && u.CenterName.Contains(searchValue, StringComparison.OrdinalIgnoreCase)) ||
                        (u.RoleName != null && u.RoleName.Contains(searchValue, StringComparison.OrdinalIgnoreCase)) ||
                        u.Id.Contains(searchValue)
                    ).ToList();

                    filteredRecords = usersVM.Count;
                }

                // Apply sorting
                if (!string.IsNullOrEmpty(sortColumnName) && !string.IsNullOrEmpty(sortDirection))
                {
                    usersVM = sortDirection.ToLower() == "asc"
                        ? SortUsersAscending(usersVM, sortColumnName)
                        : SortUsersDescending(usersVM, sortColumnName);
                }

                return Ok(new
                {
                    draw = draw,
                    recordsTotal = totalRecords,
                    recordsFiltered = filteredRecords,
                    data = usersVM
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading inactive users data");
                return StatusCode(500, new { error = "Error loading data. Please try again." });
            }
        }

        // Update user role - GET
        [HttpGet]
        public async Task<IActionResult> UpdateUserRole(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            try
            {
                var userDTO = await _superAdminService.GetUserByIdAsync(id);
                var userVM = _mapper.Map<Models.User.UserVM>(userDTO);

                var updateUserRoleVM = new Models.User.UpdateUserRoleVM();
                
                if (!string.IsNullOrEmpty(userVM.RoleName) && Enum.TryParse<RolesNames>(userVM.RoleName, out var currentRole))
                {
                    updateUserRoleVM.Role = currentRole;
                }

                ViewBag.UserId = id;
                ViewBag.UserName = userVM.UserName;
                ViewBag.CurrentRole = userVM.RoleName;

                return View(updateUserRoleVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading user for role update: {id}");
                TempData["ErrorMessage"] = $"Error loading user: {ex.Message}";
                return RedirectToAction(nameof(Users));
            }
        }

        // Update user role - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUserRole(string id, Models.User.UpdateUserRoleVM updateUserRoleVM)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                try
                {
                    var userDTO = await _superAdminService.GetUserByIdAsync(id);
                    var userVM = _mapper.Map<Models.User.UserVM>(userDTO);
                    ViewBag.UserId = id;
                    ViewBag.UserName = userVM.UserName;
                    ViewBag.CurrentRole = userVM.RoleName;
                }
                catch { }
                return View(updateUserRoleVM);
            }

            try
            {
                var userBeforeUpdate = await _superAdminService.GetUserByIdAsync(id);
                var updateUserRoleDTO = _mapper.Map<Application.DTOs.User.UpdateUserRoleDTO>(updateUserRoleVM);
                await _superAdminService.UpdateUserRoleAsync(id, updateUserRoleDTO);
                
                var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                
                if (id == currentUserId)
                {
                    TempData["SuccessMessage"] = $"Your role has been updated from {userBeforeUpdate.RoleName} to {updateUserRoleDTO.Role}. The changes are now active.";
                }
                else
                {
                    TempData["SuccessMessage"] = $"User role updated successfully from {userBeforeUpdate.RoleName} to {updateUserRoleDTO.Role}!";
                }
                
                return RedirectToAction(nameof(UserDetails), new { id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating user role: {id}");
                TempData["ErrorMessage"] = $"Error updating role: {ex.Message}";
                
                try
                {
                    var userDTO = await _superAdminService.GetUserByIdAsync(id);
                    var userVM = _mapper.Map<Models.User.UserVM>(userDTO);
                    ViewBag.UserId = id;
                    ViewBag.UserName = userVM.UserName;
                    ViewBag.CurrentRole = userVM.RoleName;
                }
                catch { }
                return View(updateUserRoleVM);
            }
        }

        // Center users page
        [HttpGet]
        public async Task<IActionResult> CenterUsers(int centerId)
        {
            if (centerId <= 0)
            {
                return NotFound();
            }

            try
            {
                var centerDTO = await _centerService.GetByIdAsync(centerId);
                var centerVM = _mapper.Map<CenterVM>(centerDTO);

                ViewBag.CenterId = centerId;
                ViewBag.CenterName = centerVM.Name;
                ViewBag.CenterAddress = centerVM.Address;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading center users page for center ID {centerId}");
                TempData["ErrorMessage"] = $"Error loading center: {ex.Message}";
                return RedirectToAction(nameof(Centers));
            }
        }

        // DataTables - Get center users data
        [HttpPost]
        public async Task<IActionResult> GetCenterUsersData(int centerId)
        {
            try
            {
                if (centerId <= 0)
                {
                    return BadRequest(new { error = "Invalid center ID" });
                }

                // Parse DataTables parameters
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                var sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
                var sortColumnName = Request.Form[$"columns[{sortColumnIndex}][name]"].FirstOrDefault();
                var sortDirection = Request.Form["order[0][dir]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 25;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int pageNumber = (skip / pageSize) + 1;

                var paginationParams = new PaginationParamter
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                int totalRecords = await _superAdminService.GetUsersByCenterIdCountAsync(centerId);
                int filteredRecords = totalRecords;

                var usersDTO = await _superAdminService.GetUsersByCenterIdAsync(centerId, paginationParams);
                var usersVM = _mapper.Map<List<Models.User.UserVM>>(usersDTO);

                // Apply search filter
                if (!string.IsNullOrEmpty(searchValue))
                {
                    usersVM = usersVM.Where(u =>
                        u.Name.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ||
                        u.UserName.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ||
                        (u.Email != null && u.Email.Contains(searchValue, StringComparison.OrdinalIgnoreCase)) ||
                        (u.PhoneNumber != null && u.PhoneNumber.Contains(searchValue, StringComparison.OrdinalIgnoreCase)) ||
                        (u.RoleName != null && u.RoleName.Contains(searchValue, StringComparison.OrdinalIgnoreCase)) ||
                        u.Id.Contains(searchValue)
                    ).ToList();

                    filteredRecords = usersVM.Count;
                }

                // Apply sorting
                if (!string.IsNullOrEmpty(sortColumnName) && !string.IsNullOrEmpty(sortDirection))
                {
                    usersVM = sortDirection.ToLower() == "asc"
                        ? SortUsersAscending(usersVM, sortColumnName)
                        : SortUsersDescending(usersVM, sortColumnName);
                }

                return Ok(new
                {
                    draw = draw,
                    recordsTotal = totalRecords,
                    recordsFiltered = filteredRecords,
                    data = usersVM
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading center users data for center ID {centerId}");
                return StatusCode(500, new { error = "Error loading data. Please try again." });
            }
        }

        #endregion

        #region Helper Methods

        // Search centers for Select2 dropdown
        [HttpGet]
        public async Task<IActionResult> SearchCenters(string term)
        {
            try
            {
                var centers = await _centerService.GetNonDeletedAsync(new PaginationParamter
                {
                    PageSize = 50,
                    PageNumber = 1
                });

                var filteredCenters = centers
                    .Where(c => string.IsNullOrEmpty(term) || 
                                c.Name.Contains(term, StringComparison.OrdinalIgnoreCase))
                    .Select(c => new
                    {
                        id = c.Id,
                        text = $"{c.Name} - {c.Address}"
                    })
                    .ToList();

                return Json(new { results = filteredCenters });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching centers");
                return Json(new { results = new List<object>() });
            }
        }

        // Get active centers for dropdown
        private async Task<List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>> GetActiveCentersForDropdown()
        {
            var centers = await _centerService.GetNonDeletedAsync(new PaginationParamter
            {
                PageSize = 200,
                PageNumber = 1
            });

            return centers
                .Where(c => c.IsActive)
                .Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = $"{c.Name} - {c.Address}"
                })
                .ToList();
        }

        // Sort users ascending
        private List<Models.User.UserVM> SortUsersAscending(List<Models.User.UserVM> users, string columnName)
        {
            return columnName?.ToLower() switch
            {
                "id" => users.OrderBy(u => u.Id).ToList(),
                "name" => users.OrderBy(u => u.Name).ToList(),
                "username" => users.OrderBy(u => u.UserName).ToList(),
                "email" => users.OrderBy(u => u.Email).ToList(),
                "phonenumber" => users.OrderBy(u => u.PhoneNumber).ToList(),
                "centername" => users.OrderBy(u => u.CenterName).ToList(),
                "rolename" => users.OrderBy(u => u.RoleName).ToList(),
                "isactive" => users.OrderBy(u => u.IsActive).ToList(),
                "isdeleted" => users.OrderBy(u => u.IsDeleted).ToList(),
                "createdat" => users.OrderBy(u => u.CreatedAt).ToList(),
                _ => users
            };
        }

        // Sort users descending
        private List<Models.User.UserVM> SortUsersDescending(List<Models.User.UserVM> users, string columnName)
        {
            return columnName?.ToLower() switch
            {
                "id" => users.OrderByDescending(u => u.Id).ToList(),
                "name" => users.OrderByDescending(u => u.Name).ToList(),
                "username" => users.OrderByDescending(u => u.UserName).ToList(),
                "email" => users.OrderByDescending(u => u.Email).ToList(),
                "phonenumber" => users.OrderByDescending(u => u.PhoneNumber).ToList(),
                "centername" => users.OrderByDescending(u => u.CenterName).ToList(),
                "rolename" => users.OrderByDescending(u => u.RoleName).ToList(),
                "isactive" => users.OrderByDescending(u => u.IsActive).ToList(),
                "isdeleted" => users.OrderByDescending(u => u.IsDeleted).ToList(),
                "createdat" => users.OrderByDescending(u => u.CreatedAt).ToList(),
                _ => users
            };
        }

        #endregion
    }
}
