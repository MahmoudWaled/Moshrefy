using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moshrefy.Application.DTOs.Course;
using Moshrefy.Application.Interfaces.IUnitOfWork;
using Moshrefy.Application.Interfaces.IServices;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Exceptions;
using Moshrefy.Domain.Identity;
using Moshrefy.Domain.Paramter;

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

            var course = await _unitOfWork.Courses.GetByIdAsync(id);
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

            var allCourses = await _unitOfWork.Courses.GetAllAsync(paginationParamter);

            var filteredCourses = allCourses
                .Where(c => c.CenterId == currentCenterId && !c.IsDeleted)
                .ToList();

            if (filteredCourses.Count == 0)
                throw new NoDataFoundException("No courses found.");

            return _mapper.Map<List<CourseResponseDTO>>(filteredCourses);
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

            _unitOfWork.Courses.UpdateAsync(course);
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

            _unitOfWork.Courses.DeleteAsync(course);
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
            _unitOfWork.Courses.UpdateAsync(course);
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
            _unitOfWork.Courses.UpdateAsync(course);
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
            _unitOfWork.Courses.UpdateAsync(course);
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
            _unitOfWork.Courses.UpdateAsync(course);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
