using Moshrefy.Application.DTOs.Center;
using Moshrefy.Application.DTOs.Common;
using Moshrefy.Domain.Paramter;

namespace Moshrefy.Application.Interfaces.IServices
{
    public interface ICenterService
    {
        Task<CenterResponseDTO> CreateAsync(CreateCenterDTO createCenterDTO);
        Task<CenterResponseDTO?> GetByIdAsync(int id);
        Task<List<CenterResponseDTO>> GetAllAsync(PaginationParamter paginationParamter);
        Task<PaginatedResult<CenterResponseDTO>>  GetNonDeletedAsync(PaginationParamter paginationParamter);
        Task<PaginatedResult<CenterResponseDTO>> GetDeletedAsync(PaginationParamter paginationParamter);
        Task<PaginatedResult<CenterResponseDTO>> GetActiveAsync(PaginationParamter paginationParamter);
        Task<PaginatedResult<CenterResponseDTO>> GetInactiveAsync(PaginationParamter paginationParamter);
        Task<List<CenterResponseDTO>> GetByNameAsync(string name);
        Task UpdateAsync(int id, UpdateCenterDTO updateCenterDTO);
        Task HardDeleteAsync(int id);
        Task SoftDeleteAsync(int id);
        Task RestoreAsync(int id);
        Task ActivateAsync(int id);
        Task DeactivateAsync(int id);
        Task<int> GetTotalCountAsync();
        Task<int> GetNonDeletedCountAsync();
        Task<int> GetDeletedCountAsync();
    }
}