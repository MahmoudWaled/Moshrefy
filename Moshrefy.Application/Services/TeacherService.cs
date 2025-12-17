using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moshrefy.Application.DTOs.Teacher;
using Moshrefy.Application.Interfaces.IUnitOfWork;
using Moshrefy.Application.Interfaces.IServices;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Exceptions;
using Moshrefy.Domain.Identity;
using Moshrefy.Domain.Paramter;

namespace Moshrefy.Application.Services
{
    public class TeacherService(
        IUnitOfWork unitOfWork, 
        IMapper mapper,
        ITenantContext tenantContext,
        UserManager<ApplicationUser> userManager) : BaseService(tenantContext), ITeacherService
    {
        public async Task<TeacherResponseDTO> CreateAsync(CreateTeacherDTO createTeacherDTO)
        {
            var teacher = mapper.Map<Teacher>(createTeacherDTO);
            
            // Set audit fields
            teacher.CenterId = tenantContext.GetCurrentCenterId() ?? 0;
            var currentUser = await userManager.FindByIdAsync(tenantContext.GetCurrentUserId());
            teacher.CreatedById = currentUser!.Id;
            teacher.CreatedByName = currentUser!.UserName ?? string.Empty;
            
            await unitOfWork.Teachers.AddAsync(teacher);
            await unitOfWork.SaveChangesAsync();
            return mapper.Map<TeacherResponseDTO>(teacher);
        }

        public async Task<TeacherResponseDTO?> GetByIdAsync(int id)
        {
            var teacher = await unitOfWork.Teachers.GetByIdAsync(id);
            if (teacher == null)
                throw new NotFoundException<int>(nameof(teacher), "teacher", id);

            return mapper.Map<TeacherResponseDTO>(teacher);
        }

        public async Task<List<TeacherResponseDTO>> GetAllAsync(PaginationParamter paginationParamter)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            
            // Filter at database level BEFORE pagination
            var teachers = await unitOfWork.Teachers.GetAllAsync(
                t => t.CenterId == currentCenterId && !t.IsDeleted,
                paginationParamter);
            
            return mapper.Map<List<TeacherResponseDTO>>(teachers.ToList());
        }


        public async Task<List<TeacherResponseDTO>> GetByNameAsync(string name)
        {
            var teachers = await unitOfWork.Teachers.GetByNameAsync(name);
            return mapper.Map<List<TeacherResponseDTO>>(teachers);
        }

        public async Task<List<TeacherResponseDTO>> GetActiveAsync(PaginationParamter paginationParamter)
        {
            var teachers = await unitOfWork.Teachers.GetActiveTeachersAsync();
            return mapper.Map<List<TeacherResponseDTO>>(teachers);
        }

        public async Task<List<TeacherResponseDTO>> GetInactiveAsync(PaginationParamter paginationParamter)
        {
            var teachers = await unitOfWork.Teachers.GetAllAsync(paginationParamter);
            var inactiveTeachers = teachers.Where(t => !t.IsActive).ToList();
            return mapper.Map<List<TeacherResponseDTO>>(inactiveTeachers);
        }

        public async Task<TeacherResponseDTO?> GetByPhoneNumberAsync(string phoneNumber)
        {
            var teacher = await unitOfWork.Teachers.GetByPhoneNumberAsync(phoneNumber);
            if (teacher == null)
                throw new NotFoundException<string>(nameof(teacher), "teacher", phoneNumber);

            return mapper.Map<TeacherResponseDTO>(teacher);
        }

