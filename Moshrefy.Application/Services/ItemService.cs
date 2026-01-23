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
    public class ItemService(
        IUnitOfWork unitOfWork, 
        IMapper mapper,
        ITenantContext tenantContext
    ) : BaseService(tenantContext), IItemService
    {
        public async Task<ItemResponseDTO> CreateAsync(CreateItemDTO createItemDTO)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var item = mapper.Map<Item>(createItemDTO);
            item.CenterId = currentCenterId;
            await unitOfWork.Items.AddAsync(item);
            await unitOfWork.SaveChangesAsync();
            return mapper.Map<ItemResponseDTO>(item);
        }

        public async Task<ItemResponseDTO?> GetByIdAsync(int id)
        {
            var item = await unitOfWork.Items.GetByIdAsync(id);
            if (item == null)
                throw new NotFoundException<int>(nameof(item), "item", id);

            ValidateCenterAccess(item.CenterId, nameof(Item));
            return mapper.Map<ItemResponseDTO>(item);
        }

        public async Task<List<ItemResponseDTO>> GetAllAsync(PaginationParameter paginationParamter)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var items = await unitOfWork.Items.GetAllAsync(
                i => i.CenterId == currentCenterId && !i.IsDeleted,
                paginationParamter);
            return mapper.Map<List<ItemResponseDTO>>(items.ToList());
        }

        public async Task<List<ItemResponseDTO>> GetByNameAsync(string name)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var items = await unitOfWork.Items.GetAllAsync(
                i => i.CenterId == currentCenterId && i.Name.Contains(name) && !i.IsDeleted,
                new PaginationParameter { PageSize = 1000 });
            return mapper.Map<List<ItemResponseDTO>>(items.ToList());
        }

        public async Task<List<ItemResponseDTO>> GetByStatusAsync(ItemStatus status)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var items = await unitOfWork.Items.GetAllAsync(
                i => i.CenterId == currentCenterId && i.ItemStatus == status && !i.IsDeleted,
                new PaginationParameter { PageSize = 1000 });
            return mapper.Map<List<ItemResponseDTO>>(items.ToList());
        }

        public async Task<List<ItemResponseDTO>> GetAvailableItemsAsync(PaginationParameter paginationParamter)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var items = await unitOfWork.Items.GetAllAsync(
                i => i.CenterId == currentCenterId && i.ItemStatus == ItemStatus.Available && !i.IsDeleted,
                paginationParamter);
            return mapper.Map<List<ItemResponseDTO>>(items.ToList());
        }

        public async Task<List<ItemResponseDTO>> GetReservedItemsAsync(PaginationParameter paginationParamter)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var items = await unitOfWork.Items.GetAllAsync(
                i => i.CenterId == currentCenterId && i.ReservedByStudentId != null && !i.IsDeleted,
                paginationParamter);
            return mapper.Map<List<ItemResponseDTO>>(items.ToList());
        }

        public async Task UpdateAsync(int id, UpdateItemDTO updateItemDTO)
        {
            var item = await unitOfWork.Items.GetByIdAsync(id);
            if (item == null)
                throw new NotFoundException<int>(nameof(item), "item", id);

            ValidateCenterAccess(item.CenterId, nameof(Item));
            mapper.Map(updateItemDTO, item);
            unitOfWork.Items.Update(item);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var item = await unitOfWork.Items.GetByIdAsync(id);
            if (item == null)
                throw new NotFoundException<int>(nameof(item), "item", id);

            ValidateCenterAccess(item.CenterId, nameof(Item));
            unitOfWork.Items.SoftDelete(item);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int id)
        {
            var item = await unitOfWork.Items.GetByIdAsync(id);
            if (item == null)
                throw new NotFoundException<int>(nameof(item), "item", id);

            ValidateCenterAccess(item.CenterId, nameof(Item));
            item.IsDeleted = true;
            unitOfWork.Items.Update(item);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task RestoreAsync(int id)
        {
            var item = await unitOfWork.Items.GetByIdAsync(id);
            if (item == null)
                throw new NotFoundException<int>(nameof(item), "item", id);

            ValidateCenterAccess(item.CenterId, nameof(Item));
            item.IsDeleted = false;
            unitOfWork.Items.Update(item);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task ReserveItemAsync(int itemId, int studentId)
        {
            var item = await unitOfWork.Items.GetByIdAsync(itemId);
            if (item == null)
                throw new NotFoundException<int>(nameof(item), "item", itemId);

            ValidateCenterAccess(item.CenterId, nameof(Item));
            item.ReservedByStudentId = studentId;
            unitOfWork.Items.Update(item);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task ReturnItemAsync(int itemId)
        {
            var item = await unitOfWork.Items.GetByIdAsync(itemId);
            if (item == null)
                throw new NotFoundException<int>(nameof(item), "item", itemId);

            ValidateCenterAccess(item.CenterId, nameof(Item));
            item.ReservedByStudentId = null;
            unitOfWork.Items.Update(item);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateStatusAsync(int id, ItemStatus status)
        {
            var item = await unitOfWork.Items.GetByIdAsync(id);
            if (item == null)
                throw new NotFoundException<int>(nameof(item), "item", id);

            ValidateCenterAccess(item.CenterId, nameof(Item));
            item.ItemStatus = status;
            unitOfWork.Items.Update(item);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
