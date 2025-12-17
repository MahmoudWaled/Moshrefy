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
        public new async Task<IEnumerable<AcademicYear>> GetAllAsync(Expression<Func<AcademicYear, bool>> predicate, PaginationParamter paginationParamter)
        {
            var pageNumber = paginationParamter.PageNumber ?? 1;
            var pageSize = paginationParamter.PageSize ?? 25;

            return await appDbContext.Set<AcademicYear>()
                .Where(predicate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<AcademicYear>> GetByName(string academicYearName)
        {
            return await appDbContext.Set<AcademicYear>()
                .Where(ay => ay.Name.Contains(academicYearName))
                .ToListAsync();
        }
    }
}
