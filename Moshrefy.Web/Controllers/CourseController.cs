using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moshrefy.Application.DTOs.Course;
using Moshrefy.Application.Interfaces.IServices;
using Moshrefy.Domain.Paramter;
using Moshrefy.Web.Models.Course;

namespace Moshrefy.Web.Controllers
{
    [Authorize(Policy = "Course.View")]
    public class CourseController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly IAcademicYearService _academicYearService;
        private readonly ITeacherCourseService _teacherCourseService;
        private readonly IMapper _mapper;
        private readonly ILogger<CourseController> _logger;

        public CourseController(
            ICourseService courseService,
            IAcademicYearService academicYearService,
            ITeacherCourseService teacherCourseService,
            IMapper _mapper,
            ILogger<CourseController> logger)
        {
            _courseService = courseService;
            _academicYearService = academicYearService;
            _teacherCourseService = teacherCourseService;
            this._mapper = _mapper;
            _logger = logger;
        }

        // GET: Course/Index
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // POST: Course/GetCoursesData (AJAX for DataTables)
        [HttpPost]
        public async Task<IActionResult> GetCoursesData()
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
                var filterAcademicYear = Request.Form["filterAcademicYear"].FirstOrDefault();

                // Get total count from database efficiently
                int totalRecords = 0;
                try
                {
                    totalRecords = await _courseService.GetTotalCountAsync();
                }
                catch (Exception)
                {
                    // No data, total is 0
                }

                // Fetch current page (service filters by CenterId at database level)
                var paginationParams = new PaginationParamter
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                List<CourseResponseDTO> coursesDTO;

                try
                {
                    coursesDTO = await _courseService.GetAllAsync(paginationParams);
                }
                catch (Exception)
                {
                    // No data found, return empty list
                    coursesDTO = new List<CourseResponseDTO>();
                }

                var coursesVM = _mapper.Map<List<CourseVM>>(coursesDTO);

                // Apply UI filters (status, academicYear, search) on the current page
                // Note: These are applied client-side on the page data
                if (!string.IsNullOrEmpty(filterStatus))
                {
                    if (filterStatus == "active")
                    {
                        coursesVM = coursesVM.Where(c => c.IsActive).ToList();
                    }
                    else if (filterStatus == "inactive")
                    {
                        coursesVM = coursesVM.Where(c => !c.IsActive).ToList();
                    }
                }

                if (!string.IsNullOrEmpty(filterAcademicYear) && int.TryParse(filterAcademicYear, out int academicYearId) && academicYearId > 0)
                {
                    coursesVM = coursesVM.Where(c => c.AcademicYearId == academicYearId).ToList();
                }

                if (!string.IsNullOrEmpty(searchValue))
                {
                    coursesVM = coursesVM.Where(c =>
                        c.Name.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ||
                        (c.AcademicYearName != null && c.AcademicYearName.Contains(searchValue, StringComparison.OrdinalIgnoreCase))
                    ).ToList();
                }

                var jsonData = new
                {
                    draw = draw,
                    recordsTotal = totalRecords,
                    recordsFiltered = totalRecords,
                    data = coursesVM
                };

