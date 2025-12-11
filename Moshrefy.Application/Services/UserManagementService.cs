using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moshrefy.Application.DTOs.User;
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

            var query = _userManager.Users.Where(u => u.CenterId == currentCenterId);

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

            var query = _userManager.Users
                .Where(u => u.CenterId == currentCenterId && u.IsActive);

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

            var query = _userManager.Users
                .Where(u => u.CenterId == currentCenterId && !u.IsActive);

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

        public async Task RestoreUserAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new BadRequestException("User id cannot be null.");

            var currentCenterId = _tenantContext.GetCurrentCenterId();
            if (currentCenterId == null)
                throw new BadRequestException("Admin must be assigned to a center.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.CenterId != currentCenterId)
                throw new NotFoundException<string>(nameof(user), "id", userId);

            if (!user.IsDeleted)
                throw new ConflictException("User is not deleted.");

            var currentUser = await _userManager.FindByIdAsync(_tenantContext.GetCurrentUserId());
            user.IsDeleted = false;
            user.IsActive = true;
            user.ModifiedById = currentUser?.Id;
            user.ModifiedByName = currentUser?.UserName;
            user.ModifiedAt = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);
        }
    }
}