using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moshrefy.Application.DTOs.Teacher;
using Moshrefy.Application.DTOs.TeacherCourse;
using Moshrefy.Application.Interfaces.IServices;
using Moshrefy.Domain.Paramter;
using Moshrefy.Web.Models.Teacher;

namespace Moshrefy.Web.Controllers
{
    [Authorize(Roles = "Admin,Manager,Employee")]
    public class TeacherController : Controller
    {
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

        // GET: Teacher/Index
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // POST: Teacher/GetTeachersData (AJAX for DataTables)
        [HttpPost]
        public async Task<IActionResult> GetTeachersData()
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

                // Get total count from database efficiently
                int totalRecords = 0;
                try
                {
                    totalRecords = await _teacherService.GetTotalCountAsync();
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

                List<TeacherResponseDTO> teachersDTO;

                try
                {
                    teachersDTO = await _teacherService.GetAllAsync(paginationParams);
                }
                catch (Exception)
                {
                    teachersDTO = new List<TeacherResponseDTO>();
                }

                var teachersVM = _mapper.Map<List<TeacherVM>>(teachersDTO);

                // Populate course counts for each teacher
                foreach (var teacher in teachersVM)
                {
                    var teacherCourses = await _teacherCourseService.GetByTeacherIdAsync(teacher.Id);
                    teacher.CourseCount = teacherCourses.Count(tc => !tc.IsDeleted && !tc.CourseIsDeleted);
                }

                // Apply UI filters on the current page
                if (!string.IsNullOrEmpty(filterStatus) && filterStatus != "all")
                {
                    var isActive = filterStatus == "Active";
                    teachersVM = teachersVM.Where(t => t.IsActive == isActive).ToList();
                }

                if (!string.IsNullOrEmpty(searchValue))
                {
                    teachersVM = teachersVM.Where(t =>
                        t.Name.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ||
                        t.Phone.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ||
                        t.Specialization.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ||
                        (t.Email != null && t.Email.Contains(searchValue, StringComparison.OrdinalIgnoreCase))
                    ).ToList();
                }

                // Apply sorting
                var sortColumn = Request.Form["order[0][column]"].FirstOrDefault();
                var sortDirection = Request.Form["order[0][dir]"].FirstOrDefault();

                if (!string.IsNullOrEmpty(sortColumn))
                {
                    int colIndex = int.Parse(sortColumn);
                    bool isAsc = sortDirection == "asc";

                    teachersVM = colIndex switch
                    {
                        1 => isAsc ? teachersVM.OrderBy(t => t.Name).ToList() : teachersVM.OrderByDescending(t => t.Name).ToList(),
                        2 => isAsc ? teachersVM.OrderBy(t => t.Phone).ToList() : teachersVM.OrderByDescending(t => t.Phone).ToList(),
                        3 => isAsc ? teachersVM.OrderBy(t => t.Specialization).ToList() : teachersVM.OrderByDescending(t => t.Specialization).ToList(),
                        4 => isAsc ? teachersVM.OrderBy(t => t.IsActive).ToList() : teachersVM.OrderByDescending(t => t.IsActive).ToList(),
                        _ => teachersVM.OrderBy(t => t.Name).ToList()
                    };
                }

                var jsonData = new
                {
                    draw = draw,
                    recordsTotal = totalRecords,
                    recordsFiltered = totalRecords,
                    data = teachersVM
                };

                return Ok(jsonData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading teachers data");
                return StatusCode(500, new { error = "Error loading data. Please try again." });
            }
        }


        // GET: Teacher/Details/5
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

        // GET: Teacher/Create
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Create()
        {
            return View(new CreateTeacherVM());
        }

        // POST: Teacher/Create
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

        // GET: Teacher/Edit/5
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

        // POST: Teacher/Edit/5
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

        // POST: Teacher/Activate/5
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

        // POST: Teacher/Deactivate/5
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

        // POST: Teacher/Delete/5
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

        // POST: Teacher/Restore/5
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
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // POST: Teacher/HardDelete/5
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

        // ============= COURSE ASSIGNMENT ACTIONS =============

        // GET: Teacher/ManageCourses/5
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

        // GET: Teacher/GetAssignedCourses/5 (AJAX)
        [HttpGet]
        public async Task<IActionResult> GetAssignedCourses(int id)
        {
            try
            {
                var teacherCourses = await _teacherCourseService.GetByTeacherIdAsync(id);
                var assignedCourses = teacherCourses
                    .Where(tc => !tc.IsDeleted && !tc.CourseIsDeleted) // Filter out deleted assignments AND deleted courses
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

        // GET: Teacher/GetAvailableCourses/5 (AJAX)
        [HttpGet]
        public async Task<IActionResult> GetAvailableCourses(int id)
        {
            try
            {
                // Get all active courses
                var allCourses = await _courseService.GetActiveAsync(new PaginationParamter { PageSize = 1000 });
                
                // Get already assigned courses
                var assignedCourses = await _teacherCourseService.GetByTeacherIdAsync(id);
                var assignedCourseIds = assignedCourses
                    .Where(tc => !tc.IsDeleted)
                    .Select(tc => tc.CourseId)
                    .ToHashSet();

                // Filter out already assigned
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

        // POST: Teacher/AssignCourse
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

        // POST: Teacher/UnassignCourse
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

        // POST: Teacher/AssignMultipleCourses
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
    }
}
