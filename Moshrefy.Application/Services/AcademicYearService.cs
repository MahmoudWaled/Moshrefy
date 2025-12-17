using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moshrefy.Application.DTOs.AcademicYear;
using Moshrefy.Application.Interfaces.IUnitOfWork;
using Moshrefy.Application.Interfaces.IServices;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Exceptions;
using Moshrefy.Domain.Identity;
using Moshrefy.Domain.Paramter;

namespace Moshrefy.Application.Services
{
    public class AcademicYearService(
        IUnitOfWork _unitOfWork,
        IMapper _mapper,
        ITenantContext tenantContext,
        UserManager<ApplicationUser> _userManager
        ) : BaseService(tenantContext), IAcademicYearService
    {
        public async Task AddAcademicYearAsync(CreateAcademicYearDTO createAcademicYearDTO)
        {
            if (createAcademicYearDTO == null)
                throw new BadRequestException("CreateAcademicYearDTO cannot be null.");

            var currentCenterId = GetCurrentCenterIdOrThrow();
            var academicYear = _mapper.Map<AcademicYear>(createAcademicYearDTO);

            var currentUser = await _userManager.FindByIdAsync(tenantContext.GetCurrentUserId());
            academicYear.CenterId = currentCenterId;
            academicYear.CreatedById = currentUser!.Id;
            academicYear.CreatedByName = currentUser!.UserName ?? string.Empty;

            await _unitOfWork.AcademicYears.AddAsync(academicYear);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAcademicYearAsync(int id)
        {
            if (id <= 0)
                throw new BadRequestException("Invalid academic year id.");

            var academicYear = await _unitOfWork.AcademicYears.GetByIdAsync(id);

            if (academicYear == null)
                throw new NotFoundException<int>(nameof(AcademicYear), "id", id);

            ValidateCenterAccess(academicYear.CenterId, nameof(AcademicYear));
            var currentUser = await _userManager.FindByIdAsync(tenantContext.GetCurrentUserId());
            academicYear.IsDeleted = true;
            academicYear.IsActive = false;
            academicYear.ModifiedById = currentUser!.Id;
            academicYear.ModifiedByName = currentUser!.UserName ?? string.Empty;
            academicYear.ModifiedAt = DateTime.UtcNow;

            _unitOfWork.AcademicYears.UpdateAsync(academicYear);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<AcademicYearResponseDTO> GetAcademicYearByIdAsync(int id)
        {
            if (id <= 0)
                throw new BadRequestException("Invalid academic year id.");

            var academicYear = await _unitOfWork.AcademicYears.GetByIdAsync(id);

            if (academicYear == null)
                throw new NotFoundException<int>(nameof(AcademicYear), "id", id);

            ValidateCenterAccess(academicYear.CenterId, nameof(AcademicYear));

            return _mapper.Map<AcademicYearResponseDTO>(academicYear);
        }

        public async Task<List<AcademicYearResponseDTO>> GetAcademicYearsByNameAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new BadRequestException("Academic year name cannot be null or empty.");

            var currentCenterId = GetCurrentCenterIdOrThrow();

            var academicYears = await _unitOfWork.AcademicYears.GetByName(name);

            var filteredAcademicYears = academicYears
                .Where(ay => ay.CenterId == currentCenterId)
                .ToList();

            if (filteredAcademicYears.Count == 0)
                throw new NoDataFoundException($"No academic years found with name '{name}' in your center.");

            return _mapper.Map<List<AcademicYearResponseDTO>>(filteredAcademicYears);
        }

        public async Task<List<AcademicYearResponseDTO>> GetAllAcademicYearsAsync(PaginationParamter paginationParamter)
        {
            if (paginationParamter == null)
                throw new BadRequestException("Pagination parameters cannot be null.");

            var currentCenterId = GetCurrentCenterIdOrThrow();

            // Filter at database level BEFORE pagination
            var academicYears = await _unitOfWork.AcademicYears.GetAllAsync(
                ay => ay.CenterId == currentCenterId && !ay.IsDeleted,
                paginationParamter);

            if (!academicYears.Any())
                throw new NoDataFoundException("No academic years found.");

            return _mapper.Map<List<AcademicYearResponseDTO>>(academicYears.ToList());
        }


        public async Task UpdateAcademicYearAsync(int id, UpdateAcademicYearDTO updateAcademicYearDTO)
        {
            if (id <= 0)
                throw new BadRequestException("Invalid academic year id.");

            if (updateAcademicYearDTO == null)
                throw new BadRequestException("UpdateAcademicYearDTO cannot be null.");

            var academicYear = await _unitOfWork.AcademicYears.GetByIdAsync(id);

            if (academicYear == null)
                throw new NotFoundException<int>(nameof(AcademicYear), "id", id);

            ValidateCenterAccess(academicYear.CenterId, nameof(AcademicYear));

            _mapper.Map(updateAcademicYearDTO, academicYear);
            var currentUser = await _userManager.FindByIdAsync(tenantContext.GetCurrentUserId());
            academicYear.ModifiedById = currentUser!.Id;
            academicYear.ModifiedByName = currentUser!.UserName ?? string.Empty;
            academicYear.ModifiedAt = DateTime.UtcNow;

            _unitOfWork.AcademicYears.UpdateAsync(academicYear);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> GetTotalCountAsync()
        {
            var centerId = GetCurrentCenterIdOrThrow();
            return await _unitOfWork.AcademicYears.CountAsync(ay => ay.CenterId == centerId && !ay.IsDeleted);
        }
    }
}
