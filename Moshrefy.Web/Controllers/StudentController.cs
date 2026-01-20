using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moshrefy.Application.DTOs.Student;
using Moshrefy.Application.Interfaces.IServices;
using Moshrefy.Domain.Enums;
using Moshrefy.Domain.Paramter;
using Moshrefy.Web.Models.Student;
using Moshrefy.Web.Extensions;
using Moshrefy.Application.DTOs.Common;

namespace Moshrefy.Web.Controllers
{
    [Authorize(Roles = "Admin,Manager,Employee")]
    public class StudentController : Controller
    {
        #region Dependencies

        private readonly IStudentService _studentService;
        private readonly IEnrollmentService _enrollmentService;
        private readonly IMapper _mapper;
        private readonly ILogger<StudentController> _logger;

        public StudentController(
            IStudentService studentService,
            IEnrollmentService enrollmentService,
            IMapper mapper,
            ILogger<StudentController> logger)
        {
            _studentService = studentService;
            _enrollmentService = enrollmentService;
            _mapper = mapper;
            _logger = logger;
        }

        #endregion

        #region Student Management

        // List all students
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // DataTables - Get students data
        [HttpPost]
        public async Task<IActionResult> GetStudentsData()
        {
            try
            {
                var request = Request.GetDataTableRequest();
                var response = await _studentService.GetStudentsDataTableAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading students data");
                return StatusCode(500, new { error = "Error loading data. Please try again." });
            }
        }

        // View student details
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            try
            {
                var studentDTO = await _studentService.GetByIdAsync(id);
                if (studentDTO == null || studentDTO.IsDeleted)
                {
                    return NotFound();
                }
                
                var studentVM = _mapper.Map<StudentVM>(studentDTO);
                return View(studentVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading student details for ID {id}");
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // Create student - GET
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Create()
        {
            return View(new CreateStudentVM());
        }

        // Create student - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create(CreateStudentVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var createDTO = _mapper.Map<CreateStudentDTO>(model);
                await _studentService.CreateAsync(createDTO);
                TempData["SuccessMessage"] = "Student created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating student");
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return View(model);
            }
        }

        // Edit student - GET
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
                var studentDTO = await _studentService.GetByIdAsync(id);
                var updateVM = _mapper.Map<UpdateStudentVM>(studentDTO);

                ViewBag.StudentId = id;
                ViewBag.StudentName = studentDTO.Name;
                return View(updateVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading student for editing: {id}");
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // Edit student - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int id, UpdateStudentVM model)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.StudentId = id;
                return View(model);
            }

            try
            {
                var updateDTO = _mapper.Map<UpdateStudentDTO>(model);
                await _studentService.UpdateAsync(id, updateDTO);
                TempData["SuccessMessage"] = "Student updated successfully!";
                return RedirectToAction(nameof(Details), new { id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating student: {id}");
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                ViewBag.StudentId = id;
                return View(model);
            }
        }

        // Activate student
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
                await _studentService.ActivateAsync(id);
                _logger.LogInformation($"Student {id} activated");
                return Json(new { success = true, message = "Student activated successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error activating student {id}");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // Deactivate student
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
                await _studentService.DeactivateAsync(id);
                _logger.LogInformation($"Student {id} deactivated");
                return Json(new { success = true, message = "Student deactivated successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deactivating student {id}");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // Suspend student
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Suspend(int id)
        {
            if (id <= 0)
            {
                return Json(new { success = false, message = "Invalid ID" });
            }

            try
            {
                await _studentService.SuspendAsync(id);
                _logger.LogInformation($"Student {id} suspended");
                return Json(new { success = true, message = "Student suspended successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error suspending student {id}");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // Soft delete student
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
                await _studentService.SoftDeleteAsync(id);
                _logger.LogInformation($"Student {id} soft deleted");
                return Json(new { success = true, message = "Student deleted successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting student {id}");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // Restore student
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
                await _studentService.RestoreAsync(id);
                _logger.LogInformation($"Student {id} restored");
                return Json(new { success = true, message = "Student restored successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error restoring student {id}");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // Hard delete student (permanent)
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
                await _studentService.DeleteAsync(id);
                _logger.LogInformation($"Student {id} permanently deleted");
                return Json(new { success = true, message = "Student permanently deleted!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error permanently deleting student {id}");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // GET: Student/GetEnrollments/5 (AJAX)
        // Get enrollments for a student
        [HttpGet]
        public async Task<IActionResult> GetEnrollments(int id)
        {
            try
            {
                var enrollments = await _enrollmentService.GetByStudentIdAsync(id);
                var result = enrollments
                    .Where(e => !e.IsDeleted && !e.CourseIsDeleted)
                    .Select(e => new
                    {
                        enrollmentId = e.Id,
                        courseId = e.CourseId,
                        courseName = e.CourseName,
                        academicYearName = e.AcademicYearName,
                        isActive = e.IsActive
                    })
                    .ToList();
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading enrollments for student {id}");
                return Json(new List<object>());
            }
        }

        #endregion
    }
}
