
using Moshrefy.Application.DTOs.Center;
using Moshrefy.Application.DTOs.Common;
using Moshrefy.Domain.Paramter;

namespace Moshrefy.Application.Interfaces.IServices
{
    public interface ICenterService
    {
        Task<CenterResponseDTO> CreateAsync(CreateCenterDTO createCenterDTO);
        Task<CenterResponseDTO?> GetByIdAsync(int id);
        Task<CenterResponseDTO?> GetByEmailAsync(string email);
        Task<List<CenterResponseDTO>> GetAllAsync(PaginationParameter paginationParamter);
        Task<PaginatedResult<CenterResponseDTO>>  GetNonDeletedAsync(PaginationParameter paginationParamter);
        Task<PaginatedResult<CenterResponseDTO>> GetDeletedAsync(PaginationParameter paginationParamter);
        Task<PaginatedResult<CenterResponseDTO>> GetActiveAsync(PaginationParameter paginationParamter);
        Task<PaginatedResult<CenterResponseDTO>> GetInactiveAsync(PaginationParameter paginationParamter);
        Task<PaginatedResult<CenterResponseDTO>> GetCentersPagedAsync(PaginationParameter paginationParameter, string status);
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