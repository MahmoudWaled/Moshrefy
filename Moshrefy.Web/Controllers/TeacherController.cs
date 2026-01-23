using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moshrefy.Application.DTOs.Teacher;
using Moshrefy.Application.DTOs.TeacherCourse;
using Moshrefy.Application.Interfaces.IServices;
using Moshrefy.Domain.Paramter;
using Moshrefy.Web.Models.Teacher;
using Moshrefy.Web.Extensions;
using Moshrefy.Application.DTOs.Common;

namespace Moshrefy.Web.Controllers
{
    [Authorize(Roles = "Admin,Manager,Employee")]
    public class TeacherController : Controller
    {
        #region Dependencies

        private readonly ITeacherService _teacherService;
        private readonly ITeacherCourseService _teacherCourseService;
        private readonly ICourseService _courseService;
        private readonly IMapper _mapper;
        private readonly ILogger<TeacherController> _logger;

        public TeacherController(
            ITeacherService teacherService,
            ITeacherCourseService teacherCourseService,
            ICourseService courseService,
            IMapper mapper,
            ILogger<TeacherController> logger)
        {
            _teacherService = teacherService;
            _teacherCourseService = teacherCourseService;
            _courseService = courseService;
            _mapper = mapper;
            _logger = logger;
        }

        #endregion

        #region Teacher Management

        // List all teachers
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // DataTables - Get teachers data
        [HttpPost]
        public async Task<IActionResult> GetTeachersData()
        {
            try
            {
                var request = Request.GetDataTableRequest();
                var response = await _teacherService.GetTeachersDataTableAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading teachers data");
                return StatusCode(500, new { error = "Error loading data. Please try again." });
            }
        }

