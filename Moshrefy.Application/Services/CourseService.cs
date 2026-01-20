using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moshrefy.Application.DTOs.Course;
using Moshrefy.Application.Interfaces.IUnitOfWork;
using Moshrefy.Application.Interfaces.IServices;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Exceptions;
using Moshrefy.Domain.Identity;
using Moshrefy.Domain.Paramter;
using Microsoft.EntityFrameworkCore;

namespace Moshrefy.Application.Services
{
    public class CourseService(
        IUnitOfWork _unitOfWork,
        IMapper _mapper,
        ITenantContext tenantContext,
        UserManager<ApplicationUser> _userManager
        ) : BaseService(tenantContext), ICourseService
    {
        public async Task<CourseResponseDTO> CreateAsync(CreateCourseDTO createCourseDTO)
        {
            if (createCourseDTO == null)
                throw new BadRequestException("CreateCourseDTO cannot be null.");

            var currentCenterId = GetCurrentCenterIdOrThrow();
            var course = _mapper.Map<Course>(createCourseDTO);

            var currentUser = await _userManager.FindByIdAsync(tenantContext.GetCurrentUserId());
            course.CenterId = currentCenterId;
            course.CreatedById = currentUser!.Id;
            course.CreatedByName = currentUser!.UserName ?? string.Empty;

            await _unitOfWork.Courses.AddAsync(course);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<CourseResponseDTO>(course);
        }

        public async Task<CourseResponseDTO?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new BadRequestException("Invalid course id.");

            var course = await _unitOfWork.Courses.GetByIdWithAcademicYearAsync(id);
            if (course == null)
                throw new NotFoundException<int>(nameof(Course), "id", id);

            ValidateCenterAccess(course.CenterId, nameof(Course));

            return _mapper.Map<CourseResponseDTO>(course);
        }

        public async Task<List<CourseResponseDTO>> GetAllAsync(PaginationParamter paginationParamter)
        {
            if (paginationParamter == null)
                throw new BadRequestException("Pagination parameters cannot be null.");

            var currentCenterId = GetCurrentCenterIdOrThrow();

            // Filter at database level BEFORE pagination
            var courses = await _unitOfWork.Courses.GetAllAsync(
                c => c.CenterId == currentCenterId && !c.IsDeleted,
                paginationParamter);

            if (!courses.Any())
                throw new NoDataFoundException("No courses found.");

            return _mapper.Map<List<CourseResponseDTO>>(courses.ToList());
        }


        public async Task<List<CourseResponseDTO>> GetByNameAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new BadRequestException("Course name cannot be null or empty.");

            var currentCenterId = GetCurrentCenterIdOrThrow();

            var courses = await _unitOfWork.Courses.GetByName(name);

            var filteredCourses = courses
                .Where(c => c.CenterId == currentCenterId && !c.IsDeleted)
                .ToList();

            if (filteredCourses.Count == 0)
                throw new NoDataFoundException($"No courses found with name '{name}' in your center.");

