using Moshrefy.Application.DTOs.ExamResult;
using Moshrefy.Domain.Enums;
using Moshrefy.Domain.Paramter;

namespace Moshrefy.Application.Interfaces.IServices
{
    public interface IExamResultService
    {
        Task<ExamResultResponseDTO> CreateAsync(CreateExamResultDTO createExamResultDTO);
        Task<ExamResultResponseDTO?> GetByIdAsync(int id);
        Task<List<ExamResultResponseDTO>> GetAllAsync(PaginationParameter paginationParamter);
        Task<List<ExamResultResponseDTO>> GetByStudentIdAsync(int studentId);
        Task<List<ExamResultResponseDTO>> GetByExamIdAsync(int examId);
        Task<List<ExamResultResponseDTO>> GetByStatusAsync(ExamResultStatus status);
        Task UpdateAsync(int id, UpdateExamResultDTO updateExamResultDTO);
        Task DeleteAsync(int id);
        Task SoftDeleteAsync(int id);
        Task RestoreAsync(int id);
    }
}