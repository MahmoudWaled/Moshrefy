using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moshrefy.Application.DTOs.User;
using Moshrefy.Application.DTOs.Common;
using Moshrefy.Application.Interfaces.IServices;
using Moshrefy.Domain.Enums;
using Moshrefy.Domain.Exceptions;
using Moshrefy.Domain.Identity;
using Moshrefy.Domain.Paramter;

namespace Moshrefy.Application.Services
{
    // Service for managing users within the tenant's center context.
    public class UserManagementService(
        UserManager<ApplicationUser> _userManager,
        IMapper _mapper,
        ITenantContext _tenantContext) : IUserManagementService
    {
        #region User Management

        // Create user
        public async Task<UserResponseDTO> CreateUserAsync(CreateUserDTO createUserDTO)
        {
            if (createUserDTO == null)
                throw new BadRequestException("CreateUserDTO cannot be null.");

            // Check if the current user is assigned to a center
            var currentCenterId = _tenantContext.GetCurrentCenterId();
            if (currentCenterId == null)
                throw new BadRequestException("Admin must be assigned to a center.");

            // Check role constraints
            if (createUserDTO.RoleName == RolesNames.SuperAdmin)
                throw new ForbiddenException("You cannot create a SuperAdmin user.");

            // forbid creating Admin users unless current user is SuperAdmin
            if (createUserDTO.RoleName == RolesNames.Admin)
            {
                var isCurrentUserSuperAdmin = _tenantContext.IsSuperAdmin();
                if (!isCurrentUserSuperAdmin)
                    throw new ForbiddenException("Only SuperAdmin can create Admin users.");
            }

            // Forbid creating users for other centers
            if (createUserDTO.CenterId != currentCenterId)
                throw new ForbiddenException("You can only create users for your center.");

            createUserDTO.CenterId = currentCenterId;

            if (!string.IsNullOrEmpty(createUserDTO.UserName))
            {
                var existingUser = await _userManager.FindByNameAsync(createUserDTO.UserName);
                if (existingUser != null)
                    throw new ConflictException($"A user with username {createUserDTO.UserName} already exists.");
            }

            if (!string.IsNullOrEmpty(createUserDTO.Email))
            {
                var existingUser = await _userManager.FindByEmailAsync(createUserDTO.Email);
                if (existingUser != null)
                    throw new ConflictException($"A user with email {createUserDTO.Email} already exists.");
            }

            var user = _mapper.Map<ApplicationUser>(createUserDTO);
            var currentUser = await _userManager.FindByIdAsync(_tenantContext.GetCurrentUserId());
            user.CreatedById = currentUser?.Id;
            user.CreatedByName = currentUser?.UserName;

            var result = await _userManager.CreateAsync(user, createUserDTO.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new FailedException($"User creation failed: {errors}");
            }

            await _userManager.AddToRoleAsync(user, createUserDTO.RoleName.ToString());

            return _mapper.Map<UserResponseDTO>(user);
        }

        public async Task<List<UserResponseDTO>> GetAllUsersInMyCenterAsync(PaginationParamter paginationParamter)
        {
            var currentCenterId = _tenantContext.GetCurrentCenterId();
            if (currentCenterId == null)
                throw new BadRequestException("Admin must be assigned to a center.");

            // Exclude soft-deleted users
            var query = _userManager.Users.Where(u => u.CenterId == currentCenterId && !u.IsDeleted);

            if (paginationParamter.PageSize != null && paginationParamter.PageNumber != null)
            {
                query = query
                    .Skip((paginationParamter.PageNumber.Value - 1) * paginationParamter.PageSize.Value)
                    .Take(paginationParamter.PageSize.Value);
            }

            var users = await query.ToListAsync();
            return _mapper.Map<List<UserResponseDTO>>(users);
        }

        public async Task<List<UserResponseDTO>> GetActiveUsersInMyCenterAsync(PaginationParamter paginationParamter)
        {
            var currentCenterId = _tenantContext.GetCurrentCenterId();
            if (currentCenterId == null)
                throw new BadRequestException("Admin must be assigned to a center.");

            // Exclude soft-deleted users
            var query = _userManager.Users
                .Where(u => u.CenterId == currentCenterId && u.IsActive && !u.IsDeleted);

            if (paginationParamter.PageSize != null && paginationParamter.PageNumber != null)
            {
                query = query
                    .Skip((paginationParamter.PageNumber.Value - 1) * paginationParamter.PageSize.Value)
                    .Take(paginationParamter.PageSize.Value);
            }

            var users = await query.ToListAsync();
            return _mapper.Map<List<UserResponseDTO>>(users);
        }

        public async Task<List<UserResponseDTO>> GetInactiveUsersInMyCenterAsync(PaginationParamter paginationParamter)
        {
            var currentCenterId = _tenantContext.GetCurrentCenterId();
            if (currentCenterId == null)
                throw new BadRequestException("Admin must be assigned to a center.");

            // Exclude soft-deleted users
            var query = _userManager.Users
                .Where(u => u.CenterId == currentCenterId && !u.IsActive && !u.IsDeleted);

            if (paginationParamter.PageSize != null && paginationParamter.PageNumber != null)
            {
                query = query
                    .Skip((paginationParamter.PageNumber.Value - 1) * paginationParamter.PageSize.Value)
                    .Take(paginationParamter.PageSize.Value);
            }

            var users = await query.ToListAsync();
            return _mapper.Map<List<UserResponseDTO>>(users);
        }

        public async Task<List<UserResponseDTO>> GetUsersByRoleInMyCenterAsync(string roleName, PaginationParamter paginationParamter)
        {
            if (string.IsNullOrEmpty(roleName))
                throw new BadRequestException("Role name cannot be null.");

            if (!Enum.TryParse<RolesNames>(roleName, out var parsedRole))
                throw new BadRequestException("Invalid role name.");

            var currentCenterId = _tenantContext.GetCurrentCenterId();
            if (currentCenterId == null)
                throw new BadRequestException("Admin must be assigned to a center.");

            var usersInRole = await _userManager.GetUsersInRoleAsync(parsedRole.ToString());

            // Filter users by current center
            var filteredUsers = usersInRole.Where(u => u.CenterId == currentCenterId).ToList();

            var paginatedUsers = filteredUsers.AsEnumerable();
            if (paginationParamter.PageSize != null && paginationParamter.PageNumber != null)
            {
                paginatedUsers = paginatedUsers
                    .Skip((paginationParamter.PageNumber.Value - 1) * paginationParamter.PageSize.Value)
                    .Take(paginationParamter.PageSize.Value);
            }

            return _mapper.Map<List<UserResponseDTO>>(paginatedUsers.ToList());
        }

        public async Task<UserResponseDTO> GetUserByIdInMyCenterAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new BadRequestException("User id cannot be null.");

            var currentCenterId = _tenantContext.GetCurrentCenterId();
            if (currentCenterId == null)
                throw new BadRequestException("Admin must be assigned to a center.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.CenterId != currentCenterId)
                throw new NotFoundException<string>(nameof(user), "id", userId);

            return _mapper.Map<UserResponseDTO>(user);
        }

        public async Task UpdateUserAsync(string userId, UpdateUserDTO updateUserDTO)
        {
            if (string.IsNullOrEmpty(userId))
                throw new BadRequestException("User id cannot be null.");
            if (updateUserDTO == null)
                throw new BadRequestException("User data cannot be null.");

            var currentCenterId = _tenantContext.GetCurrentCenterId();
            if (currentCenterId == null)
                throw new BadRequestException("Admin must be assigned to a center.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.CenterId != currentCenterId)
                throw new NotFoundException<string>(nameof(user), "id", userId);

            // Forbid changing center
            if (updateUserDTO.CenterId != currentCenterId)
                throw new ForbiddenException("You cannot change the center for this user.");

            if (!string.IsNullOrEmpty(updateUserDTO.UserName))
            {
                var existingUser = await _userManager.FindByNameAsync(updateUserDTO.UserName);
                if (existingUser != null && existingUser.Id != userId)
                    throw new ConflictException($"A user with username {updateUserDTO.UserName} already exists.");
            }

            if (!string.IsNullOrEmpty(updateUserDTO.Email))
            {
                var existingUser = await _userManager.FindByEmailAsync(updateUserDTO.Email);
                if (existingUser != null && existingUser.Id != userId)
                    throw new ConflictException($"A user with email {updateUserDTO.Email} already exists.");
            }

            _mapper.Map(updateUserDTO, user);
            var currentUser = await _userManager.FindByIdAsync(_tenantContext.GetCurrentUserId());
            user.ModifiedById = currentUser?.Id;
            user.ModifiedByName = currentUser?.UserName;
            user.ModifiedAt = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new FailedException($"User update failed: {errors}");
            }
        }

        public async Task ActivateUserAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new BadRequestException("User id cannot be null.");

            var currentCenterId = _tenantContext.GetCurrentCenterId();
            if (currentCenterId == null)
                throw new BadRequestException("Admin must be assigned to a center.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.CenterId != currentCenterId)
                throw new NotFoundException<string>(nameof(user), "id", userId);

            if (user.IsActive)
                throw new ConflictException("User is already active.");

            var currentUser = await _userManager.FindByIdAsync(_tenantContext.GetCurrentUserId());
            user.IsActive = true;
            user.ModifiedById = currentUser?.Id;
            user.ModifiedByName = currentUser?.UserName;
            user.ModifiedAt = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);
        }

