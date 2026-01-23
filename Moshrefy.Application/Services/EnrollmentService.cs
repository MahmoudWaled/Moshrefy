using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moshrefy.Application.DTOs.Enrollment;
using Moshrefy.Application.Interfaces.IUnitOfWork;
using Moshrefy.Application.Interfaces.IServices;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Exceptions;
using Moshrefy.Domain.Identity;
using Moshrefy.Domain.Paramter;

namespace Moshrefy.Application.Services
{
    public class EnrollmentService(
        IUnitOfWork unitOfWork, 
        IMapper mapper,
        ITenantContext tenantContext,
        UserManager<ApplicationUser> userManager
    ) : BaseService(tenantContext), IEnrollmentService
    {
        public async Task<EnrollmentResponseDTO> CreateAsync(CreateEnrollmentDTO createEnrollmentDTO)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var currentUserId = tenantContext.GetCurrentUserId();
            var currentUser = await userManager.FindByIdAsync(currentUserId);
            
            var existing = await unitOfWork.Enrollments.GetAllAsync(
                e => e.CenterId == currentCenterId && e.StudentId == createEnrollmentDTO.StudentId && e.CourseId == createEnrollmentDTO.CourseId && !e.IsDeleted,
                new PaginationParameter { PageSize = 1 });
            if (existing.Any())
            {
                throw new BadRequestException("Student is already enrolled in this course.");
            }

            var enrollment = mapper.Map<Enrollment>(createEnrollmentDTO);
            enrollment.CenterId = currentCenterId;
            enrollment.CreatedById = currentUserId;
            enrollment.CreatedByName = currentUser?.Name ?? "System";
            enrollment.CreatedAt = DateTimeOffset.UtcNow;
            
            await unitOfWork.Enrollments.AddAsync(enrollment);
            await unitOfWork.SaveChangesAsync();
            return mapper.Map<EnrollmentResponseDTO>(enrollment);
        }

        public async Task<EnrollmentResponseDTO?> GetByIdAsync(int id)
        {
            var enrollment = await unitOfWork.Enrollments.GetByIdAsync(id);
            if (enrollment == null)
                throw new NotFoundException<int>(nameof(enrollment), "enrollment", id);

            ValidateCenterAccess(enrollment.CenterId, nameof(Enrollment));
            return mapper.Map<EnrollmentResponseDTO>(enrollment);
        }

