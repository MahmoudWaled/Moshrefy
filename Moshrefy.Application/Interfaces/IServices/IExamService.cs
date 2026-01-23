using Moshrefy.Application.DTOs.Exam;
using Moshrefy.Domain.Enums;
using Moshrefy.Domain.Paramter;

namespace Moshrefy.Application.Interfaces.IServices
{
    public interface IExamService
    {
        Task<ExamResponseDTO> CreateAsync(CreateExamDTO createExamDTO);
        Task<ExamResponseDTO?> GetByIdAsync(int id);
        Task<List<ExamResponseDTO>> GetAllAsync(PaginationParameter paginationParamter);
        Task<List<ExamResponseDTO>> GetByNameAsync(string name);
        Task<List<ExamResponseDTO>> GetByDateAsync(DateTime date);
        Task<List<ExamResponseDTO>> GetByCourseIdAsync(int courseId);
        Task<List<ExamResponseDTO>> GetByClassroomIdAsync(int classroomId);
        Task<List<ExamResponseDTO>> GetByTeacherIdAsync(int teacherId);
        Task<List<ExamResponseDTO>> GetByStatusAsync(ExamStatus status);
        Task UpdateAsync(int id, UpdateExamDTO updateExamDTO);
        Task DeleteAsync(int id);
        Task SoftDeleteAsync(int id);
        Task RestoreAsync(int id);
        Task UpdateStatusAsync(int id, ExamStatus status);
    }
}