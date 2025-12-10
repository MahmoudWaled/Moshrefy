using AutoMapper;
using Moshrefy.Application.DTOs.Teacher;
using Moshrefy.Application.Interfaces.IUnitOfWork;
using Moshrefy.Application.Interfaces.IServices;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Exceptions;
using Moshrefy.Domain.Paramter;

namespace Moshrefy.Application.Services
{
    public class TeacherService(IUnitOfWork unitOfWork, IMapper mapper) : ITeacherService
    {
        public async Task<TeacherResponseDTO> CreateAsync(CreateTeacherDTO createTeacherDTO)
        {
            var teacher = mapper.Map<Teacher>(createTeacherDTO);
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
            var teachers = await unitOfWork.Teachers.GetAllAsync(paginationParamter);
            return mapper.Map<List<TeacherResponseDTO>>(teachers);
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

            mapper.Map(updateTeacherDTO, teacher);
            unitOfWork.Teachers.UpdateAsync(teacher);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var teacher = await unitOfWork.Teachers.GetByIdAsync(id);
            if (teacher == null)
                throw new NotFoundException<int>(nameof(teacher), "teacher", id);

            unitOfWork.Teachers.DeleteAsync(teacher);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task ActivateAsync(int id)
        {
            var teacher = await unitOfWork.Teachers.GetByIdAsync(id);
            if (teacher == null)
                throw new NotFoundException<int>(nameof(teacher), "teacher", id);

            teacher.IsActive = true;
            unitOfWork.Teachers.UpdateAsync(teacher);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeactivateAsync(int id)
        {
            var teacher = await unitOfWork.Teachers.GetByIdAsync(id);
            if (teacher == null)
                throw new NotFoundException<int>(nameof(teacher), "teacher", id);

            teacher.IsActive = false;
            unitOfWork.Teachers.UpdateAsync(teacher);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int id)
        {
            var teacher = await unitOfWork.Teachers.GetByIdAsync(id);
            if (teacher == null)
                throw new NotFoundException<int>(nameof(teacher), "teacher", id);

            teacher.IsDeleted = true;
            unitOfWork.Teachers.UpdateAsync(teacher);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task RestoreAsync(int id)
        {
            var teacher = await unitOfWork.Teachers.GetByIdAsync(id);
            if (teacher == null)
                throw new NotFoundException<int>(nameof(teacher), "teacher", id);

            teacher.IsDeleted = false;
            unitOfWork.Teachers.UpdateAsync(teacher);
            await unitOfWork.SaveChangesAsync();
        }
    }
}