        public async Task DeactivateUserAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new BadRequestException("User id cannot be null.");

            var currentCenterId = _tenantContext.GetCurrentCenterId();
            if (currentCenterId == null)
                throw new BadRequestException("Admin must be assigned to a center.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.CenterId != currentCenterId)
                throw new NotFoundException<string>(nameof(user), "id", userId);

            if (!user.IsActive)
                throw new ConflictException("User is already inactive.");

            var currentUser = await _userManager.FindByIdAsync(_tenantContext.GetCurrentUserId());
            user.IsActive = false;
            user.ModifiedById = currentUser?.Id;
            user.ModifiedByName = currentUser?.UserName;
            user.ModifiedAt = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);
        }

        public async Task SoftDeleteUserAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new BadRequestException("User id cannot be null.");

            var currentCenterId = _tenantContext.GetCurrentCenterId();
            if (currentCenterId == null)
                throw new BadRequestException("Admin must be assigned to a center.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.CenterId != currentCenterId)
                throw new NotFoundException<string>(nameof(user), "id", userId);

            if (user.IsDeleted)
                throw new ConflictException("User is already deleted.");

            var currentUser = await _userManager.FindByIdAsync(_tenantContext.GetCurrentUserId());
            user.IsDeleted = true;
            user.IsActive = false;
            user.ModifiedById = currentUser?.Id;
            user.ModifiedByName = currentUser?.UserName;
            user.ModifiedAt = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);
        }

        #endregion

        #region Role Management

        // Update user role
        public async Task UpdateUserRoleAsync(string userId, string newRole)
        {
            if (string.IsNullOrEmpty(userId))
                throw new BadRequestException("User id cannot be null.");
            
            if (string.IsNullOrEmpty(newRole))
                throw new BadRequestException("New role cannot be null.");

            var currentCenterId = _tenantContext.GetCurrentCenterId();
            if (currentCenterId == null)
                throw new BadRequestException("Admin must be assigned to a center.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.CenterId != currentCenterId)
                throw new NotFoundException<string>(nameof(user), "id", userId);

            // Validate role
            if (!Enum.TryParse<RolesNames>(newRole, out var parsedRole))
                throw new BadRequestException("Invalid role name.");

            // Check role constraints - only allow Employee and Manager
            if (parsedRole != RolesNames.Employee && parsedRole != RolesNames.Manager)
                throw new ForbiddenException("You can only assign Employee or Manager roles.");

            // Get current roles
            var currentRoles = await _userManager.GetRolesAsync(user);

            // Remove all current roles
            if (currentRoles.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                {
                    var errors = string.Join(", ", removeResult.Errors.Select(e => e.Description));
                    throw new FailedException($"Failed to remove current roles: {errors}");
                }
            }

            // Add new role
            var addResult = await _userManager.AddToRoleAsync(user, parsedRole.ToString());
            if (!addResult.Succeeded)
            {
                var errors = string.Join(", ", addResult.Errors.Select(e => e.Description));
                throw new FailedException($"Failed to add new role: {errors}");
            }

            // Update modification info
            var currentUser = await _userManager.FindByIdAsync(_tenantContext.GetCurrentUserId());
            user.ModifiedById = currentUser?.Id;
            user.ModifiedByName = currentUser?.UserName;
            user.ModifiedAt = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);
        }

        #endregion

        #region DataTables

        // Get total count
        public async Task<int> GetTotalCountAsync()
        {
            var centerId = _tenantContext.GetCurrentCenterId();
            if (centerId == null)
                throw new BadRequestException("Admin must be assigned to a center.");

            return await _userManager.Users
                .Where(u => u.CenterId == centerId && !u.IsDeleted)
                .CountAsync();
        }

        // Get users for DataTables
        public async Task<DataTableResponse<UserResponseDTO>> GetUsersDataTableAsync(DataTableRequest request)
        {
            var currentCenterId = _tenantContext.GetCurrentCenterId();
            if (currentCenterId == null)
                throw new BadRequestException("Admin must be assigned to a center.");

            var query = _userManager.Users.Where(u => u.CenterId == currentCenterId);

            // 1. Initial Filtering (Active/Deleted)
            if (request.FilterDeleted == "deleted")
            {
                query = query.Where(u => u.IsDeleted);
            }
            else
            {
                query = query.Where(u => !u.IsDeleted);
            }

            // Role filtering
            if (!string.IsNullOrEmpty(request.FilterRole) && request.FilterRole != "all")
            {
                var roleUsers = await _userManager.GetUsersInRoleAsync(request.FilterRole);
                var roleUserIds = roleUsers.Select(u => u.Id).ToList();
                query = query.Where(u => roleUserIds.Contains(u.Id));
            }

            // Active/Inactive Filter
            if (!string.IsNullOrEmpty(request.FilterStatus) && request.FilterStatus != "all")
            {
                bool isActive = request.FilterStatus == "active";
                query = query.Where(u => u.IsActive == isActive);
            }

            // Count total and filtered (Total is based on context before search but after high-level filters?)
            // Usually TotalRecords = All records in the system (or filtered by deleted status).
            // FilteredRecords = Records after search and specific filters.
            
            var totalQuery = _userManager.Users.Where(u => u.CenterId == currentCenterId);
            if (request.FilterDeleted == "deleted") totalQuery = totalQuery.Where(u => u.IsDeleted);
            else totalQuery = totalQuery.Where(u => !u.IsDeleted);
            
            var recordsTotal = await totalQuery.CountAsync();

            // 3. Search
            if (!string.IsNullOrEmpty(request.SearchValue))
            {
                var search = request.SearchValue.ToLower();
                query = query.Where(u =>
                    u.UserName.ToLower().Contains(search) ||
                    u.Email.ToLower().Contains(search) ||
                    u.PhoneNumber.Contains(search) ||
                    u.Id.Contains(search) ||
                    (u.CreatedByName != null && u.CreatedByName.ToLower().Contains(search))
                );
            }

            var recordsFiltered = await query.CountAsync();

            // 4. Sorting
            if (!string.IsNullOrEmpty(request.SortColumnName) && !string.IsNullOrEmpty(request.SortDirection))
            {
                bool isAsc = request.SortDirection.ToLower() == "asc";
                switch (request.SortColumnName.ToLower())
                {
                    case "username": query = isAsc ? query.OrderBy(u => u.UserName) : query.OrderByDescending(u => u.UserName); break;
                    case "email": query = isAsc ? query.OrderBy(u => u.Email) : query.OrderByDescending(u => u.Email); break;
                    case "phonenumber": query = isAsc ? query.OrderBy(u => u.PhoneNumber) : query.OrderByDescending(u => u.PhoneNumber); break;
                    case "isactive": query = isAsc ? query.OrderBy(u => u.IsActive) : query.OrderByDescending(u => u.IsActive); break;
                    case "createdbyname": query = isAsc ? query.OrderBy(u => u.CreatedByName) : query.OrderByDescending(u => u.CreatedByName); break;
                    case "createdat": query = isAsc ? query.OrderBy(u => u.CreatedAt) : query.OrderByDescending(u => u.CreatedAt); break;
                    default: query = query.OrderBy(u => u.UserName); break;
                }
            }
            else
            {
                query = query.OrderByDescending(u => u.CreatedAt); // Default sort
            }

            // 5. Pagination
            var data = await query
                .Skip(request.Skip)
                .Take(request.PageSize)
                .ToListAsync();

            var mappedData = _mapper.Map<List<UserResponseDTO>>(data);

            // Populate Roles
            foreach (var userDto in mappedData)
            {
                var user = data.First(u => u.Id == userDto.Id);
                var roles = await _userManager.GetRolesAsync(user);
                userDto.RoleName = roles.FirstOrDefault();
            }

            return new DataTableResponse<UserResponseDTO>
            {
                Draw = request.Draw,
                RecordsTotal = recordsTotal,
                RecordsFiltered = recordsFiltered,
                Data = mappedData
            };
        }

        #endregion
    }
}

