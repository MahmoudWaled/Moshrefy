using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moshrefy.Application.DTOs.Center;
using Moshrefy.Application.DTOs.Common;
using Moshrefy.Application.Interfaces.IRepositories;
using Moshrefy.Application.Interfaces.IServices;
using Moshrefy.Application.Interfaces.IUnitOfWork;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Exceptions;
using Moshrefy.Domain.Identity;
using Moshrefy.Domain.Paramter;

namespace Moshrefy.Application.Services
{
    public class CenterService(
        IUnitOfWork unitOfWork ,
        UserManager<ApplicationUser> _userManager,
        IMapper mapper) : ICenterService
    {
        public async Task<CenterResponseDTO> CreateAsync(CreateCenterDTO createCenterDTO)
        {
            if (createCenterDTO == null)
                throw new BadRequestException("Create Center DTO cannot be null.");

            var center = mapper.Map<Center>(createCenterDTO);

            center.IsActive = true;
            center.IsDeleted = false;

            await unitOfWork.Centers.AddAsync(center);
            await unitOfWork.Centers.SaveChangesAsync();
            return mapper.Map<CenterResponseDTO>(center);
        }

        public async Task<CenterResponseDTO?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new BadRequestException("Invalid center id.");

            var center = await unitOfWork.Centers.GetByIdAsync(id);
            if (center == null)
                throw new NotFoundException<int>(nameof(center), "center", id);

            return mapper.Map<CenterResponseDTO>(center);
        }

