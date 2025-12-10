using Moshrefy.Application.DTOs.Course;
using Moshrefy.Domain.Paramter;

namespace Moshrefy.Application.Interfaces.IServices
{
    public interface ICourseService
    {
        Task<CourseResponseDTO> CreateAsync(CreateCourseDTO createCourseDTO);
        Task<CourseResponseDTO?> GetByIdAsync(int id);
        Task<List<CourseResponseDTO>> GetAllAsync(PaginationParamter paginationParamter);
        Task<List<CourseResponseDTO>> GetByNameAsync(string name);
        Task<List<CourseResponseDTO>> GetActiveAsync(PaginationParamter paginationParamter);
        Task<List<CourseResponseDTO>> GetInactiveAsync(PaginationParamter paginationParamter);
        Task<List<CourseResponseDTO>> GetByAcademicYearIdAsync(int academicYearId);
        Task UpdateAsync(int id, UpdateCourseDTO updateCourseDTO);
        Task DeleteAsync(int id);
        Task SoftDeleteAsync(int id);
        Task RestoreAsync(int id);
        Task ActivateAsync(int id);
        Task DeactivateAsync(int id);
    }
}