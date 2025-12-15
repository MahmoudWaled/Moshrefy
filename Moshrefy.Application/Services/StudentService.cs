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
            var students = await unitOfWork.Students.GetAllAsync(paginationParamter);
            return mapper.Map<List<StudentResponseDTO>>(students);
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
    }
}