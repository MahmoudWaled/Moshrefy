using AutoMapper;
using Moshrefy.Application.DTOs.Item;
using Moshrefy.Application.Interfaces.IUnitOfWork;
using Moshrefy.Application.Interfaces.IServices;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Enums;
using Moshrefy.Domain.Exceptions;
using Moshrefy.Domain.Paramter;

namespace Moshrefy.Application.Services
{
    public class ItemService(IUnitOfWork unitOfWork, IMapper mapper) : IItemService
    {
        public async Task<ItemResponseDTO> CreateAsync(CreateItemDTO createItemDTO)
        {
            var item = mapper.Map<Item>(createItemDTO);
            await unitOfWork.Items.AddAsync(item);
            await unitOfWork.SaveChangesAsync();
            return mapper.Map<ItemResponseDTO>(item);
        }

        public async Task<ItemResponseDTO?> GetByIdAsync(int id)
        {
            var item = await unitOfWork.Items.GetByIdAsync(id);
            if (item == null)
                throw new NotFoundException<int>(nameof(item), "item", id);

            return mapper.Map<ItemResponseDTO>(item);
        }

        public async Task<List<ItemResponseDTO>> GetAllAsync(PaginationParamter paginationParamter)
        {
            var items = await unitOfWork.Items.GetAllAsync(paginationParamter);
            return mapper.Map<List<ItemResponseDTO>>(items);
        }

        public async Task<List<ItemResponseDTO>> GetByNameAsync(string name)
        {
            var items = await unitOfWork.Items.GetAllAsync(new PaginationParamter());
            var filtered = items.Where(i => i.Name.Contains(name)).ToList();
            return mapper.Map<List<ItemResponseDTO>>(filtered);
        }

        public async Task<List<ItemResponseDTO>> GetByStatusAsync(ItemStatus status)
        {
            var items = await unitOfWork.Items.GetAllAsync(new PaginationParamter());
            var filtered = items.Where(i => i.ItemStatus == status).ToList();
            return mapper.Map<List<ItemResponseDTO>>(filtered);
        }

        public async Task<List<ItemResponseDTO>> GetAvailableItemsAsync(PaginationParamter paginationParamter)
        {
            var items = await unitOfWork.Items.GetAllAsync(paginationParamter);
            var availableItems = items.Where(i => i.ItemStatus == ItemStatus.Available).ToList();
            return mapper.Map<List<ItemResponseDTO>>(availableItems);
        }

        public async Task<List<ItemResponseDTO>> GetReservedItemsAsync(PaginationParamter paginationParamter)
        {
            var items = await unitOfWork.Items.GetAllAsync(paginationParamter);
            var reservedItems = items.Where(i => i.ReservedByStudentId != null).ToList();
            return mapper.Map<List<ItemResponseDTO>>(reservedItems);
        }

        public async Task UpdateAsync(int id, UpdateItemDTO updateItemDTO)
        {
            var item = await unitOfWork.Items.GetByIdAsync(id);
            if (item == null)
                throw new NotFoundException<int>(nameof(item), "item", id);

            mapper.Map(updateItemDTO, item);
            unitOfWork.Items.UpdateAsync(item);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var item = await unitOfWork.Items.GetByIdAsync(id);
            if (item == null)
                throw new NotFoundException<int>(nameof(item), "item", id);

            unitOfWork.Items.DeleteAsync(item);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int id)
        {
            var item = await unitOfWork.Items.GetByIdAsync(id);
            if (item == null)
                throw new NotFoundException<int>(nameof(item), "item", id);

            item.IsDeleted = true;
            unitOfWork.Items.UpdateAsync(item);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task RestoreAsync(int id)
        {
            var item = await unitOfWork.Items.GetByIdAsync(id);
            if (item == null)
                throw new NotFoundException<int>(nameof(item), "item", id);

            item.IsDeleted = false;
            unitOfWork.Items.UpdateAsync(item);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task ReserveItemAsync(int itemId, int studentId)
        {
            var item = await unitOfWork.Items.GetByIdAsync(itemId);
            if (item == null)
                throw new NotFoundException<int>(nameof(item), "item", itemId);

            item.ReservedByStudentId = studentId;
            unitOfWork.Items.UpdateAsync(item);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task ReturnItemAsync(int itemId)
        {
            var item = await unitOfWork.Items.GetByIdAsync(itemId);
            if (item == null)
                throw new NotFoundException<int>(nameof(item), "item", itemId);

            item.ReservedByStudentId = null;
            unitOfWork.Items.UpdateAsync(item);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateStatusAsync(int id, ItemStatus status)
        {
            var item = await unitOfWork.Items.GetByIdAsync(id);
            if (item == null)
                throw new NotFoundException<int>(nameof(item), "item", id);

            item.ItemStatus = status;
            unitOfWork.Items.UpdateAsync(item);
            await unitOfWork.SaveChangesAsync();
        }
    }
}