using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moshrefy.Application.DTOs.Center;
using Moshrefy.Application.DTOs.Statistics;
using Moshrefy.Application.DTOs.User;
using Moshrefy.Application.Interfaces.IServices;
using Moshrefy.Application.Interfaces.IUnitOfWork;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Enums;
using Moshrefy.Domain.Exceptions;
using Moshrefy.Domain.Identity;
using Moshrefy.Domain.Paramter;

namespace Moshrefy.Application.Services
{
    public class SuperAdminService(
        IUnitOfWork _unitOfWork,
        UserManager<ApplicationUser> _userManager,
        SignInManager<ApplicationUser> _signInManager,
        IMapper _mapper,
        ITenantContext _tenantContext) : ISuperAdminService
    {


        #region Center Management
        public async Task<CenterResponseDTO> CreateCenterAsync(CreateCenterDTO createCenterDTO)
        {
            if (createCenterDTO == null)
                throw new BadRequestException("CreateCenterDTO cannot be null.");

            var center = _mapper.Map<Center>(createCenterDTO);

            // Get current user info
            var currentUser = await _userManager.FindByIdAsync(_tenantContext.GetCurrentUserId());

            // Set audit fields
            center.CreatedById = currentUser!.Id;
            center.CreatedByName = currentUser!.UserName ?? string.Empty;
            center.IsActive = true;
            center.IsDeleted = false;

            await _unitOfWork.Centers.AddAsync(center);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<CenterResponseDTO>(center);
        }

        public async Task<List<CenterResponseDTO>> GetAllCentersAsync(PaginationParamter paginationParamter)
        {
            var centers = await _unitOfWork.Centers.GetAllAsync(paginationParamter);
            return _mapper.Map<List<CenterResponseDTO>>(centers);
        }

        public async Task<int> GetTotalCentersCountAsync()
        {
            return await _unitOfWork.Centers.GetTotalCountAsync();
        }

        public async Task<List<CenterResponseDTO>> GetNonDeletedCentersAsync(PaginationParamter paginationParamter)
        {
            var centers = await _unitOfWork.Centers.GetNonDeletedCentersAsync(paginationParamter);
            return _mapper.Map<List<CenterResponseDTO>>(centers);
        }

        public async Task<int> GetNonDeletedCentersCountAsync()
        {
            return await _unitOfWork.Centers.GetNonDeletedCountAsync();
        }

        public async Task<List<CenterResponseDTO>> GetActiveCentersAsync(PaginationParamter paginationParamter)
        {
            var activeCenters = await _unitOfWork.Centers.GetActiveCentersAsync(paginationParamter);
            return _mapper.Map<List<CenterResponseDTO>>(activeCenters);
        }

        public async Task<List<CenterResponseDTO>> GetInactiveCentersAsync(PaginationParamter paginationParamter)
        {
            var inactiveCenters = await _unitOfWork.Centers.GetInactiveCentersAsync(paginationParamter);
            return _mapper.Map<List<CenterResponseDTO>>(inactiveCenters);
        }

        public async Task<List<CenterResponseDTO>> GetDeletedCentersAsync(PaginationParamter paginationParamter)
        {
            var deletedCenters = await _unitOfWork.Centers.GetDeletedCentersAsync(paginationParamter);
            return _mapper.Map<List<CenterResponseDTO>>(deletedCenters);
        }

        public async Task<int> GetDeletedCentersCountAsync()
        {
            return await _unitOfWork.Centers.GetDeletedCountAsync();
        }

        public async Task<CenterResponseDTO> GetCenterByIdAsync(int centerId)
        {
            if (centerId <= 0)
                throw new BadRequestException("Invalid center id.");

            var center = await _unitOfWork.Centers.GetByIdAsync(centerId);

            if (center == null)
                throw new NotFoundException<int>(nameof(Center), "id", centerId);

            return _mapper.Map<CenterResponseDTO>(center);
        }

        public async Task UpdateCenterAsync(int centerId, UpdateCenterDTO updateCenterDTO)
        {
            if (centerId <= 0)
                throw new BadRequestException("Invalid center id.");

            if (updateCenterDTO == null)
                throw new BadRequestException("UpdateCenterDTO cannot be null.");

            var center = await _unitOfWork.Centers.GetByIdAsync(centerId);

            if (center == null)
                throw new NotFoundException<int>(nameof(Center), "id", centerId);

            _mapper.Map(updateCenterDTO, center);

            // Get current user info
            var currentUser = await _userManager.FindByIdAsync(_tenantContext.GetCurrentUserId());

            // Set audit fields
            center.ModifiedById = currentUser?.Id;
            center.ModifiedByName = currentUser?.UserName;
            center.ModifiedAt = DateTimeOffset.UtcNow;

            _unitOfWork.Centers.UpdateAsync(center);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task SoftDeleteCenterAsync(int centerId)
        {
            if (centerId <= 0)
                throw new BadRequestException("Invalid center id.");

            var center = await _unitOfWork.Centers.GetByIdAsync(centerId);

            if (center == null)
                throw new NotFoundException<int>(nameof(Center), "id", centerId);

            if (center.IsDeleted)
                throw new ConflictException("Center is already deleted.");

            // Get current user info
            var currentUser = await _userManager.FindByIdAsync(_tenantContext.GetCurrentUserId());

            // Set audit fields
            center.IsDeleted = true;
            center.IsActive = false;
            center.ModifiedById = currentUser?.Id;
            center.ModifiedByName = currentUser?.UserName;
            center.ModifiedAt = DateTimeOffset.UtcNow;

            _unitOfWork.Centers.UpdateAsync(center);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteCenterAsync(int centerId)
        {
            if (centerId <= 0)
                throw new BadRequestException("Invalid center id.");

            var center = await _unitOfWork.Centers.GetByIdAsync(centerId);

            if (center == null)
                throw new NotFoundException<int>(nameof(Center), "id", centerId);

            _unitOfWork.Centers.DeleteAsync(center);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RestoreCenterAsync(int centerId)
        {
            if (centerId <= 0)
                throw new BadRequestException("Invalid center id.");

            var center = await _unitOfWork.Centers.GetByIdAsync(centerId);

            if (center == null)
                throw new NotFoundException<int>(nameof(Center), "id", centerId);

            if (!center.IsDeleted)
                throw new ConflictException("Center is not deleted.");

            // Get current user info
            var currentUser = await _userManager.FindByIdAsync(_tenantContext.GetCurrentUserId());

            // Restore center
            center.IsDeleted = false;
            center.IsActive = true;

            // Set audit fields
            center.ModifiedById = currentUser?.Id;
            center.ModifiedByName = currentUser?.UserName;
            center.ModifiedAt = DateTimeOffset.UtcNow;

            _unitOfWork.Centers.UpdateAsync(center);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ActivateCenterAsync(int centerId)
        {
            if (centerId <= 0)
                throw new BadRequestException("Invalid center id.");

            var center = await _unitOfWork.Centers.GetByIdAsync(centerId);

            if (center == null)
                throw new NotFoundException<int>(nameof(Center), "id", centerId);

            if (center.IsActive)
                throw new ConflictException("Center is already active.");

            // Get current user info
            var currentUser = await _userManager.FindByIdAsync(_tenantContext.GetCurrentUserId());

            // Activate center
            center.IsActive = true;

            // Set audit fields
            center.ModifiedById = currentUser?.Id;
            center.ModifiedByName = currentUser?.UserName;
            center.ModifiedAt = DateTimeOffset.UtcNow;

            _unitOfWork.Centers.UpdateAsync(center);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeactivateCenterAsync(int centerId)
        {
            if (centerId <= 0)
                throw new BadRequestException("Invalid center id.");

            var center = await _unitOfWork.Centers.GetByIdAsync(centerId);

            if (center == null)
                throw new NotFoundException<int>(nameof(Center), "id", centerId);

            if (!center.IsActive)
                throw new ConflictException("Center is already inactive.");

            // Get current user info
            var currentUser = await _userManager.FindByIdAsync(_tenantContext.GetCurrentUserId());

            // Deactivate center
            center.IsActive = false;

            // Set audit fields
            center.ModifiedById = currentUser?.Id;
            center.ModifiedByName = currentUser?.UserName;
            center.ModifiedAt = DateTimeOffset.UtcNow;

            _unitOfWork.Centers.UpdateAsync(center);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<UserResponseDTO?> GetCenterAdminAsync(int centerId)
        {
            if (centerId <= 0)
                throw new BadRequestException("Invalid center id.");

            // Find the first user with Admin role in this center
            var adminUsers = await _userManager.GetUsersInRoleAsync(RolesNames.Admin.ToString());
            var centerAdmin = adminUsers.FirstOrDefault(u => u.CenterId == centerId && !u.IsDeleted);

            if (centerAdmin == null)
                return null;

            return _mapper.Map<UserResponseDTO>(centerAdmin);
        }

       

        #endregion Center Management

        #region User Management

        public async Task<UserResponseDTO> CreateAdminForCenterAsync(CreateUserDTO createUserDTO)
        {
            if (createUserDTO == null)
                throw new BadRequestException("CreateUserDTO cannot be null.");

            if (createUserDTO.CenterId == null || createUserDTO.CenterId <= 0)
                throw new BadRequestException("CenterId is required.");

            var center = await _unitOfWork.Centers.GetByIdAsync(createUserDTO.CenterId.Value);
            if (center == null)
                throw new NotFoundException<int>(nameof(Center), "id", createUserDTO.CenterId.Value);

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
            user.IsActive = true;

            var result = await _userManager.CreateAsync(user, createUserDTO.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new BadRequestException($"User creation failed: {errors}");
            }

            // Get current user info
            var currentUser = await _userManager.FindByIdAsync(_tenantContext.GetCurrentUserId());

            // Set audit fields
            user.CreatedById = currentUser?.Id;
            user.CreatedByName = currentUser?.UserName;
            user.CreatedAt = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);

            await _userManager.AddToRoleAsync(user, createUserDTO.RoleName.ToString());

            // Reload user with Center navigation property
            var createdUser = await _userManager.Users
                .Include(u => u.Center)
                .FirstOrDefaultAsync(u => u.Id == user.Id);

            return _mapper.Map<UserResponseDTO>(createdUser ?? user);
        }

        public async Task<UserResponseDTO> CreateCenterAdminAsync(int centerId, CreateUserDTO createUserDTO)
        {
            if (centerId <= 0)
                throw new BadRequestException("Invalid center id.");

            if (createUserDTO == null)
                throw new BadRequestException("CreateUserDTO cannot be null.");

            // Verify center exists
            var center = await _unitOfWork.Centers.GetByIdAsync(centerId);
            if (center == null)
                throw new NotFoundException<int>(nameof(Center), "id", centerId);

            if (center.IsDeleted)
                throw new BadRequestException("Cannot create user for a deleted center.");

            if (!center.IsActive)
                throw new BadRequestException("Cannot create user for an inactive center.");

            // Check username uniqueness
            if (!string.IsNullOrEmpty(createUserDTO.UserName))
            {
                var existingUser = await _userManager.FindByNameAsync(createUserDTO.UserName);
                if (existingUser != null)
                    throw new ConflictException($"A user with username {createUserDTO.UserName} already exists.");
            }

            // Check email uniqueness
            if (!string.IsNullOrEmpty(createUserDTO.Email))
            {
                var existingUser = await _userManager.FindByEmailAsync(createUserDTO.Email);
                if (existingUser != null)
                    throw new ConflictException($"A user with email {createUserDTO.Email} already exists.");
            }

            // Override CenterId to ensure it matches the parameter
            createUserDTO.CenterId = centerId;
            
            // Create the user
            var user = _mapper.Map<ApplicationUser>(createUserDTO);
            user.IsActive = true;
            user.IsDeleted = false;

            var result = await _userManager.CreateAsync(user, createUserDTO.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new BadRequestException($"Center admin creation failed: {errors}");
            }

            // Get current user info for audit
            var currentUser = await _userManager.FindByIdAsync(_tenantContext.GetCurrentUserId());

            // Set audit fields
            user.CreatedById = currentUser?.Id;
            user.CreatedByName = currentUser?.UserName;
            user.CreatedAt = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);

            // Assign Admin role (or the role specified in DTO)
            var roleToAssign = createUserDTO.RoleName == RolesNames.Admin ? RolesNames.Admin : createUserDTO.RoleName;
            await _userManager.AddToRoleAsync(user, roleToAssign.ToString());

            // Reload user with Center navigation property
            var createdUser = await _userManager.Users
                .Include(u => u.Center)
                .FirstOrDefaultAsync(u => u.Id == user.Id);

            return _mapper.Map<UserResponseDTO>(createdUser ?? user);
        }

        public async Task<UserResponseDTO> CreateUserForCenterAsync(int centerId, CreateUserDTO createUserDTO)
        {
            if (centerId <= 0)
                throw new BadRequestException("Invalid center id.");

            if (createUserDTO == null)
                throw new BadRequestException("CreateUserDTO cannot be null.");

            // Verify center exists and is active
            var center = await _unitOfWork.Centers.GetByIdAsync(centerId);
            if (center == null)
                throw new NotFoundException<int>(nameof(Center), "id", centerId);

            if (center.IsDeleted)
                throw new BadRequestException("Cannot create user for a deleted center.");

            if (!center.IsActive)
                throw new BadRequestException("Cannot create user for an inactive center.");

            // Check username uniqueness
            if (!string.IsNullOrEmpty(createUserDTO.UserName))
            {
                var existingUser = await _userManager.FindByNameAsync(createUserDTO.UserName);
                if (existingUser != null)
                    throw new ConflictException($"A user with username {createUserDTO.UserName} already exists.");
            }

            // Check email uniqueness
            if (!string.IsNullOrEmpty(createUserDTO.Email))
            {
                var existingUser = await _userManager.FindByEmailAsync(createUserDTO.Email);
                if (existingUser != null)
                    throw new ConflictException($"A user with email {createUserDTO.Email} already exists.");
            }

            // Override CenterId to ensure it matches the parameter
            createUserDTO.CenterId = centerId;

            // Create the user
            var user = _mapper.Map<ApplicationUser>(createUserDTO);
            user.IsActive = true;
            user.IsDeleted = false;

            var result = await _userManager.CreateAsync(user, createUserDTO.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new BadRequestException($"User creation failed: {errors}");
            }

            // Get current user info for audit
            var currentUser = await _userManager.FindByIdAsync(_tenantContext.GetCurrentUserId());

            // Set audit fields
            user.CreatedById = currentUser?.Id;
            user.CreatedByName = currentUser?.UserName;
            user.CreatedAt = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);

            // Assign the specified role
            await _userManager.AddToRoleAsync(user, createUserDTO.RoleName.ToString());

            // Reload user with Center navigation property
            var createdUser = await _userManager.Users
                .Include(u => u.Center)
                .FirstOrDefaultAsync(u => u.Id == user.Id);

            return _mapper.Map<UserResponseDTO>(createdUser ?? user);
        }

        public async Task<List<UserResponseDTO>> GetAllUsersAsync(PaginationParamter paginationParamter)
        {
            IQueryable<ApplicationUser> query = _userManager.Users.Include(u => u.Center);

            if (paginationParamter.PageSize != null && paginationParamter.PageNumber != null)
            {
                query = query
                    .Skip((paginationParamter.PageNumber.Value - 1) * paginationParamter.PageSize.Value)
                    .Take(paginationParamter.PageSize.Value);
            }

            var users = await query.ToListAsync();
            return _mapper.Map<List<UserResponseDTO>>(users);
        }

        public async Task<List<UserResponseDTO>> GetUsersByCenterIdAsync(int centerId, PaginationParamter paginationParamter)
        {
            if (centerId <= 0)
                throw new BadRequestException("Invalid center id.");

            IQueryable<ApplicationUser> query = _userManager.Users.Include(u => u.Center).Where(u => u.CenterId == centerId);

            if (paginationParamter.PageSize != null && paginationParamter.PageNumber != null)
            {
                query = query
                    .Skip((paginationParamter.PageNumber.Value - 1) * paginationParamter.PageSize.Value)
                    .Take(paginationParamter.PageSize.Value);
            }

            var users = await query.ToListAsync();
            return _mapper.Map<List<UserResponseDTO>>(users);
        }

        public async Task<int> GetUsersByCenterIdCountAsync(int centerId)
        {
            if (centerId <= 0)
                throw new BadRequestException("Invalid center id.");

            return await _userManager.Users.CountAsync(u => u.CenterId == centerId);
        }

        public async Task<int> GetTotalUsersCountAsync()
        {
            return await _userManager.Users.CountAsync();
        }

        public async Task<int> GetActiveUsersCountAsync()
        {
            return await _userManager.Users.CountAsync(u => u.IsActive && !u.IsDeleted);
        }

        public async Task<int> GetInactiveUsersCountAsync()
        {
            return await _userManager.Users.CountAsync(u => !u.IsActive && !u.IsDeleted);
        }

        public async Task<int> GetDeletedUsersCountAsync()
        {
            return await _userManager.Users.CountAsync(u => u.IsDeleted);
        }

        public async Task<List<UserResponseDTO>> GetUsersByRoleAsync(string roleName, PaginationParamter paginationParamter)
        {
            if (string.IsNullOrEmpty(roleName))
                throw new BadRequestException("Role name cannot be null.");

            if (!Enum.TryParse<RolesNames>(roleName, out var parsedRole))
                throw new BadRequestException("Invalid role name.");

            var usersInRole = await _userManager.GetUsersInRoleAsync(parsedRole.ToString());

            var paginatedUsers = usersInRole.AsEnumerable();
            if (paginationParamter.PageSize != null && paginationParamter.PageNumber != null)
            {
                paginatedUsers = paginatedUsers
                    .Skip((paginationParamter.PageNumber.Value - 1) * paginationParamter.PageSize.Value)
                    .Take(paginationParamter.PageSize.Value);
            }

            return _mapper.Map<List<UserResponseDTO>>(paginatedUsers.ToList());
        }

        public async Task<UserResponseDTO> GetUserByIdAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new BadRequestException("User id cannot be null.");

            var user = await _userManager.Users.Include(u => u.Center).FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new NotFoundException<string>(nameof(ApplicationUser), "id", userId);

            return _mapper.Map<UserResponseDTO>(user);
        }

        public async Task<UserResponseDTO> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new BadRequestException("Email cannot be null.");

            var user = await _userManager.Users.Include(u => u.Center).FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                throw new NotFoundException<string>(nameof(ApplicationUser), "email", email);

            return _mapper.Map<UserResponseDTO>(user);
        }

        public async Task<UserResponseDTO> GetUserByUsernameAsync(string username)
        {
            if (string.IsNullOrEmpty(username))
                throw new BadRequestException("Username cannot be null.");

            var user = await _userManager.Users.Include(u => u.Center).FirstOrDefaultAsync(u => u.UserName == username);

            if (user == null)
                throw new NotFoundException<string>(nameof(ApplicationUser), "username", username);

            return _mapper.Map<UserResponseDTO>(user);
        }

        public async Task UpdateUserAsync(string userId, UpdateUserDTO updateUserDTO)
        {
            if (string.IsNullOrEmpty(userId))
                throw new BadRequestException("User id cannot be null.");

            if (updateUserDTO == null)
                throw new BadRequestException("UpdateUserDTO cannot be null.");

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                throw new NotFoundException<string>(nameof(ApplicationUser), "id", userId);

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
                throw new BadRequestException($"User update failed: {errors}");
            }
        }

        public async Task UpdateUserInAnyCenterAsync(string userId, UpdateUserDTO updateUserDTO)
        {
            if (string.IsNullOrEmpty(userId))
                throw new BadRequestException("User id cannot be null.");

            if (updateUserDTO == null)
                throw new BadRequestException("UpdateUserDTO cannot be null.");

            var user = await _userManager.Users
                .Include(u => u.Center)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new NotFoundException<string>(nameof(ApplicationUser), "id", userId);

            // If CenterId is provided, verify the center exists and is valid
            if (updateUserDTO.CenterId.HasValue)
            {
                var center = await _unitOfWork.Centers.GetByIdAsync(updateUserDTO.CenterId.Value);
                if (center == null)
                    throw new NotFoundException<int>(nameof(Center), "id", updateUserDTO.CenterId.Value);

                if (center.IsDeleted)
                    throw new BadRequestException("Cannot assign user to a deleted center.");

                if (!center.IsActive)
                    throw new BadRequestException("Cannot assign user to an inactive center.");
            }

            // Check username uniqueness
            if (!string.IsNullOrEmpty(updateUserDTO.UserName) && updateUserDTO.UserName != user.UserName)
            {
                var existingUser = await _userManager.FindByNameAsync(updateUserDTO.UserName);
                if (existingUser != null && existingUser.Id != userId)
                    throw new ConflictException($"A user with username {updateUserDTO.UserName} already exists.");
            }

            // Check email uniqueness
            if (!string.IsNullOrEmpty(updateUserDTO.Email) && updateUserDTO.Email != user.Email)
            {
                var existingUser = await _userManager.FindByEmailAsync(updateUserDTO.Email);
                if (existingUser != null && existingUser.Id != userId)
                    throw new ConflictException($"A user with email {updateUserDTO.Email} already exists.");
            }

            // Map updates
            _mapper.Map(updateUserDTO, user);
            
            // Set audit fields
            var currentUser = await _userManager.FindByIdAsync(_tenantContext.GetCurrentUserId());
            user.ModifiedById = currentUser?.Id;
            user.ModifiedByName = currentUser?.UserName;
            user.ModifiedAt = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new BadRequestException($"User update failed: {errors}");
            }
        }

