using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moshrefy.Application.DTOs.AcademicYear;
using Moshrefy.Application.Interfaces.IServices;
using Moshrefy.Domain.Paramter;
using Moshrefy.Web.Models.AcademicYear;
using Moshrefy.Web.Extensions;
using Moshrefy.Application.DTOs.Common;

namespace Moshrefy.Web.Controllers
{
    [Authorize(Policy = "AcademicYear.View")]
    public class AcademicYearController : Controller
    {
        #region Dependencies

        private readonly IAcademicYearService _academicYearService;
        private readonly ICourseService _courseService;
        private readonly IMapper _mapper;
        private readonly ILogger<AcademicYearController> _logger;

        public AcademicYearController(
            IAcademicYearService academicYearService,
            ICourseService courseService,
            IMapper mapper,
            ILogger<AcademicYearController> logger)
        {
            _academicYearService = academicYearService;
            _courseService = courseService;
            _mapper = mapper;
            _logger = logger;
        }

        #endregion

        #region Academic Year Management

        // List all academic years
        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var paginationParameter = new PaginationParameter { PageNumber = page, PageSize = pageSize };
            var result = await _academicYearService.GetAcademicYearsPagedAsync(paginationParameter);
            var mappedResult = _mapper.Map<PaginatedResult<AcademicYearVM>>(result);

            return View(mappedResult);
        }



        // View academic year details
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            try
            {
                var academicYearDTO = await _academicYearService.GetAcademicYearByIdAsync(id);
                var academicYearVM = _mapper.Map<AcademicYearVM>(academicYearDTO);
                return View(academicYearVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading academic year details for ID {id}");
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // Create academic year - GET
        [HttpGet]
        [Authorize(Policy = "AcademicYear.Add")]
        public IActionResult Create()
        {
            return View(new CreateAcademicYearVM());
        }

        // Create academic year - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AcademicYear.Add")]
        public async Task<IActionResult> Create(CreateAcademicYearVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var createDTO = _mapper.Map<CreateAcademicYearDTO>(model);
                await _academicYearService.AddAcademicYearAsync(createDTO);
                TempData["SuccessMessage"] = "Academic year created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating academic year");
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return View(model);
            }
        }

        // Edit academic year - GET
        [HttpGet]
        [Authorize(Policy = "AcademicYear.Update")]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            try
            {
                var academicYearDTO = await _academicYearService.GetAcademicYearByIdAsync(id);
                var updateVM = new UpdateAcademicYearVM
                {
                    Name = academicYearDTO.Name,
                    IsActive = academicYearDTO.IsActive
                };

                ViewBag.AcademicYearId = id;
                ViewBag.AcademicYearName = academicYearDTO.Name;
                return View(updateVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading academic year for editing: {id}");
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // Edit academic year - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AcademicYear.Update")]
        public async Task<IActionResult> Edit(int id, UpdateAcademicYearVM model)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.AcademicYearId = id;
                return View(model);
            }

            try
            {
                var updateDTO = _mapper.Map<UpdateAcademicYearDTO>(model);
                await _academicYearService.UpdateAcademicYearAsync(id, updateDTO);
                TempData["SuccessMessage"] = "Academic year updated successfully!";
                return RedirectToAction(nameof(Details), new { id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating academic year: {id}");
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                ViewBag.AcademicYearId = id;
                return View(model);
            }
        }

        // Delete academic year
        [HttpPost]
        [Authorize(Policy = "AcademicYear.Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return Json(new { success = false, message = "Invalid ID" });
            }

            try
            {
                await _academicYearService.DeleteAcademicYearAsync(id);
                _logger.LogInformation($"Academic year {id} deleted");
                return Json(new { success = true, message = "Academic year deleted successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting academic year {id}");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // Get courses for academic year (AJAX)
        [HttpGet]
        public async Task<IActionResult> GetCourses(int id)
        {
            try
            {
                var courses = await _courseService.GetByAcademicYearIdAsync(id);
                var result = courses
                    .Where(c => !c.IsDeleted)
                    .Select(c => new
                    {
                        id = c.Id,
                        name = c.Name,
                        isActive = c.IsActive
                    })
                    .ToList();
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading courses for academic year {id}");
                return Json(new List<object>());
            }
        }

        #endregion
    }
}
