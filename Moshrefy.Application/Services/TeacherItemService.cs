using AutoMapper;
using Moshrefy.Application.DTOs.TeacherItem;
using Moshrefy.Application.Interfaces.IUnitOfWork;
using Moshrefy.Application.Interfaces.IServices;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Exceptions;
using Moshrefy.Domain.Paramter;

namespace Moshrefy.Application.Services
{
    public class TeacherItemService(IUnitOfWork unitOfWork, IMapper mapper) : ITeacherItemService
    {
        public async Task<TeacherItemResponseDTO> CreateAsync(CreateTeacherItemDTO createTeacherItemDTO)
        {
            // Check if already exists
            var existing = await unitOfWork.TeacherItems.GetAllAsync(new PaginationParamter());
            if (existing.Any(ti => ti.TeacherId == createTeacherItemDTO.TeacherId && ti.ItemId == createTeacherItemDTO.ItemId))
            {
                throw new BadRequestException("Teacher is already assigned to this item.");
            }

            var teacherItem = mapper.Map<TeacherItem>(createTeacherItemDTO);
            await unitOfWork.TeacherItems.AddAsync(teacherItem);
            await unitOfWork.SaveChangesAsync();
            return mapper.Map<TeacherItemResponseDTO>(teacherItem);
        }

        public async Task<TeacherItemResponseDTO?> GetByIdAsync(int id)
        {
            var teacherItem = await unitOfWork.TeacherItems.GetByIdAsync(id);
            if (teacherItem == null)
                throw new NotFoundException<int>(nameof(teacherItem), "teacherItem", id);

            return mapper.Map<TeacherItemResponseDTO>(teacherItem);
        }

        public async Task<List<TeacherItemResponseDTO>> GetAllAsync(PaginationParamter paginationParamter)
        {
            var teacherItems = await unitOfWork.TeacherItems.GetAllAsync(paginationParamter);
            return mapper.Map<List<TeacherItemResponseDTO>>(teacherItems);
        }

        public async Task<List<TeacherItemResponseDTO>> GetByTeacherIdAsync(int teacherId)
        {
            var teacherItems = await unitOfWork.TeacherItems.GetByTeacherIdAsync(teacherId);
            return mapper.Map<List<TeacherItemResponseDTO>>(teacherItems);
        }

        public async Task<List<TeacherItemResponseDTO>> GetByItemIdAsync(int itemId)
        {
            var teacherItems = await unitOfWork.TeacherItems.GetAllAsync(new PaginationParamter());
            var filtered = teacherItems.Where(ti => ti.ItemId == itemId).ToList();
            return mapper.Map<List<TeacherItemResponseDTO>>(filtered);
        }

        public async Task<List<TeacherItemResponseDTO>> GetActiveAsync(PaginationParamter paginationParamter)
        {
            var teacherItems = await unitOfWork.TeacherItems.GetAllAsync(paginationParamter);
            var active = teacherItems.Where(ti => ti.IsActive).ToList();
            return mapper.Map<List<TeacherItemResponseDTO>>(active);
        }

        public async Task<List<TeacherItemResponseDTO>> GetInactiveAsync(PaginationParamter paginationParamter)
        {
            var teacherItems = await unitOfWork.TeacherItems.GetAllAsync(paginationParamter);
            var inactive = teacherItems.Where(ti => !ti.IsActive).ToList();
            return mapper.Map<List<TeacherItemResponseDTO>>(inactive);
        }

        public async Task UpdateAsync(int id, UpdateTeacherItemDTO updateTeacherItemDTO)
        {
            var teacherItem = await unitOfWork.TeacherItems.GetByIdAsync(id);
            if (teacherItem == null)
                throw new NotFoundException<int>(nameof(teacherItem), "teacherItem", id);

            mapper.Map(updateTeacherItemDTO, teacherItem);
            unitOfWork.TeacherItems.UpdateAsync(teacherItem);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var teacherItem = await unitOfWork.TeacherItems.GetByIdAsync(id);
            if (teacherItem == null)
                throw new NotFoundException<int>(nameof(teacherItem), "teacherItem", id);

            unitOfWork.TeacherItems.DeleteAsync(teacherItem);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int id)
        {
            var teacherItem = await unitOfWork.TeacherItems.GetByIdAsync(id);
            if (teacherItem == null)
                throw new NotFoundException<int>(nameof(teacherItem), "teacherItem", id);

            teacherItem.IsDeleted = true;
            unitOfWork.TeacherItems.UpdateAsync(teacherItem);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task RestoreAsync(int id)
        {
            var teacherItem = await unitOfWork.TeacherItems.GetByIdAsync(id);
            if (teacherItem == null)
                throw new NotFoundException<int>(nameof(teacherItem), "teacherItem", id);

            teacherItem.IsDeleted = false;
            unitOfWork.TeacherItems.UpdateAsync(teacherItem);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task ActivateAsync(int id)
        {
            var teacherItem = await unitOfWork.TeacherItems.GetByIdAsync(id);
            if (teacherItem == null)
                throw new NotFoundException<int>(nameof(teacherItem), "teacherItem", id);

            teacherItem.IsActive = true;
            unitOfWork.TeacherItems.UpdateAsync(teacherItem);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeactivateAsync(int id)
        {
            var teacherItem = await unitOfWork.TeacherItems.GetByIdAsync(id);
            if (teacherItem == null)
                throw new NotFoundException<int>(nameof(teacherItem), "teacherItem", id);

            teacherItem.IsActive = false;
            unitOfWork.TeacherItems.UpdateAsync(teacherItem);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> IsTeacherAssignedToItemAsync(int teacherId, int itemId)
        {
            var teacherItems = await unitOfWork.TeacherItems.GetAllAsync(new PaginationParamter());
            return teacherItems.Any(ti => ti.TeacherId == teacherId && ti.ItemId == itemId && !ti.IsDeleted);
        }
    }
}