                return Ok(jsonData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading courses data");
                return StatusCode(500, new { error = "Error loading data. Please try again." });
            }
        }




        // GET: Course/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            try
            {
                var courseDTO = await _courseService.GetByIdAsync(id);
                var courseVM = _mapper.Map<CourseVM>(courseDTO);
                return View(courseVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading course details for ID {id}");
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Course/Create
        [HttpGet]
        [Authorize(Policy = "Course.Add")]
        public async Task<IActionResult> Create()
        {
            try
            {
                var academicYears = await GetAcademicYearsForDropdown();
                var model = new CreateCourseVM
                {
                    AcademicYears = new SelectList(academicYears, "Value", "Text")
                };
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading create course form");
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Course/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Course.Add")]
        public async Task<IActionResult> Create(CreateCourseVM model)
        {
            if (!ModelState.IsValid)
            {
                model.AcademicYears = new SelectList(await GetAcademicYearsForDropdown(), "Value", "Text");
                return View(model);
            }

            try
            {
                var createDTO = _mapper.Map<CreateCourseDTO>(model);
                await _courseService.CreateAsync(createDTO);
                TempData["SuccessMessage"] = "Course created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating course");
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                model.AcademicYears = new SelectList(await GetAcademicYearsForDropdown(), "Value", "Text");
                return View(model);
            }
        }

        // GET: Course/Edit/5
        [HttpGet]
        [Authorize(Policy = "Course.Update")]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            try
            {
                var courseDTO = await _courseService.GetByIdAsync(id);
                var updateVM = new UpdateCourseVM
                {
                    Name = courseDTO.Name,
                    AcademicYearId = courseDTO.AcademicYearId,
                    IsActive = courseDTO.IsActive,
                    AcademicYears = new SelectList(await GetAcademicYearsForDropdown(), "Value", "Text", courseDTO.AcademicYearId)
                };

                ViewBag.CourseId = id;
                ViewBag.CourseName = courseDTO.Name;
                return View(updateVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading course for editing: {id}");
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Course/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Course.Update")]
        public async Task<IActionResult> Edit(int id, UpdateCourseVM model)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.CourseId = id;
                model.AcademicYears = new SelectList(await GetAcademicYearsForDropdown(), "Value", "Text", model.AcademicYearId);
                return View(model);
            }

            try
            {
                var updateDTO = _mapper.Map<UpdateCourseDTO>(model);
                await _courseService.UpdateAsync(id, updateDTO);
                TempData["SuccessMessage"] = "Course updated successfully!";
                return RedirectToAction(nameof(Details), new { id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating course: {id}");
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                ViewBag.CourseId = id;
                model.AcademicYears = new SelectList(await GetAcademicYearsForDropdown(), "Value", "Text", model.AcademicYearId);
                return View(model);
            }
        }

        // POST: Course/Delete/5
        [HttpPost]
        [Authorize(Policy = "Course.Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return Json(new { success = false, message = "Invalid ID" });
            }

            try
            {
                await _courseService.SoftDeleteAsync(id);
                _logger.LogInformation($"Course {id} deleted");
                return Json(new { success = true, message = "Course deleted successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting course {id}");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // POST: Course/Activate/5
        [HttpPost]
        [Authorize(Policy = "Course.Update")]
        public async Task<IActionResult> Activate(int id)
        {
            if (id <= 0)
            {
                return Json(new { success = false, message = "Invalid ID" });
            }

            try
            {
                await _courseService.ActivateAsync(id);
                _logger.LogInformation($"Course {id} activated");
                return Json(new { success = true, message = "Course activated successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error activating course {id}");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // POST: Course/Deactivate/5
        [HttpPost]
        [Authorize(Policy = "Course.Update")]
        public async Task<IActionResult> Deactivate(int id)
        {
            if (id <= 0)
            {
                return Json(new { success = false, message = "Invalid ID" });
            }

            try
            {
                await _courseService.DeactivateAsync(id);
                _logger.LogInformation($"Course {id} deactivated");
                return Json(new { success = true, message = "Course deactivated successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deactivating course {id}");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // Helper method to get academic years for dropdown
        private async Task<List<SelectListItem>> GetAcademicYearsForDropdown()
        {
            try
            {
                var academicYears = await _academicYearService.GetAllAcademicYearsAsync(new PaginationParamter { PageNumber = 1, PageSize = 100 });
                return academicYears
                    .Where(ay => ay.IsActive)
                    .Select(ay => new SelectListItem
                    {
                        Value = ay.Id.ToString(),
                        Text = ay.Name
                    })
                    .ToList();
            }
            catch
            {
                return new List<SelectListItem>();
            }
        }

        // GET: Course/GetAssignedTeachers/5 (AJAX)
        [HttpGet]
        public async Task<IActionResult> GetAssignedTeachers(int id)
        {
            try
            {
                var teacherCourses = await _teacherCourseService.GetByCourseIdAsync(id);
                var assignedTeachers = teacherCourses
                    .Where(tc => !tc.IsDeleted)
                    .Select(tc => new
                    {
                        teacherId = tc.TeacherId,
                        teacherName = tc.TeacherName,
                        isActive = tc.IsActive
                    })
                    .ToList();
                return Json(assignedTeachers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading assigned teachers for course {id}");
                return Json(new List<object>());
            }
        }
    }
}
