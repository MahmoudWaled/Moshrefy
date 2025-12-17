using AutoMapper;
using Moshrefy.Application.DTOs.Enrollment;
using Moshrefy.Application.Interfaces.IUnitOfWork;
using Moshrefy.Application.Interfaces.IServices;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Exceptions;
using Moshrefy.Domain.Paramter;

namespace Moshrefy.Application.Services
{
    public class EnrollmentService(
        IUnitOfWork unitOfWork, 
        IMapper mapper,
        ITenantContext tenantContext
    ) : BaseService(tenantContext), IEnrollmentService
    {
        public async Task<EnrollmentResponseDTO> CreateAsync(CreateEnrollmentDTO createEnrollmentDTO)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            
            var existing = await unitOfWork.Enrollments.GetAllAsync(
                e => e.CenterId == currentCenterId && e.StudentId == createEnrollmentDTO.StudentId && e.CourseId == createEnrollmentDTO.CourseId && !e.IsDeleted,
                new PaginationParamter { PageSize = 1 });
            if (existing.Any())
            {
                throw new BadRequestException("Student is already enrolled in this course.");
            }

            var enrollment = mapper.Map<Enrollment>(createEnrollmentDTO);
            enrollment.CenterId = currentCenterId;
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

        public async Task<List<EnrollmentResponseDTO>> GetAllAsync(PaginationParamter paginationParamter)
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
                new PaginationParamter { PageSize = 1000 });
            return mapper.Map<List<EnrollmentResponseDTO>>(enrollments.ToList());
        }

        public async Task<List<EnrollmentResponseDTO>> GetByCourseIdAsync(int courseId)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var enrollments = await unitOfWork.Enrollments.GetAllAsync(
                e => e.CenterId == currentCenterId && e.CourseId == courseId && !e.IsDeleted,
                new PaginationParamter { PageSize = 1000 });
            return mapper.Map<List<EnrollmentResponseDTO>>(enrollments.ToList());
        }

        public async Task<List<EnrollmentResponseDTO>> GetActiveAsync(PaginationParamter paginationParamter)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var enrollments = await unitOfWork.Enrollments.GetAllAsync(
                e => e.CenterId == currentCenterId && e.IsActive && !e.IsDeleted,
                paginationParamter);
            return mapper.Map<List<EnrollmentResponseDTO>>(enrollments.ToList());
        }

        public async Task<List<EnrollmentResponseDTO>> GetInactiveAsync(PaginationParamter paginationParamter)
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
            mapper.Map(updateEnrollmentDTO, enrollment);
            unitOfWork.Enrollments.UpdateAsync(enrollment);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var enrollment = await unitOfWork.Enrollments.GetByIdAsync(id);
            if (enrollment == null)
                throw new NotFoundException<int>(nameof(enrollment), "enrollment", id);

            ValidateCenterAccess(enrollment.CenterId, nameof(Enrollment));
            unitOfWork.Enrollments.DeleteAsync(enrollment);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int id)
        {
            var enrollment = await unitOfWork.Enrollments.GetByIdAsync(id);
            if (enrollment == null)
                throw new NotFoundException<int>(nameof(enrollment), "enrollment", id);

            ValidateCenterAccess(enrollment.CenterId, nameof(Enrollment));
            enrollment.IsDeleted = true;
            unitOfWork.Enrollments.UpdateAsync(enrollment);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task RestoreAsync(int id)
        {
            var enrollment = await unitOfWork.Enrollments.GetByIdAsync(id);
            if (enrollment == null)
                throw new NotFoundException<int>(nameof(enrollment), "enrollment", id);

            ValidateCenterAccess(enrollment.CenterId, nameof(Enrollment));
            enrollment.IsDeleted = false;
            unitOfWork.Enrollments.UpdateAsync(enrollment);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task ActivateAsync(int id)
        {
            var enrollment = await unitOfWork.Enrollments.GetByIdAsync(id);
            if (enrollment == null)
                throw new NotFoundException<int>(nameof(enrollment), "enrollment", id);

            ValidateCenterAccess(enrollment.CenterId, nameof(Enrollment));
            enrollment.IsActive = true;
            unitOfWork.Enrollments.UpdateAsync(enrollment);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeactivateAsync(int id)
        {
            var enrollment = await unitOfWork.Enrollments.GetByIdAsync(id);
            if (enrollment == null)
                throw new NotFoundException<int>(nameof(enrollment), "enrollment", id);

            ValidateCenterAccess(enrollment.CenterId, nameof(Enrollment));
            enrollment.IsActive = false;
            unitOfWork.Enrollments.UpdateAsync(enrollment);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> IsStudentEnrolledInCourseAsync(int studentId, int courseId)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var enrollments = await unitOfWork.Enrollments.GetAllAsync(
                e => e.CenterId == currentCenterId && e.StudentId == studentId && e.CourseId == courseId && !e.IsDeleted,
                new PaginationParamter { PageSize = 1 });
            return enrollments.Any();
        }
    }
}
