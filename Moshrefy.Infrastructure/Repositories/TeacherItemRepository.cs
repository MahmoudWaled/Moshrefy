using Microsoft.EntityFrameworkCore;
using Moshrefy.Application.Interfaces.IRepositories;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Paramter;
using Moshrefy.infrastructure.Data;
using Moshrefy.infrastructure.Repositories.GenericRepository;
using System.Linq.Expressions;

namespace Moshrefy.infrastructure.Repositories
{
    public class TeacherItemRepository(AppDbContext appDbContext) : GenericRepository<TeacherItem, int>(appDbContext), ITeacherItemRepository
    {
        // Predicate overload for proper server-side filtering
        public new async Task<IEnumerable<TeacherItem>> GetAllAsync(Expression<Func<TeacherItem, bool>> predicate, PaginationParamter paginationParamter)
        {
            var pageNumber = paginationParamter.PageNumber ?? 1;
            var pageSize = paginationParamter.PageSize ?? 25;

            return await appDbContext.Set<TeacherItem>()
                .Include(ti => ti.Teacher)
                .Include(ti => ti.Item)
                .Where(predicate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<TeacherItem>> GetByTeacherIdAsync(int teacherId)
        {
            return await appDbContext.Set<TeacherItem>()
                .Where(ti => ti.TeacherId == teacherId)
                .ToListAsync();
        }

        public async Task<IEnumerable<TeacherItem>> GetByTeacherNameAsync(string teacherName)
        {
            return await appDbContext.Set<TeacherItem>()
                .Where(ti => ti.Teacher.Name.Contains(teacherName))
                .ToListAsync();
        }

        public async Task<IEnumerable<TeacherItem>> GetByTeacherPhoneAsync(string teacherPhone)
        {
            return await appDbContext.Set<TeacherItem>()
                .Where(ti => ti.Teacher.Phone.Contains(teacherPhone))
                .ToListAsync();
        }
    }
}
