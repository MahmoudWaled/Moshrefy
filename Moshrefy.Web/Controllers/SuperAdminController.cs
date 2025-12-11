using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moshrefy.Application.DTOs.Center;
using Moshrefy.Application.Interfaces.IServices;
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

        public SuperAdminController(ISuperAdminService superAdminService, IMapper mapper)
        {
            _superAdminService = superAdminService;
            _mapper = mapper;
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
        public async Task<IActionResult> Centers()
        {
            var paginationParams = new PaginationParamter
            {
                PageNumber = 1,
                PageSize = 30
            };

            var centersDTO = await _superAdminService.GetAllCentersAsync(paginationParams);
            var centersVM = _mapper.Map<List<CenterVM>>(centersDTO);

            return View(centersVM);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCenter(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            try
            {
                await _superAdminService.DeleteCenterAsync(id);
                TempData["SuccessMessage"] = "Center deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting center: {ex.Message}";
            }

            return RedirectToAction(nameof(Centers));
        }

        #endregion
    }
}
