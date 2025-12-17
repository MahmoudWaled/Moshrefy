using Microsoft.EntityFrameworkCore;
using Moshrefy.Application.Interfaces.IRepositories;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Paramter;
using Moshrefy.infrastructure.Data;
using Moshrefy.infrastructure.Repositories.GenericRepository;

namespace Moshrefy.infrastructure.Repositories
{
    public class CenterRepository(AppDbContext appDbContext) : GenericRepository<Center, int>(appDbContext), ICenterRepository
    {
        private readonly AppDbContext appDbContext = appDbContext;

        public async Task<IEnumerable<Center>> GetNonDeletedCentersAsync(PaginationParamter paginationParamter)
        {
            var query = appDbContext.Set<Center>()
                .Where(c => !c.IsDeleted)
                .OrderByDescending(c => c.CreatedAt);

            // Only apply pagination if both parameters are provided
            if (paginationParamter.PageNumber.HasValue && paginationParamter.PageSize.HasValue)
            {
                var pageNumber = paginationParamter.PageNumber.Value;
                var pageSize = paginationParamter.PageSize.Value;
                query = (IOrderedQueryable<Center>)query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Center>> GetActiveCentersAsync(PaginationParamter paginationParamter)
        {
            var pageNumber = paginationParamter.PageNumber ?? 1;
            var pageSize = paginationParamter.PageSize ?? 25;

            return await appDbContext.Set<Center>()
                .Where(c => c.IsActive && !c.IsDeleted)
                .OrderByDescending(c => c.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Center>> GetInactiveCentersAsync(PaginationParamter paginationParamter)
        {
            var pageNumber = paginationParamter.PageNumber ?? 1;
            var pageSize = paginationParamter.PageSize ?? 25;

            return await appDbContext.Set<Center>()
                .Where(c => !c.IsActive && !c.IsDeleted)
                .OrderByDescending(c => c.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Center>> GetDeletedCentersAsync(PaginationParamter paginationParamter)
        {
            var pageNumber = paginationParamter.PageNumber ?? 1;
            var pageSize = paginationParamter.PageSize ?? 25;

            return await appDbContext.Set<Center>()
                .Where(c => c.IsDeleted)
                .OrderByDescending(c => c.ModifiedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Center>> GetByName(string centerName)
        {
            return await appDbContext.Set<Center>()
                .Where(c => c.Name.Contains(centerName))
                .ToListAsync();
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await appDbContext.Set<Center>().CountAsync();
        }

        public async Task<int> GetNonDeletedCountAsync()
        {
            return await appDbContext.Set<Center>()
                .Where(c => !c.IsDeleted)
                .CountAsync();
        }

        public async Task<int> GetDeletedCountAsync()
        {
            return await appDbContext.Set<Center>()
                .Where(c => c.IsDeleted)
                .CountAsync();
        }
    }
}