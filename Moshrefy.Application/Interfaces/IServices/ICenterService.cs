using Moshrefy.Application.DTOs.Center;
using Moshrefy.Domain.Paramter;

namespace Moshrefy.Application.Interfaces.IServices
{
    public interface ICenterService
    {
        Task<CenterResponseDTO> CreateAsync(CreateCenterDTO createCenterDTO);
        Task<CenterResponseDTO?> GetByIdAsync(int id);
        Task<List<CenterResponseDTO>> GetAllAsync(PaginationParamter paginationParamter);
        Task<List<CenterResponseDTO>> GetByNameAsync(string name);
        Task<List<CenterResponseDTO>> GetActiveAsync(PaginationParamter paginationParamter);
        Task<List<CenterResponseDTO>> GetInactiveAsync(PaginationParamter paginationParamter);
        Task UpdateAsync(int id, UpdateCenterDTO updateCenterDTO);
        Task DeleteAsync(int id);
        Task SoftDeleteCenterAsync(int id);
        Task RestoreCenterAsync(int id);
        Task ActivateAsync(int id);
        Task DeactivateAsync(int id);
    }
}