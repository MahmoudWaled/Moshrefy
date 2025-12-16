using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moshrefy.Application.DTOs.AcademicYear;
using Moshrefy.Application.Interfaces.IServices;
using Moshrefy.Domain.Paramter;
using Moshrefy.Web.Models.AcademicYear;

namespace Moshrefy.Web.Controllers
{
    [Authorize(Policy = "AcademicYear.View")]
    public class AcademicYearController : Controller
    {
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

        // GET: AcademicYear/Index
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // POST: AcademicYear/GetAcademicYearsData (AJAX for DataTables)
        [HttpPost]
        public async Task<IActionResult> GetAcademicYearsData()
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

                List<AcademicYearResponseDTO> academicYearsDTO;

                try
                {
                    academicYearsDTO = await _academicYearService.GetAllAcademicYearsAsync(paginationParams);
                }
                catch (Exception)
                {
                    // No data found, return empty list
                    academicYearsDTO = new List<AcademicYearResponseDTO>();
                }

                var academicYearsVM = _mapper.Map<List<AcademicYearVM>>(academicYearsDTO);

                // Apply status filter
                if (!string.IsNullOrEmpty(filterStatus))
                {
                    if (filterStatus == "active")
                    {
                        academicYearsVM = academicYearsVM.Where(ay => ay.IsActive).ToList();
                    }
                    else if (filterStatus == "inactive")
                    {
                        academicYearsVM = academicYearsVM.Where(ay => !ay.IsActive).ToList();
                    }
                }

                // Apply search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    academicYearsVM = academicYearsVM.Where(ay =>
                        ay.Name.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ||
                        (ay.CreatedByName != null && ay.CreatedByName.Contains(searchValue, StringComparison.OrdinalIgnoreCase))
                    ).ToList();
                }

                var jsonData = new
                {
                    draw = draw,
                    recordsTotal = academicYearsVM.Count,
                    recordsFiltered = academicYearsVM.Count,
                    data = academicYearsVM
                };

                return Ok(jsonData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading academic years data");
                return StatusCode(500, new { error = "Error loading data. Please try again." });
            }
        }

        // GET: AcademicYear/Details/5
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

        // GET: AcademicYear/Create
        [HttpGet]
        [Authorize(Policy = "AcademicYear.Add")]
        public IActionResult Create()
        {
            return View(new CreateAcademicYearVM());
        }

        // POST: AcademicYear/Create
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

        // GET: AcademicYear/Edit/5
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

        // POST: AcademicYear/Edit/5
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

        // POST: AcademicYear/Delete/5
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

        // GET: AcademicYear/GetCourses/5 (AJAX)
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
    }
}
