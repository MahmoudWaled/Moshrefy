using Moshrefy.Application.DTOs.TeacherCourse;
using Moshrefy.Domain.Paramter;

namespace Moshrefy.Application.Interfaces.IServices
{
    public interface ITeacherCourseService
    {
        Task<TeacherCourseResponseDTO> CreateAsync(CreateTeacherCourseDTO createTeacherCourseDTO);
        Task<TeacherCourseResponseDTO?> GetByIdAsync(int id);
        Task<List<TeacherCourseResponseDTO>> GetAllAsync(PaginationParamter paginationParamter);
        Task<List<TeacherCourseResponseDTO>> GetByTeacherIdAsync(int teacherId);
        Task<List<TeacherCourseResponseDTO>> GetByCourseIdAsync(int courseId);
        Task<List<TeacherCourseResponseDTO>> GetActiveAsync(PaginationParamter paginationParamter);
        Task<List<TeacherCourseResponseDTO>> GetInactiveAsync(PaginationParamter paginationParamter);
        Task UpdateAsync(int id, UpdateTeacherCourseDTO updateTeacherCourseDTO);
        Task DeleteAsync(int id);
        Task SoftDeleteAsync(int id);
        Task RestoreAsync(int id);
        Task ActivateAsync(int id);
        Task DeactivateAsync(int id);
        Task<bool> IsTeacherAssignedToCourseAsync(int teacherId, int courseId);
    }
}