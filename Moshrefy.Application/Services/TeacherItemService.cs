using AutoMapper;
using Moshrefy.Application.DTOs.TeacherItem;
using Moshrefy.Application.Interfaces.IUnitOfWork;
using Moshrefy.Application.Interfaces.IServices;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Exceptions;
using Moshrefy.Domain.Paramter;

namespace Moshrefy.Application.Services
{
    public class TeacherItemService(
        IUnitOfWork unitOfWork, 
        IMapper mapper,
        ITenantContext tenantContext
    ) : BaseService(tenantContext), ITeacherItemService
    {
        public async Task<TeacherItemResponseDTO> CreateAsync(CreateTeacherItemDTO createTeacherItemDTO)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            
            // Check if already exists
            var existing = await unitOfWork.TeacherItems.GetAllAsync(
                ti => ti.CenterId == currentCenterId && ti.TeacherId == createTeacherItemDTO.TeacherId && ti.ItemId == createTeacherItemDTO.ItemId,
                new PaginationParamter { PageSize = 1 });
            if (existing.Any())
            {
                throw new BadRequestException("Teacher is already assigned to this item.");
            }

            var teacherItem = mapper.Map<TeacherItem>(createTeacherItemDTO);
            teacherItem.CenterId = currentCenterId;
            await unitOfWork.TeacherItems.AddAsync(teacherItem);
            await unitOfWork.SaveChangesAsync();
            return mapper.Map<TeacherItemResponseDTO>(teacherItem);
        }

        public async Task<TeacherItemResponseDTO?> GetByIdAsync(int id)
        {
            var teacherItem = await unitOfWork.TeacherItems.GetByIdAsync(id);
            if (teacherItem == null)
                throw new NotFoundException<int>(nameof(teacherItem), "teacherItem", id);

            ValidateCenterAccess(teacherItem.CenterId, nameof(TeacherItem));
            return mapper.Map<TeacherItemResponseDTO>(teacherItem);
        }

        public async Task<List<TeacherItemResponseDTO>> GetAllAsync(PaginationParamter paginationParamter)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var teacherItems = await unitOfWork.TeacherItems.GetAllAsync(
                ti => ti.CenterId == currentCenterId && !ti.IsDeleted,
                paginationParamter);
            return mapper.Map<List<TeacherItemResponseDTO>>(teacherItems.ToList());
        }

        public async Task<List<TeacherItemResponseDTO>> GetByTeacherIdAsync(int teacherId)
        {
            var teacherItems = await unitOfWork.TeacherItems.GetByTeacherIdAsync(teacherId);
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var filtered = teacherItems.Where(ti => ti.CenterId == currentCenterId && !ti.IsDeleted).ToList();
            return mapper.Map<List<TeacherItemResponseDTO>>(filtered);
        }

        public async Task<List<TeacherItemResponseDTO>> GetByItemIdAsync(int itemId)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var teacherItems = await unitOfWork.TeacherItems.GetAllAsync(
                ti => ti.CenterId == currentCenterId && ti.ItemId == itemId && !ti.IsDeleted,
                new PaginationParamter { PageSize = 1000 });
            return mapper.Map<List<TeacherItemResponseDTO>>(teacherItems.ToList());
        }

        public async Task<List<TeacherItemResponseDTO>> GetActiveAsync(PaginationParamter paginationParamter)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var teacherItems = await unitOfWork.TeacherItems.GetAllAsync(
                ti => ti.CenterId == currentCenterId && ti.IsActive && !ti.IsDeleted,
                paginationParamter);
            return mapper.Map<List<TeacherItemResponseDTO>>(teacherItems.ToList());
        }

        public async Task<List<TeacherItemResponseDTO>> GetInactiveAsync(PaginationParamter paginationParamter)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var teacherItems = await unitOfWork.TeacherItems.GetAllAsync(
                ti => ti.CenterId == currentCenterId && !ti.IsActive && !ti.IsDeleted,
                paginationParamter);
            return mapper.Map<List<TeacherItemResponseDTO>>(teacherItems.ToList());
        }

        public async Task UpdateAsync(int id, UpdateTeacherItemDTO updateTeacherItemDTO)
        {
            var teacherItem = await unitOfWork.TeacherItems.GetByIdAsync(id);
            if (teacherItem == null)
                throw new NotFoundException<int>(nameof(teacherItem), "teacherItem", id);

            ValidateCenterAccess(teacherItem.CenterId, nameof(TeacherItem));
            mapper.Map(updateTeacherItemDTO, teacherItem);
            unitOfWork.TeacherItems.Update(teacherItem);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var teacherItem = await unitOfWork.TeacherItems.GetByIdAsync(id);
            if (teacherItem == null)
                throw new NotFoundException<int>(nameof(teacherItem), "teacherItem", id);

            ValidateCenterAccess(teacherItem.CenterId, nameof(TeacherItem));
            unitOfWork.TeacherItems.SoftDelete(teacherItem);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int id)
        {
            var teacherItem = await unitOfWork.TeacherItems.GetByIdAsync(id);
            if (teacherItem == null)
                throw new NotFoundException<int>(nameof(teacherItem), "teacherItem", id);

            ValidateCenterAccess(teacherItem.CenterId, nameof(TeacherItem));
            teacherItem.IsDeleted = true;
            unitOfWork.TeacherItems.Update(teacherItem);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task RestoreAsync(int id)
        {
            var teacherItem = await unitOfWork.TeacherItems.GetByIdAsync(id);
            if (teacherItem == null)
                throw new NotFoundException<int>(nameof(teacherItem), "teacherItem", id);

            ValidateCenterAccess(teacherItem.CenterId, nameof(TeacherItem));
            teacherItem.IsDeleted = false;
            unitOfWork.TeacherItems.Update(teacherItem);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task ActivateAsync(int id)
        {
            var teacherItem = await unitOfWork.TeacherItems.GetByIdAsync(id);
            if (teacherItem == null)
                throw new NotFoundException<int>(nameof(teacherItem), "teacherItem", id);

            ValidateCenterAccess(teacherItem.CenterId, nameof(TeacherItem));
            teacherItem.IsActive = true;
            unitOfWork.TeacherItems.Update(teacherItem);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeactivateAsync(int id)
        {
            var teacherItem = await unitOfWork.TeacherItems.GetByIdAsync(id);
            if (teacherItem == null)
                throw new NotFoundException<int>(nameof(teacherItem), "teacherItem", id);

            ValidateCenterAccess(teacherItem.CenterId, nameof(TeacherItem));
            teacherItem.IsActive = false;
            unitOfWork.TeacherItems.Update(teacherItem);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> IsTeacherAssignedToItemAsync(int teacherId, int itemId)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var teacherItems = await unitOfWork.TeacherItems.GetAllAsync(
                ti => ti.CenterId == currentCenterId && ti.TeacherId == teacherId && ti.ItemId == itemId && !ti.IsDeleted,
                new PaginationParamter { PageSize = 1 });
            return teacherItems.Any();
        }
    }
}