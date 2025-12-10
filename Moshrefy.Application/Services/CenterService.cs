using AutoMapper;
using Moshrefy.Application.DTOs.Center;
using Moshrefy.Application.Interfaces.IRepositories;
using Moshrefy.Application.Interfaces.IServices;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Exceptions;
using Moshrefy.Domain.Paramter;

namespace Moshrefy.Application.Services
{
    public class CenterService(ICenterRepository centerRepository, IMapper mapper) : ICenterService
    {
        public async Task<CenterResponseDTO> CreateAsync(CreateCenterDTO createCenterDTO)
        {
            var center = mapper.Map<Center>(createCenterDTO);
            await centerRepository.AddAsync(center);
            await centerRepository.SaveChangesAsync();
            return mapper.Map<CenterResponseDTO>(center);
        }

        public async Task<CenterResponseDTO?> GetByIdAsync(int id)
        {
            var center = await centerRepository.GetByIdAsync(id);
            if (center == null)
                throw new NotFoundException<int>(nameof(center), "center", id);

            return mapper.Map<CenterResponseDTO>(center);
        }

        public async Task<List<CenterResponseDTO>> GetAllAsync(PaginationParamter paginationParamter)
        {
            var centers = await centerRepository.GetAllAsync(paginationParamter);
            return mapper.Map<List<CenterResponseDTO>>(centers);
        }

        public async Task<List<CenterResponseDTO>> GetByNameAsync(string name)
        {
            var centers = await centerRepository.GetByName(name);
            return mapper.Map<List<CenterResponseDTO>>(centers);
        }

        public async Task<List<CenterResponseDTO>> GetActiveAsync(PaginationParamter paginationParamter)
        {
            var centers = await centerRepository.GetAllAsync(paginationParamter);
            var activeCenters = centers.Where(c => c.IsActive).ToList();
            return mapper.Map<List<CenterResponseDTO>>(activeCenters);
        }

        public async Task<List<CenterResponseDTO>> GetInactiveAsync(PaginationParamter paginationParamter)
        {
            var centers = await centerRepository.GetAllAsync(paginationParamter);
            var inactiveCenters = centers.Where(c => !c.IsActive).ToList();
            return mapper.Map<List<CenterResponseDTO>>(inactiveCenters);
        }

        public async Task UpdateAsync(int id, UpdateCenterDTO updateCenterDTO)
        {
            var center = await centerRepository.GetByIdAsync(id);
            if (center == null)
                throw new NotFoundException<int>(nameof(center), "center", id);

            mapper.Map(updateCenterDTO, center);
            centerRepository.UpdateAsync(center);
            await centerRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var center = await centerRepository.GetByIdAsync(id);
            if (center == null)
                throw new NotFoundException<int>(nameof(center), "center", id);


            centerRepository.DeleteAsync(center);
            await centerRepository.SaveChangesAsync();
        }

        public async Task ActivateAsync(int id)
        {
            var center = await centerRepository.GetByIdAsync(id);
            if (center == null)
                throw new NotFoundException<int>(nameof(center), "center", id);

            center.IsActive = true;
            centerRepository.UpdateAsync(center);
            await centerRepository.SaveChangesAsync();
        }

        public async Task DeactivateAsync(int id)
        {
            var center = await centerRepository.GetByIdAsync(id);
            if (center == null)
                throw new NotFoundException<int>(nameof(center), "center", id);

            center.IsActive = false;
            centerRepository.UpdateAsync(center);
            await centerRepository.SaveChangesAsync();
        }

        public async Task SoftDeleteCenterAsync(int id)
        {
            var center = await centerRepository.GetByIdAsync(id);
            if (center == null)
                throw new NotFoundException<int>(nameof(center), "center", id);

            center.IsDeleted = true;
            centerRepository.UpdateAsync(center);
            await centerRepository.SaveChangesAsync();
        }

        public async Task RestoreCenterAsync(int id)
        {
            var center = await centerRepository.GetByIdAsync(id);
            if (center == null)
                throw new NotFoundException<int>(nameof(center), "center", id);

            center.IsDeleted = false;
            centerRepository.UpdateAsync(center);
            await centerRepository.SaveChangesAsync();
        }


    }
}