using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moshrefy.Application.DTOs.TeacherCourse;
using Moshrefy.Application.Interfaces.IUnitOfWork;
using Moshrefy.Application.Interfaces.IServices;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Exceptions;
using Moshrefy.Domain.Identity;
using Moshrefy.Domain.Paramter;

namespace Moshrefy.Application.Services
{
    public class TeacherCourseService(
        IUnitOfWork unitOfWork, 
        IMapper mapper,
        ITenantContext tenantContext,
        UserManager<ApplicationUser> userManager) : BaseService(tenantContext), ITeacherCourseService
    {
        public async Task<TeacherCourseResponseDTO> CreateAsync(CreateTeacherCourseDTO createTeacherCourseDTO)
        {
            // Check if already exists (including soft-deleted)
            var existing = await unitOfWork.TeacherCourses.GetAllAsync(new PaginationParamter());
            var existingAssignment = existing.FirstOrDefault(tc => tc.TeacherId == createTeacherCourseDTO.TeacherId && tc.CourseId == createTeacherCourseDTO.CourseId);
            
            if (existingAssignment != null)
            {
                if (!existingAssignment.IsDeleted)
                {
                    throw new BadRequestException("Teacher is already assigned to this course.");
                }
                
                // Restore the soft-deleted assignment
                existingAssignment.IsDeleted = false;
                existingAssignment.IsActive = true;
                
                var currentUserForRestore = await userManager.FindByIdAsync(tenantContext.GetCurrentUserId());
                existingAssignment.ModifiedById = currentUserForRestore!.Id;
                existingAssignment.ModifiedByName = currentUserForRestore!.UserName ?? string.Empty;
                existingAssignment.ModifiedAt = DateTimeOffset.UtcNow;
                
                unitOfWork.TeacherCourses.UpdateAsync(existingAssignment);
                await unitOfWork.SaveChangesAsync();
                return mapper.Map<TeacherCourseResponseDTO>(existingAssignment);
            }

            var teacherCourse = mapper.Map<TeacherCourse>(createTeacherCourseDTO);
            
            // Set audit fields
            teacherCourse.CenterId = tenantContext.GetCurrentCenterId() ?? 0;
            var currentUser = await userManager.FindByIdAsync(tenantContext.GetCurrentUserId());
            teacherCourse.CreatedById = currentUser!.Id;
            teacherCourse.CreatedByName = currentUser!.UserName ?? string.Empty;
            
            await unitOfWork.TeacherCourses.AddAsync(teacherCourse);
            await unitOfWork.SaveChangesAsync();
            return mapper.Map<TeacherCourseResponseDTO>(teacherCourse);
        }

        public async Task<TeacherCourseResponseDTO?> GetByIdAsync(int id)
        {
            var teacherCourse = await unitOfWork.TeacherCourses.GetByIdAsync(id);
            if (teacherCourse == null)
                throw new NotFoundException<int>(nameof(teacherCourse), "teacherCourse", id);

            return mapper.Map<TeacherCourseResponseDTO>(teacherCourse);
        }

        public async Task<List<TeacherCourseResponseDTO>> GetAllAsync(PaginationParamter paginationParamter)
        {
            var teacherCourses = await unitOfWork.TeacherCourses.GetAllAsync(paginationParamter);
            return mapper.Map<List<TeacherCourseResponseDTO>>(teacherCourses);
        }

        public async Task<List<TeacherCourseResponseDTO>> GetByTeacherIdAsync(int teacherId)
        {
            var teacherCourses = await unitOfWork.TeacherCourses.GetByTeacherIdAsync(teacherId);
            return mapper.Map<List<TeacherCourseResponseDTO>>(teacherCourses);
        }

        public async Task<List<TeacherCourseResponseDTO>> GetByCourseIdAsync(int courseId)
        {
            var teacherCourses = await unitOfWork.TeacherCourses.GetByCourseIdAsync(courseId);
            return mapper.Map<List<TeacherCourseResponseDTO>>(teacherCourses);
        }

        public async Task<List<TeacherCourseResponseDTO>> GetActiveAsync(PaginationParamter paginationParamter)
        {
            var teacherCourses = await unitOfWork.TeacherCourses.GetAllAsync(paginationParamter);
            var active = teacherCourses.Where(tc => tc.IsActive).ToList();
            return mapper.Map<List<TeacherCourseResponseDTO>>(active);
        }

        public async Task<List<TeacherCourseResponseDTO>> GetInactiveAsync(PaginationParamter paginationParamter)
        {
            var teacherCourses = await unitOfWork.TeacherCourses.GetAllAsync(paginationParamter);
            var inactive = teacherCourses.Where(tc => !tc.IsActive).ToList();
            return mapper.Map<List<TeacherCourseResponseDTO>>(inactive);
        }

        public async Task UpdateAsync(int id, UpdateTeacherCourseDTO updateTeacherCourseDTO)
        {
            var teacherCourse = await unitOfWork.TeacherCourses.GetByIdAsync(id);
            if (teacherCourse == null)
                throw new NotFoundException<int>(nameof(teacherCourse), "teacherCourse", id);

            ValidateCenterAccess(teacherCourse.CenterId, nameof(TeacherCourse));

            mapper.Map(updateTeacherCourseDTO, teacherCourse);
            
            // Set modified audit fields
            var currentUser = await userManager.FindByIdAsync(tenantContext.GetCurrentUserId());
            teacherCourse.ModifiedById = currentUser!.Id;
            teacherCourse.ModifiedByName = currentUser!.UserName ?? string.Empty;
            teacherCourse.ModifiedAt = DateTimeOffset.UtcNow;
            
            unitOfWork.TeacherCourses.UpdateAsync(teacherCourse);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var teacherCourse = await unitOfWork.TeacherCourses.GetByIdAsync(id);
            if (teacherCourse == null)
                throw new NotFoundException<int>(nameof(teacherCourse), "teacherCourse", id);

            ValidateCenterAccess(teacherCourse.CenterId, nameof(TeacherCourse));

            unitOfWork.TeacherCourses.DeleteAsync(teacherCourse);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int id)
        {
            var teacherCourse = await unitOfWork.TeacherCourses.GetByIdAsync(id);
            if (teacherCourse == null)
                throw new NotFoundException<int>(nameof(teacherCourse), "teacherCourse", id);

            ValidateCenterAccess(teacherCourse.CenterId, nameof(TeacherCourse));

            teacherCourse.IsDeleted = true;
            
            var currentUser = await userManager.FindByIdAsync(tenantContext.GetCurrentUserId());
            teacherCourse.ModifiedById = currentUser!.Id;
            teacherCourse.ModifiedByName = currentUser!.UserName ?? string.Empty;
            teacherCourse.ModifiedAt = DateTimeOffset.UtcNow;
            
            unitOfWork.TeacherCourses.UpdateAsync(teacherCourse);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task RestoreAsync(int id)
        {
            var teacherCourse = await unitOfWork.TeacherCourses.GetByIdAsync(id);
            if (teacherCourse == null)
                throw new NotFoundException<int>(nameof(teacherCourse), "teacherCourse", id);

            ValidateCenterAccess(teacherCourse.CenterId, nameof(TeacherCourse));

            teacherCourse.IsDeleted = false;
            
            var currentUser = await userManager.FindByIdAsync(tenantContext.GetCurrentUserId());
            teacherCourse.ModifiedById = currentUser!.Id;
            teacherCourse.ModifiedByName = currentUser!.UserName ?? string.Empty;
            teacherCourse.ModifiedAt = DateTimeOffset.UtcNow;
            
            unitOfWork.TeacherCourses.UpdateAsync(teacherCourse);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task ActivateAsync(int id)
        {
            var teacherCourse = await unitOfWork.TeacherCourses.GetByIdAsync(id);
            if (teacherCourse == null)
                throw new NotFoundException<int>(nameof(teacherCourse), "teacherCourse", id);

            ValidateCenterAccess(teacherCourse.CenterId, nameof(TeacherCourse));

            teacherCourse.IsActive = true;
            
            var currentUser = await userManager.FindByIdAsync(tenantContext.GetCurrentUserId());
            teacherCourse.ModifiedById = currentUser!.Id;
            teacherCourse.ModifiedByName = currentUser!.UserName ?? string.Empty;
            teacherCourse.ModifiedAt = DateTimeOffset.UtcNow;
            
            unitOfWork.TeacherCourses.UpdateAsync(teacherCourse);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeactivateAsync(int id)
        {
            var teacherCourse = await unitOfWork.TeacherCourses.GetByIdAsync(id);
            if (teacherCourse == null)
                throw new NotFoundException<int>(nameof(teacherCourse), "teacherCourse", id);

            ValidateCenterAccess(teacherCourse.CenterId, nameof(TeacherCourse));

            teacherCourse.IsActive = false;
            
            var currentUser = await userManager.FindByIdAsync(tenantContext.GetCurrentUserId());
            teacherCourse.ModifiedById = currentUser!.Id;
            teacherCourse.ModifiedByName = currentUser!.UserName ?? string.Empty;
            teacherCourse.ModifiedAt = DateTimeOffset.UtcNow;
            
            unitOfWork.TeacherCourses.UpdateAsync(teacherCourse);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> IsTeacherAssignedToCourseAsync(int teacherId, int courseId)
        {
            var teacherCourses = await unitOfWork.TeacherCourses.GetAllAsync(new PaginationParamter());
            return teacherCourses.Any(tc => tc.TeacherId == teacherId && tc.CourseId == courseId && !tc.IsDeleted);
        }
    }
}