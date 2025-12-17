using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moshrefy.Application.DTOs.Student;
using Moshrefy.Application.Interfaces.IUnitOfWork;
using Moshrefy.Application.Interfaces.IServices;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Enums;
using Moshrefy.Domain.Exceptions;
using Moshrefy.Domain.Identity;
using Moshrefy.Domain.Paramter;
using Microsoft.EntityFrameworkCore;

namespace Moshrefy.Application.Services
{
    public class StudentService(
        IUnitOfWork unitOfWork, 
        IMapper mapper,
        ITenantContext tenantContext,
        UserManager<ApplicationUser> userManager
        ) : BaseService(tenantContext), IStudentService
    {
        public async Task<StudentResponseDTO> CreateAsync(CreateStudentDTO createStudentDTO)
        {
            var existingStudent = await unitOfWork.Students.GetByPhoneNumberAsync(createStudentDTO.FirstPhone);
            if (existingStudent != null)
            {
                throw new BadRequestException("A student with this phone number already exists.");
            }

            var currentCenterId = GetCurrentCenterIdOrThrow();
            var student = mapper.Map<Student>(createStudentDTO);
            student.StudentStatus = StudentStatus.Active;
            
            // Set audit fields
            var currentUser = await userManager.FindByIdAsync(tenantContext.GetCurrentUserId());
            student.CenterId = currentCenterId;
            student.CreatedById = currentUser!.Id;
            student.CreatedByName = currentUser!.UserName ?? string.Empty;

            await unitOfWork.Students.AddAsync(student);
            await unitOfWork.SaveChangesAsync();

            return mapper.Map<StudentResponseDTO>(student);
        }


        public async Task<StudentResponseDTO?> GetByIdAsync(int id)
        {
            var student = await unitOfWork.Students.GetByIdAsync(id);
            if (student == null)
                throw new NotFoundException<int>(nameof(student), "student", id);

            return mapper.Map<StudentResponseDTO>(student);
        }

        public async Task<List<StudentResponseDTO>> GetAllAsync(PaginationParamter paginationParamter)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            
            // Filter at database level BEFORE pagination
            var students = await unitOfWork.Students.GetAllAsync(
                s => s.CenterId == currentCenterId && !s.IsDeleted,
                paginationParamter);
            
            return mapper.Map<List<StudentResponseDTO>>(students.ToList());
        }


        public async Task<List<StudentResponseDTO>> GetByNameAsync(string name)
        {
            var students = await unitOfWork.Students.GetByNameAsync(name);
            return mapper.Map<List<StudentResponseDTO>>(students);
        }

        public async Task<StudentResponseDTO?> GetByPhoneNumberAsync(string phoneNumber)
        {
            var student = await unitOfWork.Students.GetByPhoneNumberAsync(phoneNumber);
            if (student == null)
                throw new NotFoundException<string>(nameof(student), "student", phoneNumber);

            return mapper.Map<StudentResponseDTO>(student);
        }

        public async Task<List<StudentResponseDTO>> GetByStatusAsync(StudentStatus status)
        {
            var students = await unitOfWork.Students.GetByStatusAsync(status);
            return mapper.Map<List<StudentResponseDTO>>(students);
        }

        public async Task<List<StudentResponseDTO>> GetActiveStudentsAsync(PaginationParamter paginationParamter)
        {
            var students = await unitOfWork.Students.GetByStatusAsync(StudentStatus.Active);
            return mapper.Map<List<StudentResponseDTO>>(students);
        }

        public async Task<List<StudentResponseDTO>> GetInactiveStudentsAsync(PaginationParamter paginationParamter)
        {
            var students = await unitOfWork.Students.GetByStatusAsync(StudentStatus.Inactive);
            return mapper.Map<List<StudentResponseDTO>>(students);
        }

        public async Task<List<StudentResponseDTO>> GetSuspendedStudentsAsync(PaginationParamter paginationParamter)
        {
            var students = await unitOfWork.Students.GetByStatusAsync(StudentStatus.Suspended);
            return mapper.Map<List<StudentResponseDTO>>(students);
        }

