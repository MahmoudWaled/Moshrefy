using Moshrefy.Application.DTOs.Classroom;
using Moshrefy.Domain.Paramter;

namespace Moshrefy.Application.Interfaces.IServices
{
    public interface IClassroomService
    {
        Task<ClassroomResponseDTO> CreateAsync(CreateClassroomDTO createClassroomDTO);
        Task<ClassroomResponseDTO?> GetByIdAsync(int id);
        Task<List<ClassroomResponseDTO>> GetAllAsync(PaginationParamter paginationParamter);
        Task<List<ClassroomResponseDTO>> GetByNameAsync(string name);
        Task<List<ClassroomResponseDTO>> GetActiveAsync(PaginationParamter paginationParamter);
        Task<List<ClassroomResponseDTO>> GetInactiveAsync(PaginationParamter paginationParamter);
        Task UpdateAsync(int id, UpdateClassroomDTO updateClassroomDTO);
        Task DeleteAsync(int id);
        Task SoftDeleteAsync(int id);
        Task RestoreAsync(int id);
        Task ActivateAsync(int id);
        Task DeactivateAsync(int id);
    }
}