        public async Task UpdateAsync(int id, UpdateTeacherDTO updateTeacherDTO)
        {
            var teacher = await unitOfWork.Teachers.GetByIdAsync(id);
            if (teacher == null)
                throw new NotFoundException<int>(nameof(teacher), "teacher", id);

            ValidateCenterAccess(teacher.CenterId, nameof(Teacher));

            mapper.Map(updateTeacherDTO, teacher);
            
            // Set audit fields
            var currentUser = await userManager.FindByIdAsync(tenantContext.GetCurrentUserId());
            teacher.ModifiedById = currentUser!.Id;
            teacher.ModifiedByName = currentUser!.UserName ?? string.Empty;
            teacher.ModifiedAt = DateTimeOffset.UtcNow;
            
            unitOfWork.Teachers.UpdateAsync(teacher);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var teacher = await unitOfWork.Teachers.GetByIdAsync(id);
            if (teacher == null)
                throw new NotFoundException<int>(nameof(teacher), "teacher", id);

            ValidateCenterAccess(teacher.CenterId, nameof(Teacher));

            unitOfWork.Teachers.DeleteAsync(teacher);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task ActivateAsync(int id)
        {
            var teacher = await unitOfWork.Teachers.GetByIdAsync(id);
            if (teacher == null)
                throw new NotFoundException<int>(nameof(teacher), "teacher", id);

            ValidateCenterAccess(teacher.CenterId, nameof(Teacher));

            teacher.IsActive = true;
            
            // Set audit fields
            var currentUser = await userManager.FindByIdAsync(tenantContext.GetCurrentUserId());
            teacher.ModifiedById = currentUser!.Id;
            teacher.ModifiedByName = currentUser!.UserName ?? string.Empty;
            teacher.ModifiedAt = DateTimeOffset.UtcNow;
            
            unitOfWork.Teachers.UpdateAsync(teacher);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeactivateAsync(int id)
        {
            var teacher = await unitOfWork.Teachers.GetByIdAsync(id);
            if (teacher == null)
                throw new NotFoundException<int>(nameof(teacher), "teacher", id);

            ValidateCenterAccess(teacher.CenterId, nameof(Teacher));

            teacher.IsActive = false;
            
            // Set audit fields
            var currentUser = await userManager.FindByIdAsync(tenantContext.GetCurrentUserId());
            teacher.ModifiedById = currentUser!.Id;
            teacher.ModifiedByName = currentUser!.UserName ?? string.Empty;
            teacher.ModifiedAt = DateTimeOffset.UtcNow;
            
            unitOfWork.Teachers.UpdateAsync(teacher);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int id)
        {
            var teacher = await unitOfWork.Teachers.GetByIdAsync(id);
            if (teacher == null)
                throw new NotFoundException<int>(nameof(teacher), "teacher", id);

            ValidateCenterAccess(teacher.CenterId, nameof(Teacher));

            teacher.IsDeleted = true;
            
            // Set audit fields
            var currentUser = await userManager.FindByIdAsync(tenantContext.GetCurrentUserId());
            teacher.ModifiedById = currentUser!.Id;
            teacher.ModifiedByName = currentUser!.UserName ?? string.Empty;
            teacher.ModifiedAt = DateTimeOffset.UtcNow;
            
            unitOfWork.Teachers.UpdateAsync(teacher);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task RestoreAsync(int id)
        {
            var teacher = await unitOfWork.Teachers.GetByIdAsync(id);
            if (teacher == null)
                throw new NotFoundException<int>(nameof(teacher), "teacher", id);

            ValidateCenterAccess(teacher.CenterId, nameof(Teacher));

            teacher.IsDeleted = false;
            
            // Set audit fields
            var currentUser = await userManager.FindByIdAsync(tenantContext.GetCurrentUserId());
            teacher.ModifiedById = currentUser!.Id;
            teacher.ModifiedByName = currentUser!.UserName ?? string.Empty;
            teacher.ModifiedAt = DateTimeOffset.UtcNow;
            
            unitOfWork.Teachers.UpdateAsync(teacher);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task<int> GetTotalCountAsync()
        {
            var centerId = GetCurrentCenterIdOrThrow();
            return await unitOfWork.Teachers.CountAsync(t => t.CenterId == centerId && !t.IsDeleted);
        }
    }
}
