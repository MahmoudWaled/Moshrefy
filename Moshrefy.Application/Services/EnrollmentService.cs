using AutoMapper;
using Moshrefy.Application.DTOs.Enrollment;
using Moshrefy.Application.Interfaces.IUnitOfWork;
using Moshrefy.Application.Interfaces.IServices;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Exceptions;
using Moshrefy.Domain.Paramter;

namespace Moshrefy.Application.Services
{
    public class EnrollmentService(IUnitOfWork unitOfWork, IMapper mapper) : IEnrollmentService
    {
        public async Task<EnrollmentResponseDTO> CreateAsync(CreateEnrollmentDTO createEnrollmentDTO)
        {
            var existing = await unitOfWork.Enrollments.GetAllAsync(new PaginationParamter());
            if (existing.Any(e => e.StudentId == createEnrollmentDTO.StudentId && e.CourseId == createEnrollmentDTO.CourseId))
            {
                throw new BadRequestException("Student is already enrolled in this course.");
            }

            var enrollment = mapper.Map<Enrollment>(createEnrollmentDTO);
            await unitOfWork.Enrollments.AddAsync(enrollment);
            await unitOfWork.SaveChangesAsync();
            return mapper.Map<EnrollmentResponseDTO>(enrollment);
        }

        public async Task<EnrollmentResponseDTO?> GetByIdAsync(int id)
        {
            var enrollment = await unitOfWork.Enrollments.GetByIdAsync(id);
            if (enrollment == null)
                throw new NotFoundException<int>(nameof(enrollment), "enrollment", id);

            return mapper.Map<EnrollmentResponseDTO>(enrollment);
        }

        public async Task<List<EnrollmentResponseDTO>> GetAllAsync(PaginationParamter paginationParamter)
        {
            var enrollments = await unitOfWork.Enrollments.GetAllAsync(paginationParamter);
            return mapper.Map<List<EnrollmentResponseDTO>>(enrollments);
        }

        public async Task<List<EnrollmentResponseDTO>> GetByStudentIdAsync(int studentId)
        {
            var enrollments = await unitOfWork.Enrollments.GetAllAsync(new PaginationParamter());
            var filtered = enrollments.Where(e => e.StudentId == studentId).ToList();
            return mapper.Map<List<EnrollmentResponseDTO>>(filtered);
        }

        public async Task<List<EnrollmentResponseDTO>> GetByCourseIdAsync(int courseId)
        {
            var enrollments = await unitOfWork.Enrollments.GetAllAsync(new PaginationParamter());
            var filtered = enrollments.Where(e => e.CourseId == courseId).ToList();
            return mapper.Map<List<EnrollmentResponseDTO>>(filtered);
        }

        public async Task<List<EnrollmentResponseDTO>> GetActiveAsync(PaginationParamter paginationParamter)
        {
            var enrollments = await unitOfWork.Enrollments.GetAllAsync(paginationParamter);
            var active = enrollments.Where(e => e.IsActive).ToList();
            return mapper.Map<List<EnrollmentResponseDTO>>(active);
        }

        public async Task<List<EnrollmentResponseDTO>> GetInactiveAsync(PaginationParamter paginationParamter)
        {
            var enrollments = await unitOfWork.Enrollments.GetAllAsync(paginationParamter);
            var inactive = enrollments.Where(e => !e.IsActive).ToList();
            return mapper.Map<List<EnrollmentResponseDTO>>(inactive);
        }

        public async Task UpdateAsync(int id, UpdateEnrollmentDTO updateEnrollmentDTO)
        {
            var enrollment = await unitOfWork.Enrollments.GetByIdAsync(id);
            if (enrollment == null)
                throw new NotFoundException<int>(nameof(enrollment), "enrollment", id);

            mapper.Map(updateEnrollmentDTO, enrollment);
            unitOfWork.Enrollments.UpdateAsync(enrollment);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var enrollment = await unitOfWork.Enrollments.GetByIdAsync(id);
            if (enrollment == null)
                throw new NotFoundException<int>(nameof(enrollment), "enrollment", id);

            unitOfWork.Enrollments.DeleteAsync(enrollment);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int id)
        {
            var enrollment = await unitOfWork.Enrollments.GetByIdAsync(id);
            if (enrollment == null)
                throw new NotFoundException<int>(nameof(enrollment), "enrollment", id);

            enrollment.IsDeleted = true;
            unitOfWork.Enrollments.UpdateAsync(enrollment);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task RestoreAsync(int id)
        {
            var enrollment = await unitOfWork.Enrollments.GetByIdAsync(id);
            if (enrollment == null)
                throw new NotFoundException<int>(nameof(enrollment), "enrollment", id);

            enrollment.IsDeleted = false;
            unitOfWork.Enrollments.UpdateAsync(enrollment);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task ActivateAsync(int id)
        {
            var enrollment = await unitOfWork.Enrollments.GetByIdAsync(id);
            if (enrollment == null)
                throw new NotFoundException<int>(nameof(enrollment), "enrollment", id);

            enrollment.IsActive = true;
            unitOfWork.Enrollments.UpdateAsync(enrollment);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeactivateAsync(int id)
        {
            var enrollment = await unitOfWork.Enrollments.GetByIdAsync(id);
            if (enrollment == null)
                throw new NotFoundException<int>(nameof(enrollment), "enrollment", id);

            enrollment.IsActive = false;
            unitOfWork.Enrollments.UpdateAsync(enrollment);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> IsStudentEnrolledInCourseAsync(int studentId, int courseId)
        {
            var enrollments = await unitOfWork.Enrollments.GetAllAsync(new PaginationParamter());
            return enrollments.Any(e => e.StudentId == studentId && e.CourseId == courseId && !e.IsDeleted);
        }
    }
}