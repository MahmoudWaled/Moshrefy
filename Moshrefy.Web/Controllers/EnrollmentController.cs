using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moshrefy.Application.DTOs.Enrollment;
using Moshrefy.Application.Interfaces.IServices;
using Moshrefy.Domain.Paramter;
using Moshrefy.Web.Models.Enrollment;

namespace Moshrefy.Web.Controllers
{
    [Authorize(Policy = "Enrollment.View")]
    public class EnrollmentController : Controller
    {
        #region Dependencies

        private readonly IEnrollmentService _enrollmentService;
        private readonly IStudentService _studentService;
        private readonly ICourseService _courseService;
        private readonly IAcademicYearService _academicYearService;
        private readonly IMapper _mapper;
        private readonly ILogger<EnrollmentController> _logger;

        #endregion

        #region Constructor

        public EnrollmentController(
            IEnrollmentService enrollmentService,
            IStudentService studentService,
            ICourseService courseService,
            IAcademicYearService academicYearService,
            IMapper _mapper,
            ILogger<EnrollmentController> logger)
        {
            _enrollmentService = enrollmentService;
            _studentService = studentService;
            _courseService = courseService;
            _academicYearService = academicYearService;
            this._mapper = _mapper;
            _logger = logger;
        }

        #endregion

        #region Pages (GET Actions)

        // GET: Enrollment/Index
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // GET: Enrollment/Create
        [HttpGet]
        [Authorize(Policy = "Enrollment.Add")]
        public async Task<IActionResult> Create()
        {
            await PopulateDropdownsAsync();
            return View(new CreateEnrollmentVM());
        }

        #endregion

        #region DataTable

        // POST: Enrollment/GetEnrollmentsData (AJAX for DataTables)
        [HttpPost]
        public async Task<IActionResult> GetEnrollmentsData()
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

                List<EnrollmentResponseDTO> enrollmentsDTO;

                try
                {
                    // Get all enrollments - filtering is done on client side
                    var paginationParams = new PaginationParamter { PageSize = 1000 };
                    enrollmentsDTO = await _enrollmentService.GetAllAsync(paginationParams);
                }
                catch (Exception)
                {
                    enrollmentsDTO = new List<EnrollmentResponseDTO>();
                }

                // Filter enrollments to exclude deleted ones
                var filteredEnrollmentsDTO = enrollmentsDTO
                    .Where(e => !e.IsDeleted && !e.CourseIsDeleted && !e.StudentIsDeleted)
                    .ToList();

                var enrollmentsVM = _mapper.Map<List<EnrollmentVM>>(filteredEnrollmentsDTO);

                // Apply status filter
                if (!string.IsNullOrEmpty(filterStatus))
                {
                    if (filterStatus == "active")
                    {
                        enrollmentsVM = enrollmentsVM.Where(e => e.IsActive).ToList();
                    }
                    else if (filterStatus == "inactive")
                    {
                        enrollmentsVM = enrollmentsVM.Where(e => !e.IsActive).ToList();
                    }
                }

