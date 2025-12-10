using Moshrefy.Application.DTOs.Item;
using Moshrefy.Domain.Enums;
using Moshrefy.Domain.Paramter;

namespace Moshrefy.Application.Interfaces.IServices
{
    public interface IItemService
    {
        Task<ItemResponseDTO> CreateAsync(CreateItemDTO createItemDTO);
        Task<ItemResponseDTO?> GetByIdAsync(int id);
        Task<List<ItemResponseDTO>> GetAllAsync(PaginationParamter paginationParamter);
        Task<List<ItemResponseDTO>> GetByNameAsync(string name);
        Task<List<ItemResponseDTO>> GetByStatusAsync(ItemStatus status);
        Task<List<ItemResponseDTO>> GetAvailableItemsAsync(PaginationParamter paginationParamter);
        Task<List<ItemResponseDTO>> GetReservedItemsAsync(PaginationParamter paginationParamter);
        Task UpdateAsync(int id, UpdateItemDTO updateItemDTO);
        Task DeleteAsync(int id);
        Task SoftDeleteAsync(int id);
        Task RestoreAsync(int id);
        Task ReserveItemAsync(int itemId, int studentId);
        Task ReturnItemAsync(int itemId);
        Task UpdateStatusAsync(int id, ItemStatus status);
    }
}