        public async Task<CenterResponseDTO?> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new BadRequestException("Center name cannot be null or empty.");
            var center = await unitOfWork.Centers.GetByEmailAsync(email.ToLower());
            if (center == null)
                throw new NotFoundException<string>(nameof(center), "email", email);
            return mapper.Map<CenterResponseDTO?>(center);
        }

        public async Task<List<CenterResponseDTO>> GetByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new BadRequestException("Center name cannot be null or empty.");
            var centers = await unitOfWork.Centers.GetByNameAsync(name);
            if (centers == null || centers.Count() == 0)
                throw new NotFoundException<string>(nameof(Center), "name", name);
            return mapper.Map<List<CenterResponseDTO>>(centers);
        }

        public async Task<List<CenterResponseDTO>> GetAllAsync(PaginationParameter paginationParamter)
        {
            var centers = await unitOfWork.Centers.GetAllAsync(paginationParamter);
            return mapper.Map<List<CenterResponseDTO>>(centers);
        }

        public async Task<PaginatedResult<CenterResponseDTO>> GetNonDeletedAsync(PaginationParameter paginationParamter)
        {
            var (centers, totalCount) = await unitOfWork.Centers.GetNonDeletedPagedAsync(paginationParamter);
            var centersDTO = mapper.Map<List<CenterResponseDTO>>(centers);
            return new PaginatedResult<CenterResponseDTO>(
                centersDTO,
                totalCount,
                paginationParamter.PageNumber,
                paginationParamter.PageSize
            );
        }

        public async Task<PaginatedResult<CenterResponseDTO>> GetDeletedAsync(PaginationParameter paginationParamter)
        {
            var (centers , totalCount) = await unitOfWork.Centers.GetDeletedPagedAsync(paginationParamter);
            var centersDTO = mapper.Map<List<CenterResponseDTO>>(centers);
            return new PaginatedResult<CenterResponseDTO>(
                centersDTO,
                totalCount,
                paginationParamter.PageNumber,
                paginationParamter.PageSize
            );
        }

  

        public async Task<PaginatedResult<CenterResponseDTO>> GetActiveAsync(PaginationParameter paginationParamter)
        {   
            var (centers , totalCount) = await unitOfWork.Centers.GetActivePagedAsync(paginationParamter);
            var centersDTO = mapper.Map<List<CenterResponseDTO>>(centers);  
            return new PaginatedResult<CenterResponseDTO>(
                centersDTO,
                totalCount,
                paginationParamter.PageNumber,
                paginationParamter.PageSize
            );
        }

        public async Task<PaginatedResult<CenterResponseDTO>> GetInactiveAsync(PaginationParameter paginationParamter)
        {
            var (centers , totalCount) = await unitOfWork.Centers.GetInactivePagedAsync(paginationParamter);
            var centersDTO = mapper.Map<List<CenterResponseDTO>>(centers);
            return new PaginatedResult<CenterResponseDTO>(
                centersDTO,
                totalCount,
                paginationParamter.PageNumber,
                paginationParamter.PageSize
            );
        }

        public async Task<PaginatedResult<CenterResponseDTO>> GetCentersPagedAsync(PaginationParameter paginationParameter, string status)
        {
            return status?.ToLower() switch
            {
                "active" => await GetActiveAsync(paginationParameter),
                "inactive" => await GetInactiveAsync(paginationParameter),
                "deleted" => await GetDeletedAsync(paginationParameter),
                _ => await GetNonDeletedAsync(paginationParameter)
            };
        }

        public async Task UpdateAsync(int id, UpdateCenterDTO updateCenterDTO)
        {
            if (id <= 0)
                throw new BadRequestException("Invalid center id.");

            if (updateCenterDTO == null)
                throw new BadRequestException("Update Center DTO cannot be null.");

            var center = await unitOfWork.Centers.GetByIdAsync(id);
            if (center == null)
                throw new NotFoundException<int>(nameof(center), "center", id);

            mapper.Map(updateCenterDTO, center);

            unitOfWork.Centers.Update(center);
            await unitOfWork.Centers.SaveChangesAsync();
        }

        public async Task HardDeleteAsync(int id)
        {
            if (id <= 0)
                throw new BadRequestException("Invalid center id.");

            var center = await unitOfWork.Centers.GetByIdAsync(id);
            if (center == null)
                throw new NotFoundException<int>(nameof(center), "center", id);

            unitOfWork.Centers.HardDelete(center);
            await unitOfWork.Centers.SaveChangesAsync();
        }

        public async Task ActivateAsync(int id)
        {
            if (id <= 0)
                throw new BadRequestException("Invalid center id.");

            var center = await unitOfWork.Centers.GetByIdAsync(id);
            if (center == null)
                throw new NotFoundException<int>(nameof(center), "center", id);

            if (center.IsActive)
                throw new ConflictException("Center is already active.");

            unitOfWork.Centers.Activate(center);
            await unitOfWork.Centers.SaveChangesAsync();
        }

        public async Task DeactivateAsync(int id)
        {
            if (id <= 0)
                throw new BadRequestException("Invalid center id.");

            var center = await unitOfWork.Centers.GetByIdAsync(id);
            if (center == null)
                throw new NotFoundException<int>(nameof(center), "center", id);

            if (!center.IsActive)
                throw new ConflictException("Center is already inactive.");

            unitOfWork.Centers.Deactivate(center);
            await unitOfWork.Centers.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int id)
        {
            if (id <= 0)
                throw new BadRequestException("Invalid center id.");

            var center = await unitOfWork.Centers.GetByIdAsync(id);
            if (center == null)
                throw new NotFoundException<int>(nameof(Center), "id", id);

            if (center.IsDeleted)
                throw new ConflictException("Center is already deleted.");

            unitOfWork.Centers.SoftDelete(center);
            await unitOfWork.Centers.SaveChangesAsync();
        }

        public async Task RestoreAsync(int id)
        {
            if (id <= 0)
                throw new BadRequestException("Invalid center id.");

            var center = await unitOfWork.Centers.GetByIdAsync(id);
            if (center == null)
                throw new NotFoundException<int>(nameof(center), "center", id);

            if (!center.IsDeleted)
                throw new ConflictException("Center is not deleted.");

            unitOfWork.Centers.Restore(center);
            await unitOfWork.Centers.SaveChangesAsync();
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await unitOfWork.Centers.GetTotalCountAsync();
        }

        public async Task<int> GetNonDeletedCountAsync()
        {
            return await unitOfWork.Centers.GetNonDeletedCountAsync();
        }

        public async Task<int> GetDeletedCountAsync()
        {
            return await unitOfWork.Centers.GetDeletedCountAsync();
        }


    }
}