                // Apply search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    enrollmentsVM = enrollmentsVM.Where(e =>
                        (e.StudentName != null && e.StudentName.Contains(searchValue, StringComparison.OrdinalIgnoreCase)) ||
                        (e.CourseName != null && e.CourseName.Contains(searchValue, StringComparison.OrdinalIgnoreCase)) ||
                        (e.AcademicYearName != null && e.AcademicYearName.Contains(searchValue, StringComparison.OrdinalIgnoreCase))
                    ).ToList();
                }

                var jsonData = new
                {
                    draw = draw,
                    recordsTotal = enrollmentsVM.Count,
                    recordsFiltered = enrollmentsVM.Count,
                    data = enrollmentsVM
                };

                return Ok(jsonData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading enrollments data");
                return StatusCode(500, new { error = "Error loading data. Please try again." });
            }
        }

        #endregion

        #region CRUD Actions (POST)

        // POST: Enrollment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Enrollment.Add")]
        public async Task<IActionResult> Create(CreateEnrollmentVM model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync();
                return View(model);
            }

            try
            {
                var createDTO = _mapper.Map<CreateEnrollmentDTO>(model);
                await _enrollmentService.CreateAsync(createDTO);
                TempData["SuccessMessage"] = "Enrollment created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating enrollment");
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                await PopulateDropdownsAsync();
                return View(model);
            }
        }

        // POST: Enrollment/Delete/5
        [HttpPost]
        [Authorize(Policy = "Enrollment.Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return Json(new { success = false, message = "Invalid ID" });
            }

            try
            {
                await _enrollmentService.SoftDeleteAsync(id);
                _logger.LogInformation($"Enrollment {id} deleted");
                return Json(new { success = true, message = "Enrollment deleted successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting enrollment {id}");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        #endregion

        #region Status Management

        // POST: Enrollment/Activate/5
        [HttpPost]
        [Authorize(Policy = "Enrollment.Update")]
        public async Task<IActionResult> Activate(int id)
        {
            if (id <= 0)
            {
                return Json(new { success = false, message = "Invalid ID" });
            }

            try
            {
                await _enrollmentService.ActivateAsync(id);
                _logger.LogInformation($"Enrollment {id} activated");
                return Json(new { success = true, message = "Enrollment activated successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error activating enrollment {id}");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // POST: Enrollment/Deactivate/5
        [HttpPost]
        [Authorize(Policy = "Enrollment.Update")]
        public async Task<IActionResult> Deactivate(int id)
        {
            if (id <= 0)
            {
                return Json(new { success = false, message = "Invalid ID" });
            }

            try
            {
                await _enrollmentService.DeactivateAsync(id);
                _logger.LogInformation($"Enrollment {id} deactivated");
                return Json(new { success = true, message = "Enrollment deactivated successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deactivating enrollment {id}");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        #endregion

        #region Bulk Enrollment

        // POST: Enrollment/BulkEnrollStudentInCourses
        // Enroll ONE student in MULTIPLE courses
        [HttpPost]
        [Authorize(Policy = "Enrollment.Add")]
        public async Task<IActionResult> BulkEnrollStudentInCourses([FromBody] BulkEnrollmentVM model)
        {
            if (!model.StudentId.HasValue || model.CourseIds == null || !model.CourseIds.Any())
            {
                return Json(new { success = false, message = "Invalid data provided" });
            }

            try
            {
                var (successCount, duplicateCount) = await _enrollmentService.BulkEnrollStudentInCoursesAsync(
                    model.StudentId.Value, 
                    model.CourseIds);

                string message = $"Successfully enrolled in {successCount} course(s).";
                if (duplicateCount > 0)
                {
                    message += $" {duplicateCount} duplicate(s) skipped.";
                }

                _logger.LogInformation($"Student {model.StudentId} enrolled in {successCount} courses");
                return Json(new { success = true, message = message, enrolledCount = successCount });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error bulk enrolling student {model.StudentId}");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // POST: Enrollment/BulkEnrollStudentsInCourse
        // Enroll MULTIPLE students in ONE course
        [HttpPost]
        [Authorize(Policy = "Enrollment.Add")]
        public async Task<IActionResult> BulkEnrollStudentsInCourse([FromBody] BulkEnrollmentVM model)
        {
            if (!model.CourseId.HasValue || model.StudentIds == null || !model.StudentIds.Any())
            {
                return Json(new { success = false, message = "Invalid data provided" });
            }

            try
            {
                var (successCount, duplicateCount) = await _enrollmentService.BulkEnrollStudentsInCourseAsync(
                    model.CourseId.Value, 
                    model.StudentIds);

                string message = $"Successfully enrolled {successCount} student(s).";
                if (duplicateCount > 0)
                {
                    message += $" {duplicateCount} duplicate(s) skipped.";
                }

                _logger.LogInformation($"{successCount} students enrolled in course {model.CourseId}");
                return Json(new { success = true, message = message, enrolledCount = successCount });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error bulk enrolling students in course {model.CourseId}");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        #endregion

        #region Helper Methods / AJAX Endpoints

        // GET: Enrollment/GetAvailableCoursesForStudent/5
        // Get courses that student is NOT already enrolled in, grouped by academic year
        [HttpGet]
        public async Task<IActionResult> GetAvailableCoursesForStudent(int studentId)
        {
            try
            {
                var allCourses = await _courseService.GetAllAsync(new PaginationParamter { PageSize = 1000 });
                var enrolledCourses = await _enrollmentService.GetByStudentIdAsync(studentId);
                
                // Only count non-deleted enrollments with non-deleted courses
                // This is the list of courses the student IS currently enrolled in
                var enrolledCourseIds = enrolledCourses
                    .Where(e => !e.IsDeleted && !e.CourseIsDeleted)
                    .Select(e => e.CourseId)
                    .ToHashSet();

                // Return courses student is NOT enrolled in and are active
                var availableCourses = allCourses
                    .Where(c => !enrolledCourseIds.Contains(c.Id) && c.IsActive && !c.IsDeleted)
                    .GroupBy(c => new { AcademicYearId = c.AcademicYearId, AcademicYearName = c.AcademicYearName })
                    .Select(g => new
                    {
                        academicYearId = g.Key.AcademicYearId,
                        academicYearName = g.Key.AcademicYearName ?? "Unknown",
                        courses = g.Select(c => new { id = c.Id, name = c.Name }).ToList()
                    })
                    .ToList();

                return Json(availableCourses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting available courses for student {studentId}");
                return Json(new List<object>());
            }
        }

        // GET: Enrollment/GetAvailableStudentsForCourse/5
        // Get students that are NOT already enrolled in course
        [HttpGet]
        public async Task<IActionResult> GetAvailableStudentsForCourse(int courseId)
        {
            try
            {
                var allStudents = await _studentService.GetAllAsync(new PaginationParamter { PageSize = 1000 });
                var enrolledStudents = await _enrollmentService.GetByCourseIdAsync(courseId);
                
                // Only count non-deleted enrollments with non-deleted students
                // This is the list of students who ARE currently enrolled (active enrollments only)
                var enrolledStudentIds = enrolledStudents
                    .Where(e => !e.IsDeleted && !e.StudentIsDeleted)
                    .Select(e => e.StudentId)
                    .ToHashSet();

                // Return students who are NOT in the enrolled list and are not deleted
                var availableStudents = allStudents
                    .Where(s => !enrolledStudentIds.Contains(s.Id) && !s.IsDeleted)
                    .Select(s => new { id = s.Id, name = s.Name })
                    .ToList();

                return Json(availableStudents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting available students for course {courseId}");
                return Json(new List<object>());
            }
        }

        // Private helper to populate dropdowns
        private async Task PopulateDropdownsAsync()
        {
            try
            {
                var students = await _studentService.GetAllAsync(new PaginationParamter { PageSize = 1000 });
                var courses = await _courseService.GetAllAsync(new PaginationParamter { PageSize = 1000 });

                ViewBag.Students = students
                    .Where(s => !s.IsDeleted)
                    .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name })
                    .ToList();

                ViewBag.Courses = courses
                    .Where(c => c.IsActive && !c.IsDeleted)
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = $"{c.Name} ({c.AcademicYearName})" })
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error populating dropdowns");
                ViewBag.Students = new List<SelectListItem>();
                ViewBag.Courses = new List<SelectListItem>();
            }
        }

        #endregion
    }
}