            return _mapper.Map<List<CourseResponseDTO>>(filteredCourses);
        }

        public async Task<List<CourseResponseDTO>> GetActiveAsync(PaginationParamter paginationParamter)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();

            var courses = await _unitOfWork.Courses.GetAllAsync(paginationParamter);
            var activeCourses = courses
                .Where(c => c.IsActive && c.CenterId == currentCenterId && !c.IsDeleted)
                .ToList();

            return _mapper.Map<List<CourseResponseDTO>>(activeCourses);
        }

        public async Task<List<CourseResponseDTO>> GetInactiveAsync(PaginationParamter paginationParamter)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();

            var courses = await _unitOfWork.Courses.GetAllAsync(paginationParamter);
            var inactiveCourses = courses
                .Where(c => !c.IsActive && c.CenterId == currentCenterId && !c.IsDeleted)
                .ToList();

            return _mapper.Map<List<CourseResponseDTO>>(inactiveCourses);
        }

        public async Task<List<CourseResponseDTO>> GetByAcademicYearIdAsync(int academicYearId)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();

            var courses = await _unitOfWork.Courses.GetAllAsync(new PaginationParamter());
            var coursesByYear = courses
                .Where(c => c.AcademicYearId == academicYearId && c.CenterId == currentCenterId && !c.IsDeleted)
                .ToList();

            return _mapper.Map<List<CourseResponseDTO>>(coursesByYear);
        }

        public async Task UpdateAsync(int id, UpdateCourseDTO updateCourseDTO)
        {
            if (id <= 0)
                throw new BadRequestException("Invalid course id.");

            if (updateCourseDTO == null)
                throw new BadRequestException("UpdateCourseDTO cannot be null.");

            var course = await _unitOfWork.Courses.GetByIdAsync(id);
            if (course == null)
                throw new NotFoundException<int>(nameof(Course), "id", id);

            ValidateCenterAccess(course.CenterId, nameof(Course));

            _mapper.Map(updateCourseDTO, course);
            var currentUser = await _userManager.FindByIdAsync(tenantContext.GetCurrentUserId());
            course.ModifiedById = currentUser!.Id;
            course.ModifiedByName = currentUser!.UserName ?? string.Empty;
            course.ModifiedAt = DateTime.UtcNow;

            _unitOfWork.Courses.Update(course);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new BadRequestException("Invalid course id.");

            var course = await _unitOfWork.Courses.GetByIdAsync(id);
            if (course == null)
                throw new NotFoundException<int>(nameof(Course), "id", id);

            ValidateCenterAccess(course.CenterId, nameof(Course));

            _unitOfWork.Courses.SoftDelete(course);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ActivateAsync(int id)
        {
            if (id <= 0)
                throw new BadRequestException("Invalid course id.");

            var course = await _unitOfWork.Courses.GetByIdAsync(id);
            if (course == null)
                throw new NotFoundException<int>(nameof(Course), "id", id);

            ValidateCenterAccess(course.CenterId, nameof(Course));

            course.IsActive = true;
            _unitOfWork.Courses.Update(course);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeactivateAsync(int id)
        {
            if (id <= 0)
                throw new BadRequestException("Invalid course id.");

            var course = await _unitOfWork.Courses.GetByIdAsync(id);
            if (course == null)
                throw new NotFoundException<int>(nameof(Course), "id", id);

            ValidateCenterAccess(course.CenterId, nameof(Course));

            course.IsActive = false;
            _unitOfWork.Courses.Update(course);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int id)
        {
            if (id <= 0)
                throw new BadRequestException("Invalid course id.");

            var course = await _unitOfWork.Courses.GetByIdAsync(id);
            if (course == null)
                throw new NotFoundException<int>(nameof(Course), "id", id);

            ValidateCenterAccess(course.CenterId, nameof(Course));

            course.IsDeleted = true;
            course.IsActive = false;
            _unitOfWork.Courses.Update(course);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RestoreAsync(int id)
        {
            if (id <= 0)
                throw new BadRequestException("Invalid course id.");

            var course = await _unitOfWork.Courses.GetByIdAsync(id);
            if (course == null)
                throw new NotFoundException<int>(nameof(Course), "id", id);

            ValidateCenterAccess(course.CenterId, nameof(Course));

            course.IsDeleted = false;
            _unitOfWork.Courses.Update(course);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> GetTotalCountAsync()
        {
            var centerId = GetCurrentCenterIdOrThrow();
            return await _unitOfWork.Courses.CountAsync(c => c.CenterId == centerId && !c.IsDeleted);
        }

        public async Task<Moshrefy.Application.DTOs.Common.DataTableResponse<CourseResponseDTO>> GetCoursesDataTableAsync(Moshrefy.Application.DTOs.Common.DataTableRequest request)
        {
            var centerId = GetCurrentCenterIdOrThrow();

            // 1. Initial Query
            var query = _unitOfWork.Courses.GetQueryable()
                .Where(c => c.CenterId == centerId && !c.IsDeleted);

            // 2. Total Records (before filtering)
            var totalRecords = await _unitOfWork.Courses.CountAsync(c => c.CenterId == centerId && !c.IsDeleted);

            // 3. Apply Filters
            if (!string.IsNullOrEmpty(request.SearchValue))
            {
                var search = request.SearchValue.ToLower();
                query = query.Where(c =>
                    c.Name.ToLower().Contains(search) ||
                    (c.AcademicYear.Name != null && c.AcademicYear.Name.ToLower().Contains(search))
                );
            }

            // Status Filter
            if (!string.IsNullOrEmpty(request.FilterStatus) && request.FilterStatus != "all")
            {
                bool isActive = request.FilterStatus == "active";
                query = query.Where(c => c.IsActive == isActive);
            }

            // Academic Year Filter
            if (request.AcademicYearId.HasValue && request.AcademicYearId.Value > 0)
            {
                query = query.Where(c => c.AcademicYearId == request.AcademicYearId.Value);
            }

            // Count Filtered
            var filteredRecords = await query.CountAsync();

            // 4. Sorting
            if (!string.IsNullOrEmpty(request.SortColumnName) && !string.IsNullOrEmpty(request.SortDirection))
            {
                bool isAsc = request.SortDirection.ToLower() == "asc";
                query = request.SortColumnName.ToLower() switch
                {
                    "name" => isAsc ? query.OrderBy(c => c.Name) : query.OrderByDescending(c => c.Name),
                    "academicyearname" => isAsc ? query.OrderBy(c => c.AcademicYear.Name) : query.OrderByDescending(c => c.AcademicYear.Name),
                    "isactive" => isAsc ? query.OrderBy(c => c.IsActive) : query.OrderByDescending(c => c.IsActive),
                    _ => query.OrderByDescending(c => c.CreatedAt)
                };
            }
            else
            {
                query = query.OrderByDescending(c => c.CreatedAt);
            }

            // 5. Pagination
            if (request.Length > 0)
            {
                query = query.Skip(request.Start).Take(request.Length);
            }

            // 6. Execute & Map
            var courses = await query.Include(c => c.AcademicYear).ToListAsync();
            var data = _mapper.Map<List<CourseResponseDTO>>(courses);

            return new Moshrefy.Application.DTOs.Common.DataTableResponse<CourseResponseDTO>
            {
                Draw = request.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = filteredRecords,
                Data = data
            };
        }
    }
}

