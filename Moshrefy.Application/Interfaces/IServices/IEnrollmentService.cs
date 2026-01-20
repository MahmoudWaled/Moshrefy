using Moshrefy.Application.DTOs.Enrollment;
using Moshrefy.Domain.Paramter;

namespace Moshrefy.Application.Interfaces.IServices
{
    public interface IEnrollmentService
    {
        Task<EnrollmentResponseDTO> CreateAsync(CreateEnrollmentDTO createEnrollmentDTO);
        Task<EnrollmentResponseDTO?> GetByIdAsync(int id);
        Task<List<EnrollmentResponseDTO>> GetAllAsync(PaginationParamter paginationParamter);
        Task<List<EnrollmentResponseDTO>> GetByStudentIdAsync(int studentId);
        Task<List<EnrollmentResponseDTO>> GetByCourseIdAsync(int courseId);
        Task<List<EnrollmentResponseDTO>> GetActiveAsync(PaginationParamter paginationParamter);
        Task<List<EnrollmentResponseDTO>> GetInactiveAsync(PaginationParamter paginationParamter);
        Task UpdateAsync(int id, UpdateEnrollmentDTO updateEnrollmentDTO);
        Task DeleteAsync(int id);
        Task SoftDeleteAsync(int id);
        Task RestoreAsync(int id);
        Task ActivateAsync(int id);
        Task DeactivateAsync(int id);
        Task<bool> IsStudentEnrolledInCourseAsync(int studentId, int courseId);
        Task<(int successCount, int duplicateCount)> BulkEnrollStudentInCoursesAsync(int studentId, List<int> courseIds);
        Task<(int successCount, int duplicateCount)> BulkEnrollStudentsInCourseAsync(int courseId, List<int> studentIds);
        
    }
}