using AutoMapper;
using Moshrefy.Application.DTOs.Classroom;
using Moshrefy.Application.Interfaces.IUnitOfWork;
using Moshrefy.Application.Interfaces.IServices;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Exceptions;
using Moshrefy.Domain.Paramter;

namespace Moshrefy.Application.Services
{
    public class ClassroomService(
        IUnitOfWork unitOfWork, 
        IMapper mapper,
        ITenantContext tenantContext
    ) : BaseService(tenantContext), IClassroomService
    {
        public async Task<ClassroomResponseDTO> CreateAsync(CreateClassroomDTO createClassroomDTO)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var classroom = mapper.Map<Classroom>(createClassroomDTO);
            classroom.CenterId = currentCenterId;
            await unitOfWork.Classrooms.AddAsync(classroom);
            await unitOfWork.SaveChangesAsync();
            return mapper.Map<ClassroomResponseDTO>(classroom);
        }

        public async Task<ClassroomResponseDTO?> GetByIdAsync(int id)
        {
            var classroom = await unitOfWork.Classrooms.GetByIdAsync(id);
            if (classroom == null)
                throw new NotFoundException<int>(nameof(classroom), "classroom", id);

            ValidateCenterAccess(classroom.CenterId, nameof(Classroom));
            return mapper.Map<ClassroomResponseDTO>(classroom);
        }

        public async Task<List<ClassroomResponseDTO>> GetAllAsync(PaginationParamter paginationParamter)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var classrooms = await unitOfWork.Classrooms.GetAllAsync(
                c => c.CenterId == currentCenterId && !c.IsDeleted,
                paginationParamter);
            return mapper.Map<List<ClassroomResponseDTO>>(classrooms.ToList());
        }

        public async Task<List<ClassroomResponseDTO>> GetByNameAsync(string name)
        {
            var classrooms = await unitOfWork.Classrooms.GetByName(name);
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var filtered = classrooms.Where(c => c.CenterId == currentCenterId && !c.IsDeleted).ToList();
            return mapper.Map<List<ClassroomResponseDTO>>(filtered);
        }

        public async Task<List<ClassroomResponseDTO>> GetActiveAsync(PaginationParamter paginationParamter)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var classrooms = await unitOfWork.Classrooms.GetAllAsync(
                c => c.CenterId == currentCenterId && c.IsActive && !c.IsDeleted,
                paginationParamter);
            return mapper.Map<List<ClassroomResponseDTO>>(classrooms.ToList());
        }

        public async Task<List<ClassroomResponseDTO>> GetInactiveAsync(PaginationParamter paginationParamter)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var classrooms = await unitOfWork.Classrooms.GetAllAsync(
                c => c.CenterId == currentCenterId && !c.IsActive && !c.IsDeleted,
                paginationParamter);
            return mapper.Map<List<ClassroomResponseDTO>>(classrooms.ToList());
        }

        public async Task UpdateAsync(int id, UpdateClassroomDTO updateClassroomDTO)
        {
            var classroom = await unitOfWork.Classrooms.GetByIdAsync(id);
            if (classroom == null)
                throw new NotFoundException<int>(nameof(classroom), "classroom", id);

            ValidateCenterAccess(classroom.CenterId, nameof(Classroom));
            mapper.Map(updateClassroomDTO, classroom);
            unitOfWork.Classrooms.UpdateAsync(classroom);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var classroom = await unitOfWork.Classrooms.GetByIdAsync(id);
            if (classroom == null)
                throw new NotFoundException<int>(nameof(classroom), "classroom", id);

            ValidateCenterAccess(classroom.CenterId, nameof(Classroom));
            unitOfWork.Classrooms.DeleteAsync(classroom);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task ActivateAsync(int id)
        {
            var classroom = await unitOfWork.Classrooms.GetByIdAsync(id);
            if (classroom == null)
                throw new NotFoundException<int>(nameof(classroom), "classroom", id);

            ValidateCenterAccess(classroom.CenterId, nameof(Classroom));
            classroom.IsActive = true;
            unitOfWork.Classrooms.UpdateAsync(classroom);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeactivateAsync(int id)
        {
            var classroom = await unitOfWork.Classrooms.GetByIdAsync(id);
            if (classroom == null)
                throw new NotFoundException<int>(nameof(classroom), "classroom", id);

            ValidateCenterAccess(classroom.CenterId, nameof(Classroom));
            classroom.IsActive = false;
            unitOfWork.Classrooms.UpdateAsync(classroom);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int id)
        {
            var classroom = await unitOfWork.Classrooms.GetByIdAsync(id);
            if (classroom == null)
                throw new NotFoundException<int>(nameof(classroom), "classroom", id);

            ValidateCenterAccess(classroom.CenterId, nameof(Classroom));
            classroom.IsDeleted = true;
            unitOfWork.Classrooms.UpdateAsync(classroom);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task RestoreAsync(int id)
        {
            var classroom = await unitOfWork.Classrooms.GetByIdAsync(id);
            if (classroom == null)
                throw new NotFoundException<int>(nameof(classroom), "classroom", id);

            ValidateCenterAccess(classroom.CenterId, nameof(Classroom));
            classroom.IsDeleted = false;
            unitOfWork.Classrooms.UpdateAsync(classroom);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