        // View teacher details
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            try
            {
                var teacherDTO = await _teacherService.GetByIdAsync(id);
                var teacherVM = _mapper.Map<TeacherVM>(teacherDTO);
                return View(teacherVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading teacher details for ID {id}");
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // Create teacher - GET
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Create()
        {
            return View(new CreateTeacherVM());
        }

        // Create teacher - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create(CreateTeacherVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var createDTO = _mapper.Map<CreateTeacherDTO>(model);
                var createdTeacher = await _teacherService.CreateAsync(createDTO);
                TempData["SuccessMessage"] = "Teacher created successfully! Now assign courses.";
                return RedirectToAction(nameof(ManageCourses), new { id = createdTeacher.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating teacher");
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return View(model);
            }
        }

        // Edit teacher - GET
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            try
            {
                var teacherDTO = await _teacherService.GetByIdAsync(id);
                var updateVM = _mapper.Map<UpdateTeacherVM>(teacherDTO);

                ViewBag.TeacherId = id;
                ViewBag.TeacherName = teacherDTO!.Name;
                return View(updateVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading teacher for editing: {id}");
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // Edit teacher - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int id, UpdateTeacherVM model)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.TeacherId = id;
                return View(model);
            }

            try
            {
                var updateDTO = _mapper.Map<UpdateTeacherDTO>(model);
                await _teacherService.UpdateAsync(id, updateDTO);
                TempData["SuccessMessage"] = "Teacher updated successfully!";
                return RedirectToAction(nameof(Details), new { id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating teacher: {id}");
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                ViewBag.TeacherId = id;
                return View(model);
            }
        }

        // Activate teacher
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Activate(int id)
        {
            if (id <= 0)
            {
                return Json(new { success = false, message = "Invalid ID" });
            }

            try
            {
                await _teacherService.ActivateAsync(id);
                _logger.LogInformation($"Teacher {id} activated");
                return Json(new { success = true, message = "Teacher activated successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error activating teacher {id}");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // Deactivate teacher
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Deactivate(int id)
        {
            if (id <= 0)
            {
                return Json(new { success = false, message = "Invalid ID" });
            }

            try
            {
                await _teacherService.DeactivateAsync(id);
                _logger.LogInformation($"Teacher {id} deactivated");
                return Json(new { success = true, message = "Teacher deactivated successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deactivating teacher {id}");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // Soft delete teacher
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return Json(new { success = false, message = "Invalid ID" });
            }

            try
            {
                await _teacherService.SoftDeleteAsync(id);
                _logger.LogInformation($"Teacher {id} soft deleted");
                return Json(new { success = true, message = "Teacher deleted successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting teacher {id}");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // Restore teacher
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Restore(int id)
        {
            if (id <= 0)
            {
                return Json(new { success = false, message = "Invalid ID" });
            }

            try
            {
                await _teacherService.RestoreAsync(id);
                _logger.LogInformation($"Teacher {id} restored");
                return Json(new { success = true, message = "Teacher restored successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error restoring teacher {id}");
                return Json(new { success = false,message = $"Error: {ex.Message}" });
            }
        }

        // Hard delete teacher (permanent)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> HardDelete(int id)
        {
            if (id <= 0)
            {
                return Json(new { success = false, message = "Invalid ID" });
            }

            try
            {
                await _teacherService.DeleteAsync(id);
                _logger.LogInformation($"Teacher {id} permanently deleted");
                return Json(new { success = true, message = "Teacher permanently deleted!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error permanently deleting teacher {id}");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        #endregion

        #region Course Assignment

        // Manage courses page
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> ManageCourses(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            try
            {
                var teacherDTO = await _teacherService.GetByIdAsync(id);
                var teacherVM = _mapper.Map<TeacherVM>(teacherDTO);
                return View(teacherVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading manage courses for teacher {id}");
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Details), new { id = id });
            }
        }

        // Get assigned courses (AJAX)
        [HttpGet]
        public async Task<IActionResult> GetAssignedCourses(int id)
        {
            try
            {
                var teacherCourses = await _teacherCourseService.GetByTeacherIdAsync(id);
                var assignedCourses = teacherCourses
                    .Where(tc => !tc.IsDeleted && !tc.CourseIsDeleted)
                    .Select(tc => new
                    {
                        teacherCourseId = tc.Id,
                        courseId = tc.CourseId,
                        courseName = tc.CourseName,
                        academicYear = tc.AcademicYearName,
                        isActive = tc.IsActive
                    })
                    .ToList();
                return Json(assignedCourses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading assigned courses for teacher {id}");
                return Json(new List<object>());
            }
        }

        // Get available courses (AJAX)
        [HttpGet]
        public async Task<IActionResult> GetAvailableCourses(int id)
        {
            try
            {
                var allCourses = await _courseService.GetActiveAsync(new PaginationParameter { PageSize = 1000 });
                var assignedCourses = await _teacherCourseService.GetByTeacherIdAsync(id);
                var assignedCourseIds = assignedCourses
                    .Where(tc => !tc.IsDeleted)
                    .Select(tc => tc.CourseId)
                    .ToHashSet();

                var availableCourses = allCourses
                    .Where(c => !assignedCourseIds.Contains(c.Id))
                    .Select(c => new
                    {
                        id = c.Id,
                        name = c.Name,
                        academicYear = c.AcademicYearName ?? "N/A"
                    })
                    .ToList();

                return Json(availableCourses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading available courses for teacher {id}");
                return Json(new List<object>());
            }
        }

        // Assign course to teacher
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> AssignCourse(int teacherId, int courseId)
        {
            if (teacherId <= 0 || courseId <= 0)
            {
                return Json(new { success = false, message = "Invalid teacher or course ID" });
            }

            try
            {
                var dto = new CreateTeacherCourseDTO
                {
                    TeacherId = teacherId,
                    CourseId = courseId
                };
                await _teacherCourseService.CreateAsync(dto);
                _logger.LogInformation($"Course {courseId} assigned to teacher {teacherId}");
                return Json(new { success = true, message = "Course assigned successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error assigning course {courseId} to teacher {teacherId}");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // Unassign course from teacher
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UnassignCourse(int teacherCourseId)
        {
            if (teacherCourseId <= 0)
            {
                return Json(new { success = false, message = "Invalid assignment ID" });
            }

            try
            {
                await _teacherCourseService.SoftDeleteAsync(teacherCourseId);
                _logger.LogInformation($"TeacherCourse {teacherCourseId} unassigned");
                return Json(new { success = true, message = "Course unassigned successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error unassigning course {teacherCourseId}");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // Assign multiple courses to teacher
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> AssignMultipleCourses(int teacherId, int[] courseIds)
        {
            if (teacherId <= 0 || courseIds == null || courseIds.Length == 0)
            {
                return Json(new { success = false, message = "Invalid teacher or course IDs" });
            }

            try
            {
                int successCount = 0;
                foreach (var courseId in courseIds)
                {
                    try
                    {
                        var dto = new CreateTeacherCourseDTO
                        {
                            TeacherId = teacherId,
                            CourseId = courseId
                        };
                        await _teacherCourseService.CreateAsync(dto);
                        successCount++;
                    }
                    catch
                    {
                        // Skip already assigned courses
                    }
                }
                _logger.LogInformation($"{successCount} courses assigned to teacher {teacherId}");
                return Json(new { success = true, message = $"{successCount} course(s) assigned successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error bulk assigning courses to teacher {teacherId}");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        #endregion
    }
}