        public async Task<List<EnrollmentResponseDTO>> GetAllAsync(PaginationParameter paginationParamter)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var enrollments = await unitOfWork.Enrollments.GetAllAsync(
                e => e.CenterId == currentCenterId && !e.IsDeleted,
                paginationParamter);
            return mapper.Map<List<EnrollmentResponseDTO>>(enrollments.ToList());
        }

        public async Task<List<EnrollmentResponseDTO>> GetByStudentIdAsync(int studentId)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var enrollments = await unitOfWork.Enrollments.GetAllAsync(
                e => e.CenterId == currentCenterId && e.StudentId == studentId && !e.IsDeleted,
                new PaginationParameter { PageSize = 1000 });
            return mapper.Map<List<EnrollmentResponseDTO>>(enrollments.ToList());
        }

        public async Task<List<EnrollmentResponseDTO>> GetByCourseIdAsync(int courseId)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var enrollments = await unitOfWork.Enrollments.GetAllAsync(
                e => e.CenterId == currentCenterId && e.CourseId == courseId && !e.IsDeleted,
                new PaginationParameter { PageSize = 1000 });
            return mapper.Map<List<EnrollmentResponseDTO>>(enrollments.ToList());
        }

        public async Task<List<EnrollmentResponseDTO>> GetActiveAsync(PaginationParameter paginationParamter)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var enrollments = await unitOfWork.Enrollments.GetAllAsync(
                e => e.CenterId == currentCenterId && e.IsActive && !e.IsDeleted,
                paginationParamter);
            return mapper.Map<List<EnrollmentResponseDTO>>(enrollments.ToList());
        }

        public async Task<List<EnrollmentResponseDTO>> GetInactiveAsync(PaginationParameter paginationParamter)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var enrollments = await unitOfWork.Enrollments.GetAllAsync(
                e => e.CenterId == currentCenterId && !e.IsActive && !e.IsDeleted,
                paginationParamter);
            return mapper.Map<List<EnrollmentResponseDTO>>(enrollments.ToList());
        }

        public async Task UpdateAsync(int id, UpdateEnrollmentDTO updateEnrollmentDTO)
        {
            var enrollment = await unitOfWork.Enrollments.GetByIdAsync(id);
            if (enrollment == null)
                throw new NotFoundException<int>(nameof(enrollment), "enrollment", id);

            ValidateCenterAccess(enrollment.CenterId, nameof(Enrollment));
            
            var currentUserId = tenantContext.GetCurrentUserId();
            var currentUser = await userManager.FindByIdAsync(currentUserId);
            
            mapper.Map(updateEnrollmentDTO, enrollment);
            enrollment.ModifiedById = currentUserId;
            enrollment.ModifiedByName = currentUser?.Name ?? "System";
            enrollment.ModifiedAt = DateTimeOffset.UtcNow;
            
            unitOfWork.Enrollments.Update(enrollment);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var enrollment = await unitOfWork.Enrollments.GetByIdAsync(id);
            if (enrollment == null)
                throw new NotFoundException<int>(nameof(enrollment), "enrollment", id);

            ValidateCenterAccess(enrollment.CenterId, nameof(Enrollment));
            unitOfWork.Enrollments.SoftDelete(enrollment);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int id)
        {
            var enrollment = await unitOfWork.Enrollments.GetByIdAsync(id);
            if (enrollment == null)
                throw new NotFoundException<int>(nameof(enrollment), "enrollment", id);

            ValidateCenterAccess(enrollment.CenterId, nameof(Enrollment));
            enrollment.IsDeleted = true;
            unitOfWork.Enrollments.Update(enrollment);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task RestoreAsync(int id)
        {
            var enrollment = await unitOfWork.Enrollments.GetByIdAsync(id);
            if (enrollment == null)
                throw new NotFoundException<int>(nameof(enrollment), "enrollment", id);

            ValidateCenterAccess(enrollment.CenterId, nameof(Enrollment));
            enrollment.IsDeleted = false;
            unitOfWork.Enrollments.Update(enrollment);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task ActivateAsync(int id)
        {
            var enrollment = await unitOfWork.Enrollments.GetByIdAsync(id);
            if (enrollment == null)
                throw new NotFoundException<int>(nameof(enrollment), "enrollment", id);

            ValidateCenterAccess(enrollment.CenterId, nameof(Enrollment));
            enrollment.IsActive = true;
            unitOfWork.Enrollments.Update(enrollment);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeactivateAsync(int id)
        {
            var enrollment = await unitOfWork.Enrollments.GetByIdAsync(id);
            if (enrollment == null)
                throw new NotFoundException<int>(nameof(enrollment), "enrollment", id);

            ValidateCenterAccess(enrollment.CenterId, nameof(Enrollment));
            enrollment.IsActive = false;
            unitOfWork.Enrollments.Update(enrollment);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> IsStudentEnrolledInCourseAsync(int studentId, int courseId)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var enrollments = await unitOfWork.Enrollments.GetAllAsync(
                e => e.CenterId == currentCenterId && e.StudentId == studentId && e.CourseId == courseId && !e.IsDeleted,
                new PaginationParameter { PageSize = 1 });
            return enrollments.Any();
        }

        // Bulk enrollment: Enroll ONE student in MULTIPLE courses
        public async Task<(int successCount, int duplicateCount)> BulkEnrollStudentInCoursesAsync(int studentId, List<int> courseIds)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var currentUserId = tenantContext.GetCurrentUserId();
            var currentUser = await userManager.FindByIdAsync(currentUserId);
            
            int successCount = 0;
            int duplicateCount = 0;

            foreach (var courseId in courseIds)
            {
                var exists = await unitOfWork.Enrollments.GetAllAsync(
                    e => e.CenterId == currentCenterId && e.StudentId == studentId && e.CourseId == courseId && !e.IsDeleted,
                    new PaginationParameter { PageSize = 1 });
                    
                if (exists.Any())
                {
                    duplicateCount++;
                    continue;
                }

                var enrollment = new Enrollment
                {
                    StudentId = studentId,
                    CourseId = courseId,
                    CenterId = currentCenterId,
                    IsActive = true,
                    CreatedById = currentUserId,
                    CreatedByName = currentUser?.Name ?? "System",
                    CreatedAt = DateTimeOffset.UtcNow
                };

                await unitOfWork.Enrollments.AddAsync(enrollment);
                successCount++;
            }

            if (successCount > 0)
            {
                await unitOfWork.SaveChangesAsync();
            }

            return (successCount, duplicateCount);
        }

        // Bulk enrollment: Enroll MULTIPLE students in ONE course
        public async Task<(int successCount, int duplicateCount)> BulkEnrollStudentsInCourseAsync(int courseId, List<int> studentIds)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var currentUserId = tenantContext.GetCurrentUserId();
            var currentUser = await userManager.FindByIdAsync(currentUserId);
            
            int successCount = 0;
            int duplicateCount = 0;

            foreach (var studentId in studentIds)
            {
                var exists = await unitOfWork.Enrollments.GetAllAsync(
                    e => e.CenterId == currentCenterId && e.StudentId == studentId && e.CourseId == courseId && !e.IsDeleted,
                    new PaginationParameter { PageSize = 1 });
                    
                if (exists.Any())
                {
                    duplicateCount++;
                    continue;
                }

                var enrollment = new Enrollment
                {
                    StudentId = studentId,
                    CourseId = courseId,
                    CenterId = currentCenterId,
                    IsActive = true,
                    CreatedById = currentUserId,
                    CreatedByName = currentUser?.Name ?? "System",
                    CreatedAt = DateTimeOffset.UtcNow
                };

                await unitOfWork.Enrollments.AddAsync(enrollment);
                successCount++;
            }

            if (successCount > 0)
            {
                await unitOfWork.SaveChangesAsync();
            }

            return (successCount, duplicateCount);
        }
    }
}