        public async Task SoftDeleteUserAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new BadRequestException("User id cannot be null.");

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                throw new NotFoundException<string>(nameof(ApplicationUser), "id", userId);

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

        public async Task SoftDeleteUserForCenterAsync(int centerId, string userId)
        {
            if (centerId <= 0)
                throw new BadRequestException("Invalid center id.");

            if (string.IsNullOrEmpty(userId))
                throw new BadRequestException("User id cannot be null.");

            // Verify center exists
            var center = await _unitOfWork.Centers.GetByIdAsync(centerId);
            if (center == null)
                throw new NotFoundException<int>(nameof(Center), "id", centerId);

            // Find user and verify they belong to this center
            var user = await _userManager.Users
                .Include(u => u.Center)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new NotFoundException<string>(nameof(ApplicationUser), "id", userId);

            if (user.CenterId != centerId)
                throw new BadRequestException($"User does not belong to center with id {centerId}.");

            if (user.IsDeleted)
                throw new ConflictException("User is already deleted.");

            // Soft delete the user
            var currentUser = await _userManager.FindByIdAsync(_tenantContext.GetCurrentUserId());
            user.IsDeleted = true;
            user.IsActive = false;
            user.ModifiedById = currentUser?.Id;
            user.ModifiedByName = currentUser?.UserName;
            user.ModifiedAt = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);
        }

        public async Task DeleteUserFromAnyCenterAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new BadRequestException("User id cannot be null.");

            var user = await _userManager.Users
                .Include(u => u.Center)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new NotFoundException<string>(nameof(ApplicationUser), "id", userId);

            // Hard delete - permanently remove the user
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new FailedException($"User deletion failed: {errors}");
            }
        }

        public async Task RestoreUserAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new BadRequestException("User id cannot be null.");

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                throw new NotFoundException<string>(nameof(ApplicationUser), "id", userId);

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

        public async Task<List<UserResponseDTO>> GetDeletedUsersAsync(PaginationParamter paginationParamter)
        {
            var query = _userManager.Users.Include(u => u.Center).Where(u => u.IsDeleted);

            if (paginationParamter.PageSize != null && paginationParamter.PageNumber != null)
            {
                query = query
                    .Skip((paginationParamter.PageNumber.Value - 1) * paginationParamter.PageSize.Value)
                    .Take(paginationParamter.PageSize.Value);
            }

            var users = await query.ToListAsync();
            return _mapper.Map<List<UserResponseDTO>>(users);
        }

        public async Task<List<UserResponseDTO>> GetInactiveUsersAsync(PaginationParamter paginationParamter)
        {
            IQueryable<ApplicationUser> query = _userManager.Users.Include(u => u.Center).Where(u => !u.IsActive && !u.IsDeleted);

            if (paginationParamter.PageSize != null && paginationParamter.PageNumber != null)
            {
                query = query
                    .Skip((paginationParamter.PageNumber.Value - 1) * paginationParamter.PageSize.Value)
                    .Take(paginationParamter.PageSize.Value);
            }

            var users = await query.ToListAsync();
            return _mapper.Map<List<UserResponseDTO>>(users);
        }

        public async Task<List<UserResponseDTO>> GetActiveUsersAsync(PaginationParamter paginationParamter)
        {
            IQueryable<ApplicationUser> query = _userManager.Users.Include(u => u.Center).Where(u => u.IsActive && !u.IsDeleted);

            if (paginationParamter.PageSize != null && paginationParamter.PageNumber != null)
            {
                query = query
                    .Skip((paginationParamter.PageNumber.Value - 1) * paginationParamter.PageSize.Value)
                    .Take(paginationParamter.PageSize.Value);
            }

            var users = await query.ToListAsync();
            return _mapper.Map<List<UserResponseDTO>>(users);
        }

        public async Task DeleteUserAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new BadRequestException("User id cannot be null.");

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                throw new NotFoundException<string>(nameof(ApplicationUser), "id", userId);

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new FailedException($"User deletion failed: {errors}");
            }
        }

        public async Task ActivateUserAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new BadRequestException("User id cannot be null.");

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                throw new NotFoundException<string>(nameof(ApplicationUser), "id", userId);

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

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                throw new NotFoundException<string>(nameof(ApplicationUser), "id", userId);

            if (!user.IsActive)
                throw new ConflictException("User is already inactive.");
            var currentUser = await _userManager.FindByIdAsync(_tenantContext.GetCurrentUserId());
            user.IsActive = false;
            user.ModifiedById = currentUser?.Id;
            user.ModifiedByName = currentUser?.UserName;
            user.ModifiedAt = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);
        }

        public async Task UpdateUserRoleAsync(string userId, UpdateUserRoleDTO updateUserRoleDTO)
        {
            if (string.IsNullOrEmpty(userId))
                throw new BadRequestException("User id cannot be null.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new NotFoundException<string>(nameof(ApplicationUser), "id", userId);

            // Get current roles before updating
            var currentRoles = await _userManager.GetRolesAsync(user);
            
            // Remove all current roles
            if (currentRoles.Any())
                await _userManager.RemoveFromRolesAsync(user, currentRoles);

            // Add new role
            var result = await _userManager.AddToRoleAsync(user, updateUserRoleDTO.Role.ToString());
            
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new BadRequestException($"Role update failed: {errors}");
            }

            // Refresh the sign-in if the user is currently signed in
            // This updates the authentication cookie with the new role claims
            await _signInManager.RefreshSignInAsync(user);
        }

        #endregion User Management

        #region Statistics

        // Get overall system statistics (total centers, active/inactive centers, total users, active/inactive users)
        public async Task<SystemStatisticsDTO> GetSystemStatisticsAsync()
        {
            // Centers
            var totalCenters = await _unitOfWork.Centers.GetTotalCountAsync();
            var nonDeletedCenters = await _unitOfWork.Centers.GetNonDeletedCountAsync();
            var nonDeletedCentersList = await _unitOfWork.Centers.GetNonDeletedCentersAsync(
                new PaginationParamter { PageSize = 200 });
            var activeCenters = nonDeletedCentersList.Count(c => c.IsActive);

            // Users
            var totalUsers = await _userManager.Users.CountAsync();
            var activeUsers = await _userManager.Users.CountAsync(u => u.IsActive && !u.IsDeleted);

            // Teachers
            var totalTeachers = await GetTotalTeachersCountAsync();
            var deletedTeachers = await GetDeletedTeachersCountAsync();

            // Students
            var totalStudents = await GetTotalStudentsCountAsync();
            var deletedStudents = await GetDeletedStudentsCountAsync();

            // Courses
            var totalCourses = await GetTotalCoursesCountAsync();
            var deletedCourses = await GetDeletedCoursesCountAsync();

            // Classrooms
            var totalClassrooms = await GetTotalClassroomsCountAsync();
            var deletedClassrooms = await GetDeletedClassroomsCountAsync();

            return new SystemStatisticsDTO
            {
                // Centers
                TotalCenters = nonDeletedCenters,
                ActiveCenters = activeCenters,
                InactiveCenters = nonDeletedCenters - activeCenters,
                
                // Users
                TotalUsers = totalUsers,
                ActiveUsers = activeUsers,
                InactiveUsers = totalUsers - activeUsers,
                
                // Teachers
                TotalTeachers = totalTeachers,
                DeletedTeachers = deletedTeachers,
                
                // Students
                TotalStudents = totalStudents,
                DeletedStudents = deletedStudents,
                
                // Courses
                TotalCourses = totalCourses,
                DeletedCourses = deletedCourses,
                
                // Classrooms
                TotalClassrooms = totalClassrooms,
                DeletedClassrooms = deletedClassrooms
            };
        }

        #endregion Statistics

        #region System-Wide Monitoring

        public async Task<int> GetTotalTeachersCountAsync()
        {
            var teachers = await _unitOfWork.Teachers.GetAllAsync(new PaginationParamter { PageSize = int.MaxValue, PageNumber = 1 });
            return teachers.Count();
        }

        public async Task<int> GetTotalStudentsCountAsync()
        {
            var students = await _unitOfWork.Students.GetAllAsync(new PaginationParamter { PageSize = int.MaxValue, PageNumber = 1 });
            return students.Count();
        }

        public async Task<int> GetTotalCoursesCountAsync()
        {
            var courses = await _unitOfWork.Courses.GetAllAsync(new PaginationParamter { PageSize = int.MaxValue, PageNumber = 1 });
            return courses.Count();
        }

        public async Task<int> GetTotalClassroomsCountAsync()
        {
            var classrooms = await _unitOfWork.Classrooms.GetAllAsync(new PaginationParamter { PageSize = int.MaxValue, PageNumber = 1 });
            return classrooms.Count();
        }

        public async Task<int> GetDeletedTeachersCountAsync()
        {
            var teachers = await _unitOfWork.Teachers.GetAllAsync(new PaginationParamter { PageSize = int.MaxValue, PageNumber = 1 });
            return teachers.Count(t => t.IsDeleted);
        }

        public async Task<int> GetDeletedStudentsCountAsync()
        {
            var students = await _unitOfWork.Students.GetAllAsync(new PaginationParamter { PageSize = int.MaxValue, PageNumber = 1 });
            return students.Count(s => s.IsDeleted);
        }

        public async Task<int> GetDeletedCoursesCountAsync()
        {
            var courses = await _unitOfWork.Courses.GetAllAsync(new PaginationParamter { PageSize = int.MaxValue, PageNumber = 1 });
            return courses.Count(c => c.IsDeleted);
        }

        public async Task<int> GetDeletedClassroomsCountAsync()
        {
            var classrooms = await _unitOfWork.Classrooms.GetAllAsync(new PaginationParamter { PageSize = int.MaxValue, PageNumber = 1 });
            return classrooms.Count(c => c.IsDeleted);
        }

        #endregion System-Wide Monitoring

        #region Teacher Management

        public async Task RestoreTeacherAsync(int teacherId)
        {
            var teacher = await _unitOfWork.Teachers.GetByIdAsync(teacherId);
            if (teacher == null)
                throw new NotFoundException<int>(nameof(teacher), "teacher", teacherId);

            teacher.IsDeleted = false;
            _unitOfWork.Teachers.UpdateAsync(teacher);
            await _unitOfWork.SaveChangesAsync();
        }

        #endregion

        #region Course Management

        public async Task RestoreCourseAsync(int courseId)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(courseId);
            if (course == null)
                throw new NotFoundException<int>(nameof(course), "course", courseId);

            course.IsDeleted = false;
            _unitOfWork.Courses.UpdateAsync(course);
            await _unitOfWork.SaveChangesAsync();
        }

        #endregion

        #region Classroom Management

        public async Task RestoreClassroomAsync(int classroomId)
        {
            var classroom = await _unitOfWork.Classrooms.GetByIdAsync(classroomId);
            if (classroom == null)
                throw new NotFoundException<int>(nameof(classroom), "classroom", classroomId);

            classroom.IsDeleted = false;
            _unitOfWork.Classrooms.UpdateAsync(classroom);
            await _unitOfWork.SaveChangesAsync();
        }

        #endregion

        #region Session Management

        public async Task RestoreSessionAsync(int sessionId)
        {
            var session = await _unitOfWork.Sessions.GetByIdAsync(sessionId);
            if (session == null)
                throw new NotFoundException<int>(nameof(session), "session", sessionId);

            session.IsDeleted = false;
            _unitOfWork.Sessions.UpdateAsync(session);
            await _unitOfWork.SaveChangesAsync();
        }

        #endregion

        #region Exam Management

        public async Task RestoreExamAsync(int examId)
        {
            var exam = await _unitOfWork.Exams.GetByIdAsync(examId);
            if (exam == null)
                throw new NotFoundException<int>(nameof(exam), "exam", examId);

            exam.IsDeleted = false;
            _unitOfWork.Exams.UpdateAsync(exam);
            await _unitOfWork.SaveChangesAsync();
        }

        #endregion

        #region ExamResult Management

        public async Task RestoreExamResultAsync(int examResultId)
        {
            var examResult = await _unitOfWork.ExamResults.GetByIdAsync(examResultId);
            if (examResult == null)
                throw new NotFoundException<int>(nameof(examResult), "examResult", examResultId);

            examResult.IsDeleted = false;
            _unitOfWork.ExamResults.UpdateAsync(examResult);
            await _unitOfWork.SaveChangesAsync();
        }

        #endregion

        #region Invoice Management

        public async Task RestoreInvoiceAsync(int invoiceId)
        {
            var invoice = await _unitOfWork.Invoices.GetByIdAsync(invoiceId);
            if (invoice == null)
                throw new NotFoundException<int>(nameof(invoice), "invoice", invoiceId);

            invoice.IsDeleted = false;
            _unitOfWork.Invoices.UpdateAsync(invoice);
            await _unitOfWork.SaveChangesAsync();
        }

        #endregion

        #region Item Management

        public async Task RestoreItemAsync(int itemId)
        {
            var item = await _unitOfWork.Items.GetByIdAsync(itemId);
            if (item == null)
                throw new NotFoundException<int>(nameof(item), "item", itemId);

            item.IsDeleted = false;
            _unitOfWork.Items.UpdateAsync(item);
            await _unitOfWork.SaveChangesAsync();
        }

        #endregion

        #region TeacherCourse Management

        public async Task RestoreTeacherCourseAsync(int teacherCourseId)
        {
            var teacherCourse = await _unitOfWork.TeacherCourses.GetByIdAsync(teacherCourseId);
            if (teacherCourse == null)
                throw new NotFoundException<int>(nameof(teacherCourse), "teacherCourse", teacherCourseId);

            teacherCourse.IsDeleted = false;
            _unitOfWork.TeacherCourses.UpdateAsync(teacherCourse);
            await _unitOfWork.SaveChangesAsync();
        }

        #endregion

        #region TeacherItem Management

        public async Task RestoreTeacherItemAsync(int teacherItemId)
        {
            var teacherItem = await _unitOfWork.TeacherItems.GetByIdAsync(teacherItemId);
            if (teacherItem == null)
                throw new NotFoundException<int>(nameof(teacherItem), "teacherItem", teacherItemId);

            teacherItem.IsDeleted = false;
            _unitOfWork.TeacherItems.UpdateAsync(teacherItem);
            await _unitOfWork.SaveChangesAsync();
        }

        #endregion

        #region Enrollment Management

        public async Task RestoreEnrollmentAsync(int enrollmentId)
        {
            var enrollment = await _unitOfWork.Enrollments.GetByIdAsync(enrollmentId);
            if (enrollment == null)
                throw new NotFoundException<int>(nameof(enrollment), "enrollment", enrollmentId);

            enrollment.IsDeleted = false;
            _unitOfWork.Enrollments.UpdateAsync(enrollment);
            await _unitOfWork.SaveChangesAsync();
        }

        #endregion

        #region Student Management

        public async Task RestoreStudentAsync(int studentId)
        {
            var student = await _unitOfWork.Students.GetByIdAsync(studentId);
            if (student == null)
                throw new NotFoundException<int>(nameof(student), "student", studentId);

            student.IsDeleted = false;
            _unitOfWork.Students.UpdateAsync(student);
            await _unitOfWork.SaveChangesAsync();
        }

        #endregion
    }
}