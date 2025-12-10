using AutoMapper;
using Moshrefy.Application.DTOs.ExamResult;
using Moshrefy.Application.Interfaces.IUnitOfWork;
using Moshrefy.Application.Interfaces.IServices;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Enums;
using Moshrefy.Domain.Exceptions;
using Moshrefy.Domain.Paramter;

namespace Moshrefy.Application.Services
{
    public class ExamResultService(IUnitOfWork unitOfWork, IMapper mapper) : IExamResultService
    {
        public async Task<ExamResultResponseDTO> CreateAsync(CreateExamResultDTO createExamResultDTO)
        {
            var examResult = mapper.Map<ExamResult>(createExamResultDTO);
            await unitOfWork.ExamResults.AddAsync(examResult);
            await unitOfWork.SaveChangesAsync();
            return mapper.Map<ExamResultResponseDTO>(examResult);
        }

        public async Task<ExamResultResponseDTO?> GetByIdAsync(int id)
        {
            var examResult = await unitOfWork.ExamResults.GetByIdAsync(id);
            if (examResult == null)
                throw new NotFoundException<int>(nameof(examResult), "examResult", id);

            return mapper.Map<ExamResultResponseDTO>(examResult);
        }

        public async Task<List<ExamResultResponseDTO>> GetAllAsync(PaginationParamter paginationParamter)
        {
            var examResults = await unitOfWork.ExamResults.GetAllAsync(paginationParamter);
            return mapper.Map<List<ExamResultResponseDTO>>(examResults);
        }

        public async Task<List<ExamResultResponseDTO>> GetByStudentIdAsync(int studentId)
        {
            var examResults = await unitOfWork.ExamResults.GetAllAsync(new PaginationParamter());
            var filtered = examResults.Where(er => er.StudentId == studentId).ToList();
            return mapper.Map<List<ExamResultResponseDTO>>(filtered);
        }

        public async Task<List<ExamResultResponseDTO>> GetByExamIdAsync(int examId)
        {
            var examResults = await unitOfWork.ExamResults.GetAllAsync(new PaginationParamter());
            var filtered = examResults.Where(er => er.ExamId == examId).ToList();
            return mapper.Map<List<ExamResultResponseDTO>>(filtered);
        }

        public async Task<List<ExamResultResponseDTO>> GetByStatusAsync(ExamResultStatus status)
        {
            var examResults = await unitOfWork.ExamResults.GetAllAsync(new PaginationParamter());
            var filtered = examResults.Where(er => er.ExamResultStatus == status).ToList();
            return mapper.Map<List<ExamResultResponseDTO>>(filtered);
        }

        public async Task UpdateAsync(int id, UpdateExamResultDTO updateExamResultDTO)
        {
            var examResult = await unitOfWork.ExamResults.GetByIdAsync(id);
            if (examResult == null)
                throw new NotFoundException<int>(nameof(examResult), "examResult", id);

            mapper.Map(updateExamResultDTO, examResult);
            unitOfWork.ExamResults.UpdateAsync(examResult);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var examResult = await unitOfWork.ExamResults.GetByIdAsync(id);
            if (examResult == null)
                throw new NotFoundException<int>(nameof(examResult), "examResult", id);

            unitOfWork.ExamResults.DeleteAsync(examResult);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int id)
        {
            var examResult = await unitOfWork.ExamResults.GetByIdAsync(id);
            if (examResult == null)
                throw new NotFoundException<int>(nameof(examResult), "examResult", id);

            examResult.IsDeleted = true;
            unitOfWork.ExamResults.UpdateAsync(examResult);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task RestoreAsync(int id)
        {
            var examResult = await unitOfWork.ExamResults.GetByIdAsync(id);
            if (examResult == null)
                throw new NotFoundException<int>(nameof(examResult), "examResult", id);

            examResult.IsDeleted = false;
            unitOfWork.ExamResults.UpdateAsync(examResult);
            await unitOfWork.SaveChangesAsync();
        }
    }
}