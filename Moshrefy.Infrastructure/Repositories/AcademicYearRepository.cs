using Microsoft.EntityFrameworkCore;
using Moshrefy.Application.Interfaces.IRepositories;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Paramter;
using Moshrefy.infrastructure.Data;
using Moshrefy.infrastructure.Repositories.GenericRepository;
using System.Linq.Expressions;

namespace Moshrefy.infrastructure.Repositories
{
    public class AcademicYearRepository(AppDbContext appDbContext) : GenericRepository<AcademicYear, int>(appDbContext), IAcademicYearRepository
    {
        // Predicate overload for proper server-side filtering
        public new async Task<IEnumerable<AcademicYear>> GetAllAsync(Expression<Func<AcademicYear, bool>> predicate, PaginationParameter paginationParamter)
        {
            
            return await appDbContext.Set<AcademicYear>()
                .Where(predicate)
                .Skip((paginationParamter.PageNumber - 1) * paginationParamter.PageSize)
                .Take(paginationParamter.PageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<AcademicYear>> GetByName(string academicYearName)
        {
            return await appDbContext.Set<AcademicYear>()
                .Where(ay => ay.Name.Contains(academicYearName))
                .ToListAsync();
        }

        public async Task<(IEnumerable<AcademicYear> Items, int TotalCount)> GetPagedAsync(int centerId, int pageNumber, int pageSize)
        {
            var query = appDbContext.Set<AcademicYear>()
                .Where(ay => ay.CenterId == centerId && !ay.IsDeleted);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(ay => ay.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }
}
