using Moshrefy.Application.DTOs.Teacher;
using Moshrefy.Domain.Paramter;

namespace Moshrefy.Application.Interfaces.IServices
{
    public interface ITeacherService
    {
        Task<TeacherResponseDTO> CreateAsync(CreateTeacherDTO createTeacherDTO);
        Task<TeacherResponseDTO?> GetByIdAsync(int id);
        Task<List<TeacherResponseDTO>> GetAllAsync(PaginationParamter paginationParamter);
        Task<List<TeacherResponseDTO>> GetByNameAsync(string name);
        Task<List<TeacherResponseDTO>> GetActiveAsync(PaginationParamter paginationParamter);
        Task<List<TeacherResponseDTO>> GetInactiveAsync(PaginationParamter paginationParamter);
        Task<TeacherResponseDTO?> GetByPhoneNumberAsync(string phoneNumber);
        Task UpdateAsync(int id, UpdateTeacherDTO updateTeacherDTO);
        Task DeleteAsync(int id);
        Task SoftDeleteAsync(int id);
        Task RestoreAsync(int id);
        Task ActivateAsync(int id);
        Task DeactivateAsync(int id);
    }
}