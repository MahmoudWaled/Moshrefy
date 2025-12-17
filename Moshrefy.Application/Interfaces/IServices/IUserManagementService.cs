using Moshrefy.Application.DTOs.User;
using Moshrefy.Application.DTOs.Common;
using Moshrefy.Domain.Paramter;

namespace Moshrefy.Application.Interfaces.IServices
{
    // Service for managing users within the tenant's center context.

    public interface IUserManagementService
    {
        Task<UserResponseDTO> CreateUserAsync(CreateUserDTO createUserDTO);
        Task<List<UserResponseDTO>> GetAllUsersInMyCenterAsync(PaginationParamter paginationParamter);
        Task<List<UserResponseDTO>> GetActiveUsersInMyCenterAsync(PaginationParamter paginationParamter);
        Task<List<UserResponseDTO>> GetInactiveUsersInMyCenterAsync(PaginationParamter paginationParamter);
        Task<List<UserResponseDTO>> GetUsersByRoleInMyCenterAsync(string roleName, PaginationParamter paginationParamter);
        Task<UserResponseDTO> GetUserByIdInMyCenterAsync(string userId);
        Task UpdateUserAsync(string userId, UpdateUserDTO updateUserDTO);
        Task UpdateUserRoleAsync(string userId, string newRole);
        Task ActivateUserAsync(string userId);
        Task DeactivateUserAsync(string userId);
        Task SoftDeleteUserAsync(string userId);
        Task<int> GetTotalCountAsync();
        Task<DataTableResponse<UserResponseDTO>> GetUsersDataTableAsync(DataTableRequest request);
    }

}
