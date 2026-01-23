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

        public async Task<(IEnumerable<Center> centers, int TotalCount)> GetNonDeletedPagedAsync(PaginationParameter paginationParamter)
        {
            var query = appDbContext.Set<Center>().Where(c => !c.IsDeleted);

            var totalCount = await query.CountAsync();

            var centers = await query
                .OrderByDescending(c => c.ModifiedAt)
                .Skip((paginationParamter.PageNumber - 1) * paginationParamter.PageSize)
                .Take(paginationParamter.PageSize)
                .ToListAsync();

            return (centers, totalCount);
        }
        public async Task<(IEnumerable<Center> centers, int TotalCount)> GetActivePagedAsync(PaginationParameter paginationParamter)
        {
            var query = appDbContext.Set<Center>().Where(c=> !c.IsDeleted);
               
            var totalCount = await query.Where(c => c.IsActive).CountAsync();
            var centers = await query
                .Where(c => c.IsActive)
                .OrderByDescending(c => c.CreatedAt)
                .Skip((paginationParamter.PageNumber - 1) * paginationParamter.PageSize)
                .Take(paginationParamter.PageSize)
                .ToListAsync();
            return (centers, totalCount);
        }

        public async Task<(IEnumerable<Center> centers , int TotalCount)> GetInactivePagedAsync(PaginationParameter paginationParamter)
        {
            var query = appDbContext.Set<Center>().Where(c => !c.IsDeleted);
            var totalCount = await query.Where(c => !c.IsActive).CountAsync();
            var centers = await query
                .Where(c => !c.IsActive)
                .OrderByDescending(c => c.CreatedAt)
                .Skip((paginationParamter.PageNumber - 1) * paginationParamter.PageSize)
                .Take(paginationParamter.PageSize)
                .ToListAsync();
            return (centers, totalCount);
        }

        public async Task<(IEnumerable<Center> centers, int TotalCount)> GetDeletedPagedAsync(PaginationParameter paginationParamter)
        {
            var query = appDbContext.Set<Center>().Where(c => c.IsDeleted);
            var totalCount = await query.CountAsync();
            var centers = await query
                .OrderByDescending(c => c.ModifiedAt)
                .Skip((paginationParamter.PageNumber - 1) * paginationParamter.PageSize)
                .Take(paginationParamter.PageSize)
                .ToListAsync();

           return (centers, totalCount);
        }

        public async Task<IEnumerable<Center>> GetByNameAsync(string centerName)
        {
            return await appDbContext.Set<Center>()
                .Where(c => c.Name.Contains(centerName))
                .ToListAsync();
        }
        public async Task<Center?> GetByEmailAsync(string email)
        {
            return await appDbContext.Set<Center>()
                .FirstOrDefaultAsync(c => c.Email == email);

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