        public async Task UpdateAsync(int id, UpdateStudentDTO updateStudentDTO)
        {
            var student = await unitOfWork.Students.GetByIdAsync(id);
            if (student == null)
                throw new NotFoundException<int>(nameof(student), "student", id);

            ValidateCenterAccess(student.CenterId, nameof(Student));

            mapper.Map(updateStudentDTO, student);
            
            // Set audit fields
            var currentUser = await userManager.FindByIdAsync(tenantContext.GetCurrentUserId());
            student.ModifiedById = currentUser!.Id;
            student.ModifiedByName = currentUser!.UserName ?? string.Empty;
            student.ModifiedAt = DateTimeOffset.UtcNow;
            
            unitOfWork.Students.UpdateAsync(student);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var student = await unitOfWork.Students.GetByIdAsync(id);
            if (student == null)
                throw new NotFoundException<int>(nameof(student), "student", id);

            ValidateCenterAccess(student.CenterId, nameof(Student));

            unitOfWork.Students.DeleteAsync(student);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int id)
        {
            var student = await unitOfWork.Students.GetByIdAsync(id);
            if (student == null)
                throw new NotFoundException<int>(nameof(student), "student", id);

            ValidateCenterAccess(student.CenterId, nameof(Student));

            student.IsDeleted = true;
            
            // Set audit fields
            var currentUser = await userManager.FindByIdAsync(tenantContext.GetCurrentUserId());
            student.ModifiedById = currentUser!.Id;
            student.ModifiedByName = currentUser!.UserName ?? string.Empty;
            student.ModifiedAt = DateTimeOffset.UtcNow;
            
            unitOfWork.Students.UpdateAsync(student);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task RestoreAsync(int id)
        {
            var student = await unitOfWork.Students.GetByIdAsync(id);
            if (student == null)
                throw new NotFoundException<int>(nameof(student), "student", id);

            ValidateCenterAccess(student.CenterId, nameof(Student));

            student.IsDeleted = false;
            
            // Set audit fields
            var currentUser = await userManager.FindByIdAsync(tenantContext.GetCurrentUserId());
            student.ModifiedById = currentUser!.Id;
            student.ModifiedByName = currentUser!.UserName ?? string.Empty;
            student.ModifiedAt = DateTimeOffset.UtcNow;
            
            unitOfWork.Students.UpdateAsync(student);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task ActivateAsync(int id)
        {
            var student = await unitOfWork.Students.GetByIdAsync(id);
            if (student == null)
                throw new NotFoundException<int>(nameof(student), "student", id);

            ValidateCenterAccess(student.CenterId, nameof(Student));

            student.StudentStatus = StudentStatus.Active;
            
            // Set audit fields
            var currentUser = await userManager.FindByIdAsync(tenantContext.GetCurrentUserId());
            student.ModifiedById = currentUser!.Id;
            student.ModifiedByName = currentUser!.UserName ?? string.Empty;
            student.ModifiedAt = DateTimeOffset.UtcNow;
            
            unitOfWork.Students.UpdateAsync(student);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeactivateAsync(int id)
        {
            var student = await unitOfWork.Students.GetByIdAsync(id);
            if (student == null)
                throw new NotFoundException<int>(nameof(student), "student", id);

            ValidateCenterAccess(student.CenterId, nameof(Student));

            student.StudentStatus = StudentStatus.Inactive;
            
            // Set audit fields
            var currentUser = await userManager.FindByIdAsync(tenantContext.GetCurrentUserId());
            student.ModifiedById = currentUser!.Id;
            student.ModifiedByName = currentUser!.UserName ?? string.Empty;
            student.ModifiedAt = DateTimeOffset.UtcNow;
            
            unitOfWork.Students.UpdateAsync(student);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task SuspendAsync(int id)
        {
            var student = await unitOfWork.Students.GetByIdAsync(id);
            if (student == null)
                throw new NotFoundException<int>(nameof(student), "student", id);

            ValidateCenterAccess(student.CenterId, nameof(Student));

            student.StudentStatus = StudentStatus.Suspended;
            
            // Set audit fields
            var currentUser = await userManager.FindByIdAsync(tenantContext.GetCurrentUserId());
            student.ModifiedById = currentUser!.Id;
            student.ModifiedByName = currentUser!.UserName ?? string.Empty;
            student.ModifiedAt = DateTimeOffset.UtcNow;
            
            unitOfWork.Students.UpdateAsync(student);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task<int> GetTotalCountAsync()
        {
            var centerId = GetCurrentCenterIdOrThrow();
            return await unitOfWork.Students.CountAsync(s => s.CenterId == centerId && !s.IsDeleted);
        }

        public async Task<Moshrefy.Application.DTOs.Common.DataTableResponse<StudentResponseDTO>> GetStudentsDataTableAsync(Moshrefy.Application.DTOs.Common.DataTableRequest request)
        {
            var centerId = GetCurrentCenterIdOrThrow();

            // 1. Initial Query
            var query = unitOfWork.Students.GetQueryable()
                .Where(s => s.CenterId == centerId && !s.IsDeleted);

            // 2. Count Total
            var totalRecords = await unitOfWork.Students.CountAsync(s => s.CenterId == centerId && !s.IsDeleted);

            // 3. Apply Filters
            if (!string.IsNullOrEmpty(request.SearchValue))
            {
                var search = request.SearchValue.ToLower();
                query = query.Where(s =>
                    s.Name.ToLower().Contains(search) ||
                    s.FirstPhone.ToLower().Contains(search) ||
                    (s.NationalId != null && s.NationalId.ToLower().Contains(search))
                );
            }

            // Status Filter
            if (!string.IsNullOrEmpty(request.FilterStatus) && request.FilterStatus != "all")
            {
                if (Enum.TryParse<StudentStatus>(request.FilterStatus, true, out var status))
                {
                    query = query.Where(s => s.StudentStatus == status);
                }
            }

            // Count Filtered
            var filteredRecords = await query.CountAsync();

            // 4. Sorting
            if (!string.IsNullOrEmpty(request.SortColumnName) && !string.IsNullOrEmpty(request.SortDirection))
            {
                bool isAsc = request.SortDirection.ToLower() == "asc";
                query = request.SortColumnName.ToLower() switch
                {
                    "name" => isAsc ? query.OrderBy(s => s.Name) : query.OrderByDescending(s => s.Name),
                    "firstphone" => isAsc ? query.OrderBy(s => s.FirstPhone) : query.OrderByDescending(s => s.FirstPhone),
                    // Age: Asc Age = Desc DOB
                    "age" => isAsc ? query.OrderByDescending(s => s.DateOfBirth) : query.OrderBy(s => s.DateOfBirth),
                    // Status
                    "studentstatus" => isAsc ? query.OrderBy(s => s.StudentStatus) : query.OrderByDescending(s => s.StudentStatus),
                    _ => query.OrderBy(s => s.Name)
                };
            }
            else
            {
                query = query.OrderByDescending(s => s.CreatedAt);
            }

            // 5. Pagination
            if (request.Length > 0)
            {
                query = query.Skip(request.Start).Take(request.Length);
            }

            // 6. Execute
            var students = await query.ToListAsync();
            var data = mapper.Map<List<StudentResponseDTO>>(students);

            return new Moshrefy.Application.DTOs.Common.DataTableResponse<StudentResponseDTO>
            {
                Draw = request.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = filteredRecords,
                Data = data
            };
        }
    }
}
