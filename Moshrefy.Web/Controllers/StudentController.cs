using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moshrefy.Application.DTOs.Student;
using Moshrefy.Application.Interfaces.IServices;
using Moshrefy.Domain.Enums;
using Moshrefy.Domain.Paramter;
using Moshrefy.Web.Models.Student;

namespace Moshrefy.Web.Controllers
{
    [Authorize(Roles = "Admin,Manager,Employee")]
    public class StudentController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly IMapper _mapper;
        private readonly ILogger<StudentController> _logger;

        public StudentController(
            IStudentService studentService,
            IMapper mapper,
            ILogger<StudentController> logger)
        {
            _studentService = studentService;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: Student/Index
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // POST: Student/GetStudentsData (AJAX for DataTables)
        [HttpPost]
        public async Task<IActionResult> GetStudentsData()
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
                    PageNumber = 1,
                    PageSize = 1000 // Get all for client-side filtering
                };

                List<StudentResponseDTO> studentsDTO;

                try
                {
                    studentsDTO = await _studentService.GetAllAsync(paginationParams);
                }
                catch (Exception)
                {
                    studentsDTO = new List<StudentResponseDTO>();
                }

                var studentsVM = _mapper.Map<List<StudentVM>>(studentsDTO);

                // Always filter out deleted students (only SuperAdmin can manage deleted)
                studentsVM = studentsVM.Where(s => !s.IsDeleted).ToList();

                // Apply status filter
                if (!string.IsNullOrEmpty(filterStatus) && filterStatus != "all")
                {
                    if (Enum.TryParse<StudentStatus>(filterStatus, true, out var status))
                    {
                        studentsVM = studentsVM.Where(s => s.StudentStatus == status).ToList();
                    }
                }

                // Apply search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    studentsVM = studentsVM.Where(s =>
                        s.Name.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ||
                        s.FirstPhone.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ||
                        (s.NationalId != null && s.NationalId.Contains(searchValue, StringComparison.OrdinalIgnoreCase))
                    ).ToList();
                }

                // Apply sorting
                var sortColumn = Request.Form["order[0][column]"].FirstOrDefault();
                var sortDirection = Request.Form["order[0][dir]"].FirstOrDefault();

                if (!string.IsNullOrEmpty(sortColumn))
                {
                    int colIndex = int.Parse(sortColumn);
                    bool isAsc = sortDirection == "asc";

                    studentsVM = colIndex switch
                    {
                        1 => isAsc ? studentsVM.OrderBy(s => s.Name).ToList() : studentsVM.OrderByDescending(s => s.Name).ToList(),
                        2 => isAsc ? studentsVM.OrderBy(s => s.FirstPhone).ToList() : studentsVM.OrderByDescending(s => s.FirstPhone).ToList(),
                        3 => isAsc ? studentsVM.OrderBy(s => s.Age).ToList() : studentsVM.OrderByDescending(s => s.Age).ToList(),
                        4 => isAsc ? studentsVM.OrderBy(s => s.StudentStatus).ToList() : studentsVM.OrderByDescending(s => s.StudentStatus).ToList(),
                        _ => studentsVM.OrderBy(s => s.Name).ToList()
                    };
                }

                var totalRecords = studentsVM.Count;

                // Apply pagination
                var pagedData = studentsVM.Skip(skip).Take(pageSize).ToList();

                var jsonData = new
                {
                    draw = draw,
                    recordsTotal = totalRecords,
                    recordsFiltered = totalRecords,
                    data = pagedData
                };

                return Ok(jsonData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading students data");
                return StatusCode(500, new { error = "Error loading data. Please try again." });
            }
        }

        // GET: Student/Details/5
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

        // GET: Student/Create
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Create()
        {
            return View(new CreateStudentVM());
        }

        // POST: Student/Create
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

        // GET: Student/Edit/5
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

        // POST: Student/Edit/5
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

        // POST: Student/Activate/5
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

        // POST: Student/Deactivate/5
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

        // POST: Student/Suspend/5
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

        // POST: Student/Delete/5
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

        // POST: Student/Restore/5
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

        // POST: Student/HardDelete/5
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
    }
}
