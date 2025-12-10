using AutoMapper;
using Moshrefy.Application.DTOs.Classroom;
using Moshrefy.Application.Interfaces.IUnitOfWork;
using Moshrefy.Application.Interfaces.IServices;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Exceptions;
using Moshrefy.Domain.Paramter;

namespace Moshrefy.Application.Services
{
    public class ClassroomService(IUnitOfWork unitOfWork, IMapper mapper) : IClassroomService
    {
        public async Task<ClassroomResponseDTO> CreateAsync(CreateClassroomDTO createClassroomDTO)
        {
            var classroom = mapper.Map<Classroom>(createClassroomDTO);
            await unitOfWork.Classrooms.AddAsync(classroom);
            await unitOfWork.SaveChangesAsync();
            return mapper.Map<ClassroomResponseDTO>(classroom);
        }

        public async Task<ClassroomResponseDTO?> GetByIdAsync(int id)
        {
            var classroom = await unitOfWork.Classrooms.GetByIdAsync(id);
            if (classroom == null)
                throw new NotFoundException<int>(nameof(classroom), "classroom", id);

            return mapper.Map<ClassroomResponseDTO>(classroom);
        }

        public async Task<List<ClassroomResponseDTO>> GetAllAsync(PaginationParamter paginationParamter)
        {
            var classrooms = await unitOfWork.Classrooms.GetAllAsync(paginationParamter);
            return mapper.Map<List<ClassroomResponseDTO>>(classrooms);
        }

        public async Task<List<ClassroomResponseDTO>> GetByNameAsync(string name)
        {
            var classrooms = await unitOfWork.Classrooms.GetByName(name);
            return mapper.Map<List<ClassroomResponseDTO>>(classrooms);
        }

        public async Task<List<ClassroomResponseDTO>> GetActiveAsync(PaginationParamter paginationParamter)
        {
            var classrooms = await unitOfWork.Classrooms.GetAllAsync(paginationParamter);
            var activeClassrooms = classrooms.Where(c => c.IsActive).ToList();
            return mapper.Map<List<ClassroomResponseDTO>>(activeClassrooms);
        }

        public async Task<List<ClassroomResponseDTO>> GetInactiveAsync(PaginationParamter paginationParamter)
        {
            var classrooms = await unitOfWork.Classrooms.GetAllAsync(paginationParamter);
            var inactiveClassrooms = classrooms.Where(c => !c.IsActive).ToList();
            return mapper.Map<List<ClassroomResponseDTO>>(inactiveClassrooms);
        }

        public async Task UpdateAsync(int id, UpdateClassroomDTO updateClassroomDTO)
        {
            var classroom = await unitOfWork.Classrooms.GetByIdAsync(id);
            if (classroom == null)
                throw new NotFoundException<int>(nameof(classroom), "classroom", id);

            mapper.Map(updateClassroomDTO, classroom);
            unitOfWork.Classrooms.UpdateAsync(classroom);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var classroom = await unitOfWork.Classrooms.GetByIdAsync(id);
            if (classroom == null)
                throw new NotFoundException<int>(nameof(classroom), "classroom", id);

            unitOfWork.Classrooms.DeleteAsync(classroom);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task ActivateAsync(int id)
        {
            var classroom = await unitOfWork.Classrooms.GetByIdAsync(id);
            if (classroom == null)
                throw new NotFoundException<int>(nameof(classroom), "classroom", id);

            classroom.IsActive = true;
            unitOfWork.Classrooms.UpdateAsync(classroom);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeactivateAsync(int id)
        {
            var classroom = await unitOfWork.Classrooms.GetByIdAsync(id);
            if (classroom == null)
                throw new NotFoundException<int>(nameof(classroom), "classroom", id);

            classroom.IsActive = false;
            unitOfWork.Classrooms.UpdateAsync(classroom);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int id)
        {
            var classroom = await unitOfWork.Classrooms.GetByIdAsync(id);
            if (classroom == null)
                throw new NotFoundException<int>(nameof(classroom), "classroom", id);

            classroom.IsDeleted = true;
            unitOfWork.Classrooms.UpdateAsync(classroom);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task RestoreAsync(int id)
        {
            var classroom = await unitOfWork.Classrooms.GetByIdAsync(id);
            if (classroom == null)
                throw new NotFoundException<int>(nameof(classroom), "classroom", id);

            classroom.IsDeleted = false;
            unitOfWork.Classrooms.UpdateAsync(classroom);
            await unitOfWork.SaveChangesAsync();
        }
    }
}