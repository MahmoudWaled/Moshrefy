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
using System;

namespace Moshrefy.Web.Controllers
{
    [Authorize(Roles = nameof(RolesNames.SuperAdmin))]
    public class SuperAdminController : Controller
    {
        private readonly ISuperAdminService _superAdminService;
        private readonly IMapper _mapper;
        private readonly ILogger<SuperAdminController> _logger;

        public SuperAdminController(ISuperAdminService superAdminService, IMapper mapper ,ILogger<SuperAdminController> logger )
        {
            _superAdminService = superAdminService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var statsDTO = await _superAdminService.GetSystemStatisticsAsync();
            var statsVM = _mapper.Map<SystemStatisticsVM>(statsDTO);
            return View(statsVM);
        }

        #region Center Management

        [HttpGet]
        public IActionResult Centers()
        {
            // Return empty view - data will be loaded via AJAX
            return View();
        }

        // AJAX endpoint for DataTables server-side processing
        [HttpPost]
        public async Task<IActionResult> GetCentersData()
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

                // Parse pagination
                int pageSize = length != null ? Convert.ToInt32(length) : 25;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int pageNumber = (skip / pageSize) + 1;

                // Get custom filter parameter
                var filterDeleted = Request.Form["filterDeleted"].FirstOrDefault();

                var paginationParams = new PaginationParamter
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                // Get data based on filter
                List<CenterResponseDTO> centersDTO;
                int totalRecords;
                int filteredRecords;

                if (filterDeleted == "deleted")
                {
                    // Show only deleted
                    centersDTO = await _superAdminService.GetDeletedCentersAsync(paginationParams);
                    totalRecords = await _superAdminService.GetDeletedCentersCountAsync();
                    filteredRecords = totalRecords;
                }
                else if (filterDeleted == "active")
                {
                    // Hide deleted
                    centersDTO = await _superAdminService.GetNonDeletedCentersAsync(paginationParams);
                    totalRecords = await _superAdminService.GetNonDeletedCentersCountAsync();
                    filteredRecords = totalRecords;
                }
                else
                {
                    // Show all
                    centersDTO = await _superAdminService.GetAllCentersAsync(paginationParams);
                    totalRecords = await _superAdminService.GetTotalCentersCountAsync();
                    filteredRecords = totalRecords;
                }

                var centersVM = _mapper.Map<List<CenterVM>>(centersDTO);

                // Apply search filter if provided
                if (!string.IsNullOrEmpty(searchValue))
                {
                    centersVM = centersVM.Where(c =>
                        c.Name.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ||
                        (c.Email != null && c.Email.Contains(searchValue, StringComparison.OrdinalIgnoreCase)) ||
                        c.Phone.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ||
                        c.Address.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ||
                        c.Id.ToString().Contains(searchValue)
                    ).ToList();

                    filteredRecords = centersVM.Count;
                }

                // Apply sorting
                if (!string.IsNullOrEmpty(sortColumnName) && !string.IsNullOrEmpty(sortDirection))
                {
                    centersVM = sortDirection.ToLower() == "asc" 
                        ? SortCentersAscending(centersVM, sortColumnName)
                        : SortCentersDescending(centersVM, sortColumnName);
                }

                // Return JSON response for DataTables
                var jsonData = new
                {
                    draw = draw,
                    recordsTotal = totalRecords,
                    recordsFiltered = filteredRecords,
                    data = centersVM
                };

                return Ok(jsonData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading centers data for DataTables");
                return StatusCode(500, new { error = "Error loading data. Please try again." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> CenterDetails(int centerId)
        {
            if (centerId <= 0)
            {
                return NotFound();
            }

            var centerDTO = await _superAdminService.GetCenterByIdAsync(centerId);
            var centerVM = _mapper.Map<CenterVM>(centerDTO);
            return View(centerVM);
        }


        // Create Center
        [HttpGet]
        public IActionResult CreateCenter()
        {
            return View();
        }

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
                await _superAdminService.CreateCenterAsync(createCenterDTO);
                TempData["SuccessMessage"] = "Center created successfully!";
                return RedirectToAction(nameof(Centers));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error creating center: {ex.Message}";
                return View(createCenterVM);
            }
        }


        // Edit Center
        [HttpGet]
        public async Task<IActionResult> EditCenter(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            try
            {
                var centerDTO = await _superAdminService.GetCenterByIdAsync(id);
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

                return View(updateCenterVM);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading center: {ex.Message}";
                return RedirectToAction(nameof(Centers));
            }
        }

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
                await _superAdminService.UpdateCenterAsync(id, updateCenterDTO);
                TempData["SuccessMessage"] = "Center updated successfully!";
                return RedirectToAction(nameof(CenterDetails), new { centerId = id });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating center: {ex.Message}";
                return View(updateCenterVM);
            }
        }


