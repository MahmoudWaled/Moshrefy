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
        public async Task<IEnumerable<Center>> GetNonDeletedCentersAsync(PaginationParamter paginationParamter)
        {
            return await appDbContext.Set<Center>()
               .Where(c => !c.IsDeleted)
               .OrderByDescending(c => c.ModifiedAt)
               .Skip((paginationParamter.PageNumber - 1) * paginationParamter.PageSize)
               .Take(paginationParamter.PageSize)
               .ToListAsync();
        }

        public async Task<IEnumerable<Center>> GetActiveCentersAsync(PaginationParamter paginationParamter)
        {

            return await appDbContext.Set<Center>()
                .Where(c => c.IsActive && !c.IsDeleted)
                .OrderByDescending(c => c.CreatedAt)
                .Skip((paginationParamter.PageNumber - 1) * paginationParamter.PageSize)
                .Take(paginationParamter.PageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Center>> GetInactiveCentersAsync(PaginationParamter paginationParamter)
        {
            return await appDbContext.Set<Center>()
                .Where(c => !c.IsActive && !c.IsDeleted)
                .OrderByDescending(c => c.CreatedAt)
                .Skip((paginationParamter.PageNumber - 1) * paginationParamter.PageSize)
                .Take(paginationParamter.PageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Center>> GetDeletedCentersAsync(PaginationParamter paginationParamter)
        {

            return await appDbContext.Set<Center>()
                .Where(c => c.IsDeleted)
                .OrderByDescending(c => c.ModifiedAt)
                .Skip((paginationParamter.PageNumber - 1) * paginationParamter.PageSize)
                .Take(paginationParamter.PageSize)
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

        public async Task<(IEnumerable<Center> Items, int TotalCount)> GetNonDeletedCentersPagedAsync(PaginationParamter paginationParamter)
        {
            var query = appDbContext.Set<Center>().Where(c => !c.IsDeleted);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(c => c.ModifiedAt)
                .Skip((paginationParamter.PageNumber - 1) * paginationParamter.PageSize)
                .Take(paginationParamter.PageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }
}