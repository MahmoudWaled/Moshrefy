using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moshrefy.Application.DTOs.AcademicYear;
using Moshrefy.Application.Interfaces.IUnitOfWork;
using Moshrefy.Application.Interfaces.IServices;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Exceptions;
using Moshrefy.Domain.Identity;
using Moshrefy.Domain.Paramter;
using Microsoft.EntityFrameworkCore;

namespace Moshrefy.Application.Services
{
    public class AcademicYearService(
        IUnitOfWork _unitOfWork,
        IMapper _mapper,
        ITenantContext tenantContext,
        UserManager<ApplicationUser> _userManager
        ) : BaseService(tenantContext), IAcademicYearService
    {
        #region Academic Year Management

        // Create academic year
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

        // Get academic year by ID
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

        // Get academic years by name
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

        // Get all academic years
        public async Task<List<AcademicYearResponseDTO>> GetAllAcademicYearsAsync(PaginationParamter paginationParamter)
        {
            if (paginationParamter == null)
                throw new BadRequestException("Pagination parameters cannot be null.");

            var currentCenterId = GetCurrentCenterIdOrThrow();

            var academicYears = await _unitOfWork.AcademicYears.GetAllAsync(
                ay => ay.CenterId == currentCenterId && !ay.IsDeleted,
                paginationParamter);

            if (!academicYears.Any())
                throw new NoDataFoundException("No academic years found.");

            return _mapper.Map<List<AcademicYearResponseDTO>>(academicYears.ToList());
        }

        // Update academic year
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

        // Delete academic year
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

        // Get total count
        public async Task<int> GetTotalCountAsync()
        {
            var centerId = GetCurrentCenterIdOrThrow();
            return await _unitOfWork.AcademicYears.CountAsync(ay => ay.CenterId == centerId && !ay.IsDeleted);
        }

        #endregion

        #region DataTables

        // Get academic years for DataTables
        public async Task<Moshrefy.Application.DTOs.Common.DataTableResponse<AcademicYearResponseDTO>> GetAcademicYearsDataTableAsync(Moshrefy.Application.DTOs.Common.DataTableRequest request)
        {
            var centerId = GetCurrentCenterIdOrThrow();

            // Initial Query
            var query = _unitOfWork.AcademicYears.GetQueryable()
                .Where(ay => ay.CenterId == centerId && !ay.IsDeleted);

            // Count Total Records
            var totalRecords = await _unitOfWork.AcademicYears.CountAsync(ay => ay.CenterId == centerId && !ay.IsDeleted);

            // Apply Search Filter
            if (!string.IsNullOrEmpty(request.SearchValue))
            {
                var search = request.SearchValue.ToLower();
                query = query.Where(ay =>
                    ay.Name.ToLower().Contains(search) ||
                    (ay.CreatedByName != null && ay.CreatedByName.ToLower().Contains(search))
                );
            }

            // Apply Status Filter
            if (!string.IsNullOrEmpty(request.FilterStatus) && request.FilterStatus != "all")
            {
                bool isActive = request.FilterStatus == "active";
                query = query.Where(ay => ay.IsActive == isActive);
            }

            // Count Filtered Records
            var filteredRecords = await query.CountAsync();

            // Apply Sorting
            if (!string.IsNullOrEmpty(request.SortColumnName) && !string.IsNullOrEmpty(request.SortDirection))
            {
                bool isAsc = request.SortDirection.ToLower() == "asc";
                query = request.SortColumnName.ToLower() switch
                {
                    "name" => isAsc ? query.OrderBy(ay => ay.Name) : query.OrderByDescending(ay => ay.Name),
                    "createdat" => isAsc ? query.OrderBy(ay => ay.CreatedAt) : query.OrderByDescending(ay => ay.CreatedAt),
                    "createdbyname" => isAsc ? query.OrderBy(ay => ay.CreatedByName) : query.OrderByDescending(ay => ay.CreatedByName),
                    "isactive" => isAsc ? query.OrderBy(ay => ay.IsActive) : query.OrderByDescending(ay => ay.IsActive),
                    _ => query.OrderByDescending(ay => ay.CreatedAt)
                };
            }
            else
            {
                query = query.OrderByDescending(ay => ay.CreatedAt);
            }

            // Apply Pagination
            if (request.Length > 0)
            {
                query = query.Skip(request.Start).Take(request.Length);
            }

            // Execute Query
            var academicYears = await query.ToListAsync();
            var data = _mapper.Map<List<AcademicYearResponseDTO>>(academicYears);

            return new Moshrefy.Application.DTOs.Common.DataTableResponse<AcademicYearResponseDTO>
            {
                Draw = request.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = filteredRecords,
                Data = data
            };
        }

        #endregion
    }
}
