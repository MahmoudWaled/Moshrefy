using Microsoft.EntityFrameworkCore;
using Moshrefy.Application.Interfaces.IRepositories;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Paramter;
using Moshrefy.infrastructure.Data;
using Moshrefy.infrastructure.Repositories.GenericRepository;
using System.Linq.Expressions;

namespace Moshrefy.infrastructure.Repositories
{
    public class CourseRepository(AppDbContext appDbContext) : GenericRepository<Course, int>(appDbContext), ICourseRepository
    {
        // Override to include AcademicYear navigation property
        public new async Task<IEnumerable<Course>> GetAllAsync(PaginationParamter paginationParamter)
        {
            var pageNumber = paginationParamter.PageNumber ?? 1;
            var pageSize = paginationParamter.PageSize ?? 25;

            return await appDbContext.Set<Course>()
                .Include(c => c.AcademicYear)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // Override to include AcademicYear navigation property with filtering
        public new async Task<IEnumerable<Course>> GetAllAsync(Expression<Func<Course, bool>> predicate, PaginationParamter paginationParamter)
        {
            var pageNumber = paginationParamter.PageNumber ?? 1;
            var pageSize = paginationParamter.PageSize ?? 25;

            return await appDbContext.Set<Course>()
                .Include(c => c.AcademicYear)
                .Where(predicate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // Override to include AcademicYear navigation property
        public new async Task<Course?> GetByIdAsync(int id)
        {
            return await appDbContext.Set<Course>()
                .Include(c => c.AcademicYear)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Course>> GetByName(string courseName)
        {
            return await appDbContext.Set<Course>()
                .Include(c => c.AcademicYear)
                .Where(c => c.Name.Contains(courseName))
                .ToListAsync();
        }

        public async Task<Course?> GetByIdWithAcademicYearAsync(int id)
        {
            return await appDbContext.Set<Course>()
                .Include(c => c.AcademicYear)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
