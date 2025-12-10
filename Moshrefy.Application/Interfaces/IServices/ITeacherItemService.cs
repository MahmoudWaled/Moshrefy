using Moshrefy.Application.DTOs.TeacherItem;
using Moshrefy.Domain.Paramter;

namespace Moshrefy.Application.Interfaces.IServices
{
    public interface ITeacherItemService
    {
        Task<TeacherItemResponseDTO> CreateAsync(CreateTeacherItemDTO createTeacherItemDTO);
        Task<TeacherItemResponseDTO?> GetByIdAsync(int id);
        Task<List<TeacherItemResponseDTO>> GetAllAsync(PaginationParamter paginationParamter);
        Task<List<TeacherItemResponseDTO>> GetByTeacherIdAsync(int teacherId);
        Task<List<TeacherItemResponseDTO>> GetByItemIdAsync(int itemId);
        Task<List<TeacherItemResponseDTO>> GetActiveAsync(PaginationParamter paginationParamter);
        Task<List<TeacherItemResponseDTO>> GetInactiveAsync(PaginationParamter paginationParamter);
        Task UpdateAsync(int id, UpdateTeacherItemDTO updateTeacherItemDTO);
        Task DeleteAsync(int id);
        Task SoftDeleteAsync(int id);
        Task RestoreAsync(int id);
        Task ActivateAsync(int id);
        Task DeactivateAsync(int id);
        Task<bool> IsTeacherAssignedToItemAsync(int teacherId, int itemId);
    }
}