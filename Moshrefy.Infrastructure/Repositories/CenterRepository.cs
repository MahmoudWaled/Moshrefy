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

        public async Task<IEnumerable<Center>> GetActiveCentersAsync(PaginationParamter paginationParamter)
        {
            var pageNumber = paginationParamter.PageNumber ?? 1;
            var pageSize = paginationParamter.PageSize ?? 40;

            if (pageSize > 40)
                pageSize = 40;

            return await appDbContext.Set<Center>()
                .Where(c => c.IsActive && !c.IsDeleted)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Center>> GetInactiveCentersAsync(PaginationParamter paginationParamter)
        {
            var pageNumber = paginationParamter.PageNumber ?? 1;
            var pageSize = paginationParamter.PageSize ?? 40;

            if (pageSize > 40)
                pageSize = 40;

            return await appDbContext.Set<Center>()
                .Where(c => !c.IsActive && !c.IsDeleted)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Center>> GetDeletedCentersAsync(PaginationParamter paginationParamter)
        {
            var pageNumber = paginationParamter.PageNumber ?? 1;
            var pageSize = paginationParamter.PageSize ?? 40;

            if (pageSize > 40)
                pageSize = 40;

            return await appDbContext.Set<Center>()
                .Where(c => c.IsDeleted)
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
    }
}