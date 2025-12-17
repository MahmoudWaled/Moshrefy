using Microsoft.EntityFrameworkCore;
using Moshrefy.Application.Interfaces.IRepositories;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Paramter;
using Moshrefy.infrastructure.Data;
using Moshrefy.infrastructure.Repositories.GenericRepository;
using System.Linq.Expressions;

namespace Moshrefy.infrastructure.Repositories
{
    public class ClassroomRepository(AppDbContext appDbContext) : GenericRepository<Classroom, int>(appDbContext), IClassroomRepository
    {
        // Predicate overload for proper server-side filtering
        public new async Task<IEnumerable<Classroom>> GetAllAsync(Expression<Func<Classroom, bool>> predicate, PaginationParamter paginationParamter)
        {
            var pageNumber = paginationParamter.PageNumber ?? 1;
            var pageSize = paginationParamter.PageSize ?? 25;

            return await appDbContext.Set<Classroom>()
                .Where(predicate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Classroom>> GetByName(string classroomName)
        {
            return await appDbContext.Set<Classroom>()
                .Where(c => c.Name.Contains(classroomName))
                .ToListAsync();
        }
    }
}
