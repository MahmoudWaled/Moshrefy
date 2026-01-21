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
        Task<List<CenterResponseDTO>>  GetNonDeletedAsync(PaginationParamter paginationParamter);
        Task<List<CenterResponseDTO>> GetDeletedAsync(PaginationParamter paginationParamter);
        Task<List<CenterResponseDTO>> GetByNameAsync(string name);
        Task<List<CenterResponseDTO>> GetActiveAsync(PaginationParamter paginationParamter);
        Task<List<CenterResponseDTO>> GetInactiveAsync(PaginationParamter paginationParamter);
        Task UpdateAsync(int id, UpdateCenterDTO updateCenterDTO);
        Task HardDeleteAsync(int id);
        Task SoftDeleteAsync(int id);
        Task RestoreAsync(int id);
        Task ActivateAsync(int id);
        Task DeactivateAsync(int id);
        Task<int> GetTotalCountAsync();
        Task<int> GetNonDeletedCountAsync();
        Task<int> GetDeletedCountAsync();
        Task<PaginatedResult<CenterResponseDTO>> GetNonDeletedPagedAsync(PaginationParamter paginationParamter);
    }
}