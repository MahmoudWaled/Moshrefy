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
    public class ExamService(IUnitOfWork unitOfWork, IMapper mapper) : IExamService
    {
        public async Task<ExamResponseDTO> CreateAsync(CreateExamDTO createExamDTO)
        {
            var exam = mapper.Map<Exam>(createExamDTO);
            await unitOfWork.Exams.AddAsync(exam);
            await unitOfWork.SaveChangesAsync();
            return mapper.Map<ExamResponseDTO>(exam);
        }

        public async Task<ExamResponseDTO?> GetByIdAsync(int id)
        {
            var exam = await unitOfWork.Exams.GetByIdAsync(id);
            if (exam == null)
                throw new NotFoundException<int>(nameof(exam), "exam", id);

            return mapper.Map<ExamResponseDTO>(exam);
        }

        public async Task<List<ExamResponseDTO>> GetAllAsync(PaginationParamter paginationParamter)
        {
            var exams = await unitOfWork.Exams.GetAllAsync(paginationParamter);
            return mapper.Map<List<ExamResponseDTO>>(exams);
        }

        public async Task<List<ExamResponseDTO>> GetByNameAsync(string name)
        {
            var exams = await unitOfWork.Exams.GetByName(name);
            return mapper.Map<List<ExamResponseDTO>>(exams);
        }

        public async Task<List<ExamResponseDTO>> GetByDateAsync(DateTime date)
        {
            var exams = await unitOfWork.Exams.GetByDate(date);
            return mapper.Map<List<ExamResponseDTO>>(exams);
        }

        public async Task<List<ExamResponseDTO>> GetByCourseIdAsync(int courseId)
        {
            var exams = await unitOfWork.Exams.GetAllAsync(new PaginationParamter());
            var filteredExams = exams.Where(e => e.CourseId == courseId).ToList();
            return mapper.Map<List<ExamResponseDTO>>(filteredExams);
        }

        public async Task<List<ExamResponseDTO>> GetByClassroomIdAsync(int classroomId)
        {
            var exams = await unitOfWork.Exams.GetAllAsync(new PaginationParamter());
            var filteredExams = exams.Where(e => e.ClassroomId == classroomId).ToList();
            return mapper.Map<List<ExamResponseDTO>>(filteredExams);
        }

        public async Task<List<ExamResponseDTO>> GetByTeacherIdAsync(int teacherId)
        {
            var exams = await unitOfWork.Exams.GetAllAsync(new PaginationParamter());
            var filteredExams = exams.Where(e => e.TeacherCourse != null && e.TeacherCourse.TeacherId == teacherId).ToList();
            return mapper.Map<List<ExamResponseDTO>>(filteredExams);
        }

        public async Task<List<ExamResponseDTO>> GetByStatusAsync(ExamStatus status)
        {
            var exams = await unitOfWork.Exams.GetAllAsync(new PaginationParamter());
            var filteredExams = exams.Where(e => e.ExamStatus == status).ToList();
            return mapper.Map<List<ExamResponseDTO>>(filteredExams);
        }

        public async Task UpdateAsync(int id, UpdateExamDTO updateExamDTO)
        {
            var exam = await unitOfWork.Exams.GetByIdAsync(id);
            if (exam == null)
                throw new NotFoundException<int>(nameof(exam), "exam", id);

            mapper.Map(updateExamDTO, exam);
            unitOfWork.Exams.UpdateAsync(exam);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var exam = await unitOfWork.Exams.GetByIdAsync(id);
            if (exam == null)
                throw new NotFoundException<int>(nameof(exam), "exam", id);

            unitOfWork.Exams.DeleteAsync(exam);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int id)
        {
            var exam = await unitOfWork.Exams.GetByIdAsync(id);
            if (exam == null)
                throw new NotFoundException<int>(nameof(exam), "exam", id);

            exam.IsDeleted = true;
            unitOfWork.Exams.UpdateAsync(exam);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task RestoreAsync(int id)
        {
            var exam = await unitOfWork.Exams.GetByIdAsync(id);
            if (exam == null)
                throw new NotFoundException<int>(nameof(exam), "exam", id);

            exam.IsDeleted = false;
            unitOfWork.Exams.UpdateAsync(exam);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateStatusAsync(int id, ExamStatus status)
        {
            var exam = await unitOfWork.Exams.GetByIdAsync(id);
            if (exam == null)
                throw new NotFoundException<int>(nameof(exam), "exam", id);

            exam.ExamStatus = status;
            unitOfWork.Exams.UpdateAsync(exam);
            await unitOfWork.SaveChangesAsync();
        }
    }
}