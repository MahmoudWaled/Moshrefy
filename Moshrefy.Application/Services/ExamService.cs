using AutoMapper;
using Moshrefy.Application.DTOs.Exam;
using Moshrefy.Application.Interfaces.IUnitOfWork;
using Moshrefy.Application.Interfaces.IServices;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Enums;
using Moshrefy.Domain.Exceptions;
using Moshrefy.Domain.Paramter;

namespace Moshrefy.Application.Services
{
    public class ExamService(
        IUnitOfWork unitOfWork, 
        IMapper mapper,
        ITenantContext tenantContext
    ) : BaseService(tenantContext), IExamService
    {
        public async Task<ExamResponseDTO> CreateAsync(CreateExamDTO createExamDTO)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var exam = mapper.Map<Exam>(createExamDTO);
            exam.CenterId = currentCenterId;
            await unitOfWork.Exams.AddAsync(exam);
            await unitOfWork.SaveChangesAsync();
            return mapper.Map<ExamResponseDTO>(exam);
        }

        public async Task<ExamResponseDTO?> GetByIdAsync(int id)
        {
            var exam = await unitOfWork.Exams.GetByIdAsync(id);
            if (exam == null)
                throw new NotFoundException<int>(nameof(exam), "exam", id);

            ValidateCenterAccess(exam.CenterId, nameof(Exam));
            return mapper.Map<ExamResponseDTO>(exam);
        }

        public async Task<List<ExamResponseDTO>> GetAllAsync(PaginationParameter paginationParamter)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var exams = await unitOfWork.Exams.GetAllAsync(
                e => e.CenterId == currentCenterId && !e.IsDeleted,
                paginationParamter);
            return mapper.Map<List<ExamResponseDTO>>(exams.ToList());
        }

        public async Task<List<ExamResponseDTO>> GetByNameAsync(string name)
        {
            var exams = await unitOfWork.Exams.GetByName(name);
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var filtered = exams.Where(e => e.CenterId == currentCenterId && !e.IsDeleted).ToList();
            return mapper.Map<List<ExamResponseDTO>>(filtered);
        }

        public async Task<List<ExamResponseDTO>> GetByDateAsync(DateTime date)
        {
            var exams = await unitOfWork.Exams.GetByDate(date);
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var filtered = exams.Where(e => e.CenterId == currentCenterId && !e.IsDeleted).ToList();
            return mapper.Map<List<ExamResponseDTO>>(filtered);
        }

        public async Task<List<ExamResponseDTO>> GetByCourseIdAsync(int courseId)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var exams = await unitOfWork.Exams.GetAllAsync(
                e => e.CenterId == currentCenterId && e.CourseId == courseId && !e.IsDeleted,
                new PaginationParameter { PageSize = 1000 });
            return mapper.Map<List<ExamResponseDTO>>(exams.ToList());
        }

        public async Task<List<ExamResponseDTO>> GetByClassroomIdAsync(int classroomId)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var exams = await unitOfWork.Exams.GetAllAsync(
                e => e.CenterId == currentCenterId && e.ClassroomId == classroomId && !e.IsDeleted,
                new PaginationParameter { PageSize = 1000 });
            return mapper.Map<List<ExamResponseDTO>>(exams.ToList());
        }

        public async Task<List<ExamResponseDTO>> GetByTeacherIdAsync(int teacherId)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var exams = await unitOfWork.Exams.GetAllAsync(
                e => e.CenterId == currentCenterId && e.TeacherCourse != null && e.TeacherCourse.TeacherId == teacherId && !e.IsDeleted,
                new PaginationParameter { PageSize = 1000 });
            return mapper.Map<List<ExamResponseDTO>>(exams.ToList());
        }

        public async Task<List<ExamResponseDTO>> GetByStatusAsync(ExamStatus status)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var exams = await unitOfWork.Exams.GetAllAsync(
                e => e.CenterId == currentCenterId && e.ExamStatus == status && !e.IsDeleted,
                new PaginationParameter { PageSize = 1000 });
            return mapper.Map<List<ExamResponseDTO>>(exams.ToList());
        }

        public async Task UpdateAsync(int id, UpdateExamDTO updateExamDTO)
        {
            var exam = await unitOfWork.Exams.GetByIdAsync(id);
            if (exam == null)
                throw new NotFoundException<int>(nameof(exam), "exam", id);

            ValidateCenterAccess(exam.CenterId, nameof(Exam));
            mapper.Map(updateExamDTO, exam);
            unitOfWork.Exams.Update(exam);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var exam = await unitOfWork.Exams.GetByIdAsync(id);
            if (exam == null)
                throw new NotFoundException<int>(nameof(exam), "exam", id);

            ValidateCenterAccess(exam.CenterId, nameof(Exam));
            unitOfWork.Exams.SoftDelete(exam);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int id)
        {
            var exam = await unitOfWork.Exams.GetByIdAsync(id);
            if (exam == null)
                throw new NotFoundException<int>(nameof(exam), "exam", id);

            ValidateCenterAccess(exam.CenterId, nameof(Exam));
            exam.IsDeleted = true;
            unitOfWork.Exams.Update(exam);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task RestoreAsync(int id)
        {
            var exam = await unitOfWork.Exams.GetByIdAsync(id);
            if (exam == null)
                throw new NotFoundException<int>(nameof(exam), "exam", id);

            ValidateCenterAccess(exam.CenterId, nameof(Exam));
            exam.IsDeleted = false;
            unitOfWork.Exams.Update(exam);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateStatusAsync(int id, ExamStatus status)
        {
            var exam = await unitOfWork.Exams.GetByIdAsync(id);
            if (exam == null)
                throw new NotFoundException<int>(nameof(exam), "exam", id);

            ValidateCenterAccess(exam.CenterId, nameof(Exam));
            exam.ExamStatus = status;
            unitOfWork.Exams.Update(exam);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
