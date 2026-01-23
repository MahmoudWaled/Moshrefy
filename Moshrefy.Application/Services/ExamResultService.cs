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
    public class ExamResultService(
        IUnitOfWork unitOfWork, 
        IMapper mapper,
        ITenantContext tenantContext
    ) : BaseService(tenantContext), IExamResultService
    {
        public async Task<ExamResultResponseDTO> CreateAsync(CreateExamResultDTO createExamResultDTO)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var examResult = mapper.Map<ExamResult>(createExamResultDTO);
            examResult.CenterId = currentCenterId;
            await unitOfWork.ExamResults.AddAsync(examResult);
            await unitOfWork.SaveChangesAsync();
            return mapper.Map<ExamResultResponseDTO>(examResult);
        }

        public async Task<ExamResultResponseDTO?> GetByIdAsync(int id)
        {
            var examResult = await unitOfWork.ExamResults.GetByIdAsync(id);
            if (examResult == null)
                throw new NotFoundException<int>(nameof(examResult), "examResult", id);

            ValidateCenterAccess(examResult.CenterId, nameof(ExamResult));
            return mapper.Map<ExamResultResponseDTO>(examResult);
        }

        public async Task<List<ExamResultResponseDTO>> GetAllAsync(PaginationParameter paginationParamter)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var examResults = await unitOfWork.ExamResults.GetAllAsync(
                er => er.CenterId == currentCenterId && !er.IsDeleted,
                paginationParamter);
            return mapper.Map<List<ExamResultResponseDTO>>(examResults.ToList());
        }

        public async Task<List<ExamResultResponseDTO>> GetByStudentIdAsync(int studentId)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var examResults = await unitOfWork.ExamResults.GetAllAsync(
                er => er.CenterId == currentCenterId && er.StudentId == studentId && !er.IsDeleted,
                new PaginationParameter { PageSize = 1000 });
            return mapper.Map<List<ExamResultResponseDTO>>(examResults.ToList());
        }

        public async Task<List<ExamResultResponseDTO>> GetByExamIdAsync(int examId)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var examResults = await unitOfWork.ExamResults.GetAllAsync(
                er => er.CenterId == currentCenterId && er.ExamId == examId && !er.IsDeleted,
                new PaginationParameter { PageSize = 1000 });
            return mapper.Map<List<ExamResultResponseDTO>>(examResults.ToList());
        }

        public async Task<List<ExamResultResponseDTO>> GetByStatusAsync(ExamResultStatus status)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var examResults = await unitOfWork.ExamResults.GetAllAsync(
                er => er.CenterId == currentCenterId && er.ExamResultStatus == status && !er.IsDeleted,
                new PaginationParameter { PageSize = 1000 });
            return mapper.Map<List<ExamResultResponseDTO>>(examResults.ToList());
        }

        public async Task UpdateAsync(int id, UpdateExamResultDTO updateExamResultDTO)
        {
            var examResult = await unitOfWork.ExamResults.GetByIdAsync(id);
            if (examResult == null)
                throw new NotFoundException<int>(nameof(examResult), "examResult", id);

            ValidateCenterAccess(examResult.CenterId, nameof(ExamResult));
            mapper.Map(updateExamResultDTO, examResult);
            unitOfWork.ExamResults.Update(examResult);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var examResult = await unitOfWork.ExamResults.GetByIdAsync(id);
            if (examResult == null)
                throw new NotFoundException<int>(nameof(examResult), "examResult", id);

            ValidateCenterAccess(examResult.CenterId, nameof(ExamResult));
            unitOfWork.ExamResults.SoftDelete(examResult);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int id)
        {
            var examResult = await unitOfWork.ExamResults.GetByIdAsync(id);
            if (examResult == null)
                throw new NotFoundException<int>(nameof(examResult), "examResult", id);

            ValidateCenterAccess(examResult.CenterId, nameof(ExamResult));
            examResult.IsDeleted = true;
            unitOfWork.ExamResults.Update(examResult);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task RestoreAsync(int id)
        {
            var examResult = await unitOfWork.ExamResults.GetByIdAsync(id);
            if (examResult == null)
                throw new NotFoundException<int>(nameof(examResult), "examResult", id);

            ValidateCenterAccess(examResult.CenterId, nameof(ExamResult));
            examResult.IsDeleted = false;
            unitOfWork.ExamResults.Update(examResult);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
