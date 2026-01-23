using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moshrefy.Application.DTOs.Center;
using Moshrefy.Application.DTOs.Statistics;
using Moshrefy.Application.DTOs.User;
using Moshrefy.Application.DTOs.Common;
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

        //public async Task<DataTableResponse<CenterResponseDTO>> GetCentersDataTableAsync(DataTableRequest request)
        //{
        //    var paginationParams = new PaginationParamter
        //    {
        //        PageNumber = request.PageNumber,
        //        PageSize = request.PageSize
        //    };

        //    List<CenterResponseDTO> centersDTO;
        //    int totalRecords;
        //    int filteredRecords;

        //    // 1. Initial Data Fetch based on custom filters (Active/Deleted/All)
        //    if (request.FilterDeleted == "deleted")
        //    {
        //        centersDTO = await GetDeletedCentersAsync(paginationParams);
        //        totalRecords = await GetDeletedCentersCountAsync();
        //        filteredRecords = totalRecords;
        //    }
        //    else if (request.FilterDeleted == "active") // This seems to map to "NonDeleted" in controller logic
        //    {
        //        centersDTO = await GetNonDeletedCentersAsync(paginationParams);
        //        totalRecords = await GetNonDeletedCentersCountAsync();
        //        filteredRecords = totalRecords;
        //    }
        //    else
        //    {
        //        // Show all
        //        centersDTO = await GetAllCentersAsync(paginationParams);
        //        totalRecords = await GetTotalCentersCountAsync();
        //        filteredRecords = totalRecords;
        //    }

        //    // 2. Apply Custom Active/Inactive Filter (InMemory for now, as per controller logic)
        //    if (!string.IsNullOrEmpty(request.ActiveFilter) && request.ActiveFilter != "all")
        //    {
        //        if (request.ActiveFilter == "active")
        //        {
        //            centersDTO = centersDTO.Where(c => c.IsActive).ToList();
        //        }
        //        else if (request.ActiveFilter == "inactive")
        //        {
        //            centersDTO = centersDTO.Where(c => !c.IsActive).ToList();
        //        }
        //        filteredRecords = centersDTO.Count;
        //    }

        //    var centersVM = centersDTO; // Working with DTOs directly

        //    // 3. Apply Search (InMemory)
        //    if (!string.IsNullOrEmpty(request.SearchValue))
        //    {
        //        var searchValue = request.SearchValue;
        //        centersVM = centersVM.Where(c =>
        //            c.Name.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ||
        //            (c.Email != null && c.Email.Contains(searchValue, StringComparison.OrdinalIgnoreCase)) ||
        //            c.Phone.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ||
        //            c.Address.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ||
        //            c.Id.ToString().Contains(searchValue)
        //        ).ToList();

        //        filteredRecords = centersVM.Count;
        //    }

        //    // 4. Advanced Search
        //    if (!string.IsNullOrWhiteSpace(request.CenterName))
        //    {
        //        centersVM = centersVM.Where(c => c.Name.Contains(request.CenterName, StringComparison.OrdinalIgnoreCase)).ToList();
        //    }
        //    if (!string.IsNullOrWhiteSpace(request.Email))
        //    {
        //        centersVM = centersVM.Where(c => c.Email != null && c.Email.Contains(request.Email, StringComparison.OrdinalIgnoreCase)).ToList();
        //    }
        //     if (!string.IsNullOrWhiteSpace(request.CreatedByName))
        //    {
        //        centersVM = centersVM.Where(c => c.CreatedByName != null && c.CreatedByName.Contains(request.CreatedByName, StringComparison.OrdinalIgnoreCase)).ToList();
        //    }

        //     // Note: AdminName logic requires extra fetching, which is expensive. 
        //     // The controller logic fetched it for ALL items in advanced search, but here we might want to optimize.
        //     // For now, mirroring controller logic but applying it only if needed or after filtering.
        //     // However, to sort/filter by AdminName, we need it populated.
        //     if (!string.IsNullOrWhiteSpace(request.AdminName) || request.SortColumnName == "AdminName" || !string.IsNullOrEmpty(request.SearchValue))
        //     {
        //         foreach (var center in centersVM)
        //         {
        //             try
        //             {
        //                var adminUser = await GetCenterAdminAsync(center.Id);
        //                center.AdminName = adminUser?.Name;
        //             }
        //             catch
        //             {
        //                 center.AdminName = null;
        //             }
        //         }

        //         if (!string.IsNullOrWhiteSpace(request.AdminName))
        //         {
        //             centersVM = centersVM.Where(c => c.AdminName != null && c.AdminName.Contains(request.AdminName, StringComparison.OrdinalIgnoreCase)).ToList();
        //         }
                 
        //         // Re-apply global search if it involves AdminName (as done in controller)
        //         if (!string.IsNullOrEmpty(request.SearchValue))
        //         {
        //             // This is tricky because we already filtered by other fields. 
        //             // Ideally AdminName search should be part of the main search block, but we didn't have the data then.
        //             // Because this is InMemory pagination (partially), we can't easily go back to DB.
        //             // Controller logic was also mixing DB pagination with InMemory filtering which is risky for large datasets.
        //             // We will keep it as is for refactoring parity.
        //         }
        //     }

        //    // Update filtered count after advanced filters
        //    filteredRecords = centersVM.Count;

        //    // 5. Apply Sorting
        //    if (!string.IsNullOrEmpty(request.SortColumnName) && !string.IsNullOrEmpty(request.SortDirection))
        //    {
        //         var isAsc = request.SortDirection.ToLower() == "asc";
        //         centersVM = request.SortColumnName.ToLower() switch
        //         {
        //             "id" => isAsc ? centersVM.OrderBy(c => c.Id).ToList() : centersVM.OrderByDescending(c => c.Id).ToList(),
        //             "name" => isAsc ? centersVM.OrderBy(c => c.Name).ToList() : centersVM.OrderByDescending(c => c.Name).ToList(),
        //             "email" => isAsc ? centersVM.OrderBy(c => c.Email).ToList() : centersVM.OrderByDescending(c => c.Email).ToList(),
        //             "phone" => isAsc ? centersVM.OrderBy(c => c.Phone).ToList() : centersVM.OrderByDescending(c => c.Phone).ToList(),
        //             "address" => isAsc ? centersVM.OrderBy(c => c.Address).ToList() : centersVM.OrderByDescending(c => c.Address).ToList(),
        //             "isactive" => isAsc ? centersVM.OrderBy(c => c.IsActive).ToList() : centersVM.OrderByDescending(c => c.IsActive).ToList(),
        //             "createdbyname" => isAsc ? centersVM.OrderBy(c => c.CreatedByName).ToList() : centersVM.OrderByDescending(c => c.CreatedByName).ToList(),
        //             "adminname" => isAsc ? centersVM.OrderBy(c => c.AdminName).ToList() : centersVM.OrderByDescending(c => c.AdminName).ToList(),
        //             "createdat" => isAsc ? centersVM.OrderBy(c => c.CreatedAt).ToList() : centersVM.OrderByDescending(c => c.CreatedAt).ToList(),
        //             _ => centersVM.OrderBy(c => c.Name).ToList()
        //         };
        //    }

        //    // Note: The controller logic for `AdvancedSearchCenters` fetched *ALL* centers then paginated in memory.
        //    // The `GetCentersData` (normal table) fetched *PAGINATED* from DB then filtered in memory (which is buggy if filters reduce count).
        //    // For this refactor, I am preserving the behavior where we return the processing result.
        //    // However, since we return a subset, if we did in-memory filtering on a page, we might return less than PageSize.
        //    // Correct approach implies standardizing this, but avoiding logic change risk.
            
        //    return new DataTableResponse<CenterResponseDTO>
        //    {
        //        Draw = request.Draw,
        //        RecordsTotal = totalRecords,
        //        RecordsFiltered = filteredRecords,
        //        Data = centersVM
        //    };
        //}

        #region User DataTables

        // Get users for DataTables
        public async Task<DataTableResponse<UserResponseDTO>> GetUsersDataTableAsync(DataTableRequest request)
        {
             var paginationParams = new PaginationParameter
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };

            List<UserResponseDTO> usersDTO;
            int totalRecords;
            int filteredRecords;

            // 1. Initial Fetch
            if (!string.IsNullOrEmpty(request.FilterRole) && request.FilterRole != "all")
            {
                usersDTO = await GetUsersByRoleAsync(request.FilterRole, paginationParams);
                totalRecords = await GetTotalUsersCountAsync(); // Approx
            }
            else if (request.FilterDeleted == "deleted")
            {
                usersDTO = await GetDeletedUsersAsync(paginationParams);
                totalRecords = await GetDeletedUsersCountAsync();
            }
            else
            {
                // Default to non-deleted users logic if implied (controller filterDeleted != "deleted")
                // But controller called GetAllUsersAsync which returns mix unless filtered?
                // Checking controller: "usersDTO = usersDTO.Where(u => !u.IsDeleted).ToList();"
                // So GetAllUsersAsync returns all, then we filter in memory.
                usersDTO = await GetAllUsersAsync(paginationParams);
                usersDTO = usersDTO.Where(u => !u.IsDeleted).ToList();
                totalRecords = await GetTotalUsersCountAsync() - await GetDeletedUsersCountAsync();
            }
            
            filteredRecords = totalRecords;

             // 2. Active/Inactive Filter
             if (request.FilterDeleted != "deleted")
             {
                 if (!string.IsNullOrEmpty(request.FilterStatus) && request.FilterStatus != "all")
                 {
                     if (request.FilterStatus == "active")
                     {
                         usersDTO = usersDTO.Where(u => u.IsActive).ToList();
                         totalRecords = await GetActiveUsersCountAsync();
                         filteredRecords = totalRecords;
                     }
                     else if (request.FilterStatus == "inactive")
                     {
                         usersDTO = usersDTO.Where(u => !u.IsActive).ToList();
                         totalRecords = await GetInactiveUsersCountAsync();
                         filteredRecords = totalRecords;
                     }
                 }
             }

             // 3. Search
             if (!string.IsNullOrEmpty(request.SearchValue))
             {
                 var searchValue = request.SearchValue;
                 usersDTO = usersDTO.Where(u =>
                    u.Name.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ||
                    u.UserName.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ||
                    (u.Email != null && u.Email.Contains(searchValue, StringComparison.OrdinalIgnoreCase)) ||
                    (u.PhoneNumber != null && u.PhoneNumber.Contains(searchValue, StringComparison.OrdinalIgnoreCase)) ||
                    (u.CenterName != null && u.CenterName.Contains(searchValue, StringComparison.OrdinalIgnoreCase)) ||
                    (u.RoleName != null && u.RoleName.Contains(searchValue, StringComparison.OrdinalIgnoreCase)) ||
                    u.Id.Contains(searchValue)
                 ).ToList();
                 
                 filteredRecords = usersDTO.Count;
             }

             // 4. Sorting
             if (!string.IsNullOrEmpty(request.SortColumnName) && !string.IsNullOrEmpty(request.SortDirection))
             {
                 var isAsc = request.SortDirection.ToLower() == "asc";
                 usersDTO = request.SortColumnName.ToLower() switch
                 {
                     "name" => isAsc ? usersDTO.OrderBy(u => u.Name).ToList() : usersDTO.OrderByDescending(u => u.Name).ToList(),
                     "username" => isAsc ? usersDTO.OrderBy(u => u.UserName).ToList() : usersDTO.OrderByDescending(u => u.UserName).ToList(),
                     "email" => isAsc ? usersDTO.OrderBy(u => u.Email).ToList() : usersDTO.OrderByDescending(u => u.Email).ToList(),
                     "phonenumber" => isAsc ? usersDTO.OrderBy(u => u.PhoneNumber).ToList() : usersDTO.OrderByDescending(u => u.PhoneNumber).ToList(),
                     "centername" => isAsc ? usersDTO.OrderBy(u => u.CenterName).ToList() : usersDTO.OrderByDescending(u => u.CenterName).ToList(),
                     "rolename" => isAsc ? usersDTO.OrderBy(u => u.RoleName).ToList() : usersDTO.OrderByDescending(u => u.RoleName).ToList(),
                     _ => usersDTO.OrderBy(u => u.Name).ToList()
                 };
             }

            return new DataTableResponse<UserResponseDTO>
            {
                Draw = request.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = filteredRecords,
                Data = usersDTO
            };
        }

        #endregion

        #region User Management

        // Create admin for center
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

        public async Task<List<UserResponseDTO>> GetAllUsersAsync(PaginationParameter paginationParamter)
        {
            IQueryable<ApplicationUser> query = _userManager.Users.Include(u => u.Center);
           
                query = query
                    .Skip((paginationParamter.PageNumber - 1) * paginationParamter.PageSize)
                    .Take(paginationParamter.PageSize);
            
            var users = await query.ToListAsync();
            return _mapper.Map<List<UserResponseDTO>>(users);
        }

        public async Task<List<UserResponseDTO>> GetUsersByCenterIdAsync(int centerId, PaginationParameter paginationParamter)
        {
            if (centerId <= 0)
                throw new BadRequestException("Invalid center id.");

            IQueryable<ApplicationUser> query = _userManager.Users.Include(u => u.Center).Where(u => u.CenterId == centerId);

                query = query
                    .Skip((paginationParamter.PageNumber - 1) * paginationParamter.PageSize)
                    .Take(paginationParamter.PageSize);
            

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

        public async Task<List<UserResponseDTO>> GetUsersByRoleAsync(string roleName, PaginationParameter paginationParamter)
        {
            if (string.IsNullOrEmpty(roleName))
                throw new BadRequestException("Role name cannot be null.");

            if (!Enum.TryParse<RolesNames>(roleName, out var parsedRole))
                throw new BadRequestException("Invalid role name.");

            var usersInRole = await _userManager.GetUsersInRoleAsync(parsedRole.ToString());

            var paginatedUsers = usersInRole.AsEnumerable();
           
                paginatedUsers = paginatedUsers
                    .Skip((paginationParamter.PageNumber - 1) * paginationParamter.PageSize)
                    .Take(paginationParamter.PageSize);

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

        public async Task<List<UserResponseDTO>> GetDeletedUsersAsync(PaginationParameter paginationParamter)
        {
            var query = _userManager.Users.Include(u => u.Center).Where(u => u.IsDeleted);

                query = query
                    .Skip((paginationParamter.PageNumber - 1) * paginationParamter.PageSize)
                    .Take(paginationParamter.PageSize);

            var users = await query.ToListAsync();
            return _mapper.Map<List<UserResponseDTO>>(users);
        }

        public async Task<List<UserResponseDTO>> GetInactiveUsersAsync(PaginationParameter paginationParamter)
        {
            IQueryable<ApplicationUser> query = _userManager.Users.Include(u => u.Center).Where(u => !u.IsActive && !u.IsDeleted);

                query = query
                    .Skip((paginationParamter.PageNumber - 1) * paginationParamter.PageSize)
                    .Take(paginationParamter.PageSize);

            var users = await query.ToListAsync();
            return _mapper.Map<List<UserResponseDTO>>(users);
        }

        public async Task<List<UserResponseDTO>> GetActiveUsersAsync(PaginationParameter paginationParamter)
        {
            IQueryable<ApplicationUser> query = _userManager.Users.Include(u => u.Center).Where(u => u.IsActive && !u.IsDeleted);

                query = query
                    .Skip((paginationParamter.PageNumber - 1) * paginationParamter.PageSize)
                    .Take(paginationParamter.PageSize);

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

        #endregion

        #region Statistics

        // Get system statistics
        public async Task<SystemStatisticsDTO> GetSystemStatisticsAsync()
        {
            // Centers
            var totalCenters = await _unitOfWork.Centers.GetTotalCountAsync();
            var nonDeletedCenters = await _unitOfWork.Centers.GetNonDeletedCountAsync();
            var nonDeletedCentersList = await _unitOfWork.Centers.GetNonDeletedPagedAsync(
                new PaginationParameter { PageSize = 200 });
            var activeCenters = nonDeletedCentersList.centers.Count(c => c.IsActive);

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

        #region System-Wide Monitoring (Counts)

        public async Task<int> GetTotalTeachersCountAsync()
        {
            var teachers = await _unitOfWork.Teachers.GetAllAsync(new PaginationParameter { PageSize = int.MaxValue, PageNumber = 1 });
            return teachers.Count();
        }

        public async Task<int> GetTotalStudentsCountAsync()
        {
            var students = await _unitOfWork.Students.GetAllAsync(new PaginationParameter { PageSize = int.MaxValue, PageNumber = 1 });
            return students.Count();
        }

        public async Task<int> GetTotalCoursesCountAsync()
        {
            var courses = await _unitOfWork.Courses.GetAllAsync(new PaginationParameter { PageSize = int.MaxValue, PageNumber = 1 });
            return courses.Count();
        }

        public async Task<int> GetTotalClassroomsCountAsync()
        {
            var classrooms = await _unitOfWork.Classrooms.GetAllAsync(new PaginationParameter { PageSize = int.MaxValue, PageNumber = 1 });
            return classrooms.Count();
        }

        public async Task<int> GetDeletedTeachersCountAsync()
        {
            var teachers = await _unitOfWork.Teachers.GetAllAsync(new PaginationParameter { PageSize = int.MaxValue, PageNumber = 1 });
            return teachers.Count(t => t.IsDeleted);
        }

        public async Task<int> GetDeletedStudentsCountAsync()
        {
            var students = await _unitOfWork.Students.GetAllAsync(new PaginationParameter { PageSize = int.MaxValue, PageNumber = 1 });
            return students.Count(s => s.IsDeleted);
        }

        public async Task<int> GetDeletedCoursesCountAsync()
        {
            var courses = await _unitOfWork.Courses.GetAllAsync(new PaginationParameter { PageSize = int.MaxValue, PageNumber = 1 });
            return courses.Count(c => c.IsDeleted);
        }

        public async Task<int> GetDeletedClassroomsCountAsync()
        {
            var classrooms = await _unitOfWork.Classrooms.GetAllAsync(new PaginationParameter { PageSize = int.MaxValue, PageNumber = 1 });
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
            _unitOfWork.Teachers.Update(teacher);
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
            _unitOfWork.Courses.Update(course);
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
            _unitOfWork.Classrooms.Update(classroom);
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
            _unitOfWork.Sessions.Update(session);
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
            _unitOfWork.Exams.Update(exam);
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
            _unitOfWork.ExamResults.Update(examResult);
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
            _unitOfWork.Invoices.Update(invoice);
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
            _unitOfWork.Items.Update(item);
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
            _unitOfWork.TeacherCourses.Update(teacherCourse);
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
            _unitOfWork.TeacherItems.Update(teacherItem);
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
            _unitOfWork.Enrollments.Update(enrollment);
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
            _unitOfWork.Students.Update(student);
            await _unitOfWork.SaveChangesAsync();
        }

        #endregion
    }
}