        // Delete Center (Soft Delete)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoftDeleteCenter(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            try
            {
                await _superAdminService.SoftDeleteCenterAsync(id);
                _logger.LogInformation($"Center with ID {id} soft deleted by SuperAdmin.", id);
                TempData["SuccessMessage"] = "Center moved to trash! You can restore it later.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error soft deleting center with ID {id}.", id);
                TempData["ErrorMessage"] = $"Error deleting center: {ex.Message}";
            }

            return RedirectToAction(nameof(Centers));
        }

        // Hard Delete Center (Permanent)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HardDeleteCenter(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            try
            {
                await _superAdminService.DeleteCenterAsync(id);
                _logger.LogWarning($"Center with ID {id} permanently deleted by SuperAdmin.", id);
                TempData["SuccessMessage"] = "Center permanently deleted!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error permanently deleting center with ID {id}.", id);
                TempData["ErrorMessage"] = $"Error permanently deleting center: {ex.Message}";
            }

            return RedirectToAction(nameof(Centers));
        }

        // Restore Center
        [HttpPost]
        public async Task<IActionResult> RestoreCenter(int id)
        {
            if (id <= 0)
            {
                return Json(new { success = false, message = "Invalid center ID" });
            }

            try
            {
                await _superAdminService.RestoreCenterAsync(id);
                _logger.LogInformation($"Center with ID {id} restored by SuperAdmin.", id);
                return Json(new { success = true, message = "Center restored successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error restoring center with ID {id}.", id);
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // Helper methods for sorting
        private List<CenterVM> SortCentersAscending(List<CenterVM> centers, string columnName)
        {
            return columnName?.ToLower() switch
            {
                "id" => centers.OrderBy(c => c.Id).ToList(),
                "name" => centers.OrderBy(c => c.Name).ToList(),
                "email" => centers.OrderBy(c => c.Email).ToList(),
                "phone" => centers.OrderBy(c => c.Phone).ToList(),
                "address" => centers.OrderBy(c => c.Address).ToList(),
                "isactive" => centers.OrderBy(c => c.IsActive).ToList(),
                "isdeleted" => centers.OrderBy(c => c.IsDeleted).ToList(),
                "createdat" => centers.OrderBy(c => c.CreatedAt).ToList(),
                _ => centers
            };
        }

        private List<CenterVM> SortCentersDescending(List<CenterVM> centers, string columnName)
        {
            return columnName?.ToLower() switch
            {
                "id" => centers.OrderByDescending(c => c.Id).ToList(),
                "name" => centers.OrderByDescending(c => c.Name).ToList(),
                "email" => centers.OrderByDescending(c => c.Email).ToList(),
                "phone" => centers.OrderByDescending(c => c.Phone).ToList(),
                "address" => centers.OrderByDescending(c => c.Address).ToList(),
                "isactive" => centers.OrderByDescending(c => c.IsActive).ToList(),
                "isdeleted" => centers.OrderByDescending(c => c.IsDeleted).ToList(),
                "createdat" => centers.OrderByDescending(c => c.CreatedAt).ToList(),
                _ => centers
            };
        }

        #endregion

        #region User Management

        // List all users
        [HttpGet]
        public IActionResult Users()
        {
            return View();
        }

        // Create Admin for Center
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
                RoleName = RolesNames.Admin // Default to Admin role
            };
            return View(createUserVM);
        }

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

        // Create User for Center
        [HttpGet]
        public async Task<IActionResult> CreateUserForCenter()
        {
            var createUserVM = new Models.User.CreateUserVM
            {
                Centers = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                    await GetActiveCentersForDropdown(),
                    "Value",
                    "Text"
                )
            };
            return View(createUserVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUserForCenter(Models.User.CreateUserVM createUserVM)
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

        // API endpoint to get centers for Select2 dropdown
        [HttpGet]
        public async Task<IActionResult> SearchCenters(string term)
        {
            try
            {
                var centers = await _superAdminService.GetNonDeletedCentersAsync(new PaginationParamter
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

        // Helper method to get active centers for dropdown
        private async Task<List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>> GetActiveCentersForDropdown()
        {
            var centers = await _superAdminService.GetNonDeletedCentersAsync(new PaginationParamter
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

        #endregion
    }
}
