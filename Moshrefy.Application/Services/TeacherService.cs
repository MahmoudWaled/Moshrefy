using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moshrefy.Application.DTOs.Teacher;
using Moshrefy.Application.Interfaces.IUnitOfWork;
using Moshrefy.Application.Interfaces.IServices;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Exceptions;
using Moshrefy.Domain.Identity;
using Moshrefy.Domain.Paramter;
using Microsoft.EntityFrameworkCore;

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

        public async Task<List<TeacherResponseDTO>> GetAllAsync(PaginationParameter paginationParamter)
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

        public async Task<List<TeacherResponseDTO>> GetActiveAsync(PaginationParameter paginationParamter)
        {
            var teachers = await unitOfWork.Teachers.GetActiveTeachersAsync();
            return mapper.Map<List<TeacherResponseDTO>>(teachers);
        }

        public async Task<List<TeacherResponseDTO>> GetInactiveAsync(PaginationParameter paginationParamter)
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
            
            unitOfWork.Teachers.Update(teacher);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var teacher = await unitOfWork.Teachers.GetByIdAsync(id);
            if (teacher == null)
                throw new NotFoundException<int>(nameof(teacher), "teacher", id);

            ValidateCenterAccess(teacher.CenterId, nameof(Teacher));

            unitOfWork.Teachers.SoftDelete(teacher);
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
            
            unitOfWork.Teachers.Update(teacher);
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
            
            unitOfWork.Teachers.Update(teacher);
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
            
            unitOfWork.Teachers.Update(teacher);
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
            
            unitOfWork.Teachers.Update(teacher);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task<int> GetTotalCountAsync()
        {
            var centerId = GetCurrentCenterIdOrThrow();
            return await unitOfWork.Teachers.CountAsync(t => t.CenterId == centerId && !t.IsDeleted);
        }

        public async Task<Moshrefy.Application.DTOs.Common.DataTableResponse<TeacherResponseDTO>> GetTeachersDataTableAsync(Moshrefy.Application.DTOs.Common.DataTableRequest request)
        {
            var centerId = GetCurrentCenterIdOrThrow();

            // 1. Initial Query
            var query = unitOfWork.Teachers.GetQueryable()
                .Include(t => t.TeacherCourses)
                .ThenInclude(tc => tc.Course)
                .Where(t => t.CenterId == centerId && !t.IsDeleted);

            // 2. Count Total
            var totalRecords = await unitOfWork.Teachers.CountAsync(t => t.CenterId == centerId && !t.IsDeleted);

            // 3. Apply Filters
            if (!string.IsNullOrEmpty(request.SearchValue))
            {
                var search = request.SearchValue.ToLower();
                query = query.Where(t =>
                    t.Name.ToLower().Contains(search) ||
                    t.Phone.ToLower().Contains(search) ||
                    t.Specialization.ToLower().Contains(search) ||
                    (t.Email != null && t.Email.ToLower().Contains(search))
                );
            }

            if (!string.IsNullOrEmpty(request.FilterStatus) && request.FilterStatus != "all")
            {
                bool isActive = request.FilterStatus == "Active"; // Case sensitive matching Frontend usually
                // Frontend sends "Active" or "Inactive" (capitalized?) or "active"/"inactive"
                // TeacherController lines 98-102: if (filterStatus == "Active")
                // So I should use "Active"
                if (request.FilterStatus.Equals("active", StringComparison.OrdinalIgnoreCase)) isActive = true;
                else if (request.FilterStatus.Equals("inactive", StringComparison.OrdinalIgnoreCase)) isActive = false;
                
                query = query.Where(t => t.IsActive == isActive);
            }

            // Count Filtered
            var filteredRecords = await query.CountAsync();

            // 4. Sorting
            if (!string.IsNullOrEmpty(request.SortColumnName) && !string.IsNullOrEmpty(request.SortDirection))
            {
                bool isAsc = request.SortDirection.ToLower() == "asc";
                query = request.SortColumnName.ToLower() switch
                {
                    "name" => isAsc ? query.OrderBy(t => t.Name) : query.OrderByDescending(t => t.Name),
                    "phone" => isAsc ? query.OrderBy(t => t.Phone) : query.OrderByDescending(t => t.Phone),
                    "specialization" => isAsc ? query.OrderBy(t => t.Specialization) : query.OrderByDescending(t => t.Specialization),
                    "isactive" => isAsc ? query.OrderBy(t => t.IsActive) : query.OrderByDescending(t => t.IsActive),
                    _ => query.OrderBy(t => t.Name)
                };
            }
            else
            {
                query = query.OrderByDescending(t => t.CreatedAt);
            }

            // 5. Pagination
            if (request.Length > 0)
            {
                query = query.Skip(request.Start).Take(request.Length);
            }

            // 6. Execute
            var teachers = await query.ToListAsync();
            var data = mapper.Map<List<TeacherResponseDTO>>(teachers);

            // 7. Populate CourseCount
            // We iterate over the *Entities* in 'teachers' which have inclusions, and match with DTOs
            // Or just iterate parallel since order is preserved
            for (int i = 0; i < teachers.Count; i++)
            {
                // Count active assignments where course is not deleted
                // TeacherCourse is soft deletable?
                // Teacher.cs: ICollection<TeacherCourse> TeacherCourses
                // TeacherCourse.cs (assumed): IsDeleted, Course (IsDeleted)
                data[i].CourseCount = teachers[i].TeacherCourses
                    .Count(tc => !tc.IsDeleted && (tc.Course == null || !tc.Course.IsDeleted));
            }

            return new Moshrefy.Application.DTOs.Common.DataTableResponse<TeacherResponseDTO>
            {
                Draw = request.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = filteredRecords,
                Data = data
            };
        }
    }
}
