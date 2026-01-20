using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moshrefy.Application.DTOs.Course;
using Moshrefy.Application.Interfaces.IServices;
using Moshrefy.Domain.Paramter;
using Moshrefy.Web.Models.Course;
using Moshrefy.Web.Extensions;
using Moshrefy.Application.DTOs.Common;

namespace Moshrefy.Web.Controllers
{
    [Authorize(Policy = "Course.View")]
    public class CourseController : Controller
    {
        #region Dependencies

        private readonly ICourseService _courseService;
        private readonly IAcademicYearService _academicYearService;
        private readonly ITeacherCourseService _teacherCourseService;
        private readonly IEnrollmentService _enrollmentService;
        private readonly IMapper _mapper;
        private readonly ILogger<CourseController> _logger;

        public CourseController(
            ICourseService courseService,
            IAcademicYearService academicYearService,
            ITeacherCourseService teacherCourseService,
            IEnrollmentService enrollmentService,
            IMapper _mapper,
            ILogger<CourseController> logger)
        {
            _courseService = courseService;
            _academicYearService = academicYearService;
            _teacherCourseService = teacherCourseService;
            _enrollmentService = enrollmentService;
            this._mapper = _mapper;
            _logger = logger;
        }

        #endregion

        #region Course Management

        // List all courses
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // DataTables - Get courses data
        [HttpPost]
        public async Task<IActionResult> GetCoursesData()
        {
            try
            {
                var request = Request.GetDataTableRequest();
                var response = await _courseService.GetCoursesDataTableAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading courses data");
                return StatusCode(500, new { error = "Error loading data. Please try again." });
            }
        }

        // View course details
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
                if (courseDTO == null || courseDTO.IsDeleted)
                {
                    return NotFound();
                }
                
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

        // Create course - GET
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

        // Create course - POST
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

        // Edit course - GET
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

        // Edit course - POST
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

        // Delete course
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

        // Activate course
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

        // Deactivate course
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

        // Get assigned teachers (AJAX)
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

        #endregion

        #region Helper Methods

        // Get academic years for dropdown
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

        // GET: Course/GetEnrolledStudents/5 (AJAX)
        // Get enrolled students for a course
        [HttpGet]
        public async Task<IActionResult> GetEnrolledStudents(int id)
        {
            try
            {
                var enrollments = await _enrollmentService.GetByCourseIdAsync(id);
                var result = enrollments
                    .Where(e => !e.IsDeleted && !e.StudentIsDeleted)
                    .Select(e => new
                    {
                        studentId = e.StudentId,
                        studentName = e.StudentName,
                        isActive = e.IsActive
                    })
                    .ToList();
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading enrolled students for course {id}");
                return Json(new List<object>());
            }
        }

        #endregion
    }
}
