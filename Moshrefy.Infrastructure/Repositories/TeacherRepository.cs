using Microsoft.EntityFrameworkCore;
using Moshrefy.Application.Interfaces.IRepositories;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Paramter;
using Moshrefy.infrastructure.Data;
using Moshrefy.infrastructure.Repositories.GenericRepository;
using System.Linq.Expressions;

namespace Moshrefy.infrastructure.Repositories
{
    public class TeacherRepository(AppDbContext appDbContext) : GenericRepository<Teacher, int>(appDbContext), ITeacherRepository
    {
        // Predicate overload for proper server-side filtering
        public new async Task<IEnumerable<Teacher>> GetAllAsync(Expression<Func<Teacher, bool>> predicate, PaginationParameter paginationParamter)
        {
            return await appDbContext.Set<Teacher>()
                .Where(predicate)
                .Skip((paginationParamter.PageNumber - 1) * paginationParamter.PageSize)
                .Take(paginationParamter.PageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Teacher>> GetActiveTeachersAsync()
        {
            return await appDbContext.Set<Teacher>()
                .Where(t => t.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Teacher>> GetByNameAsync(string teacherName)
        {
            return await appDbContext.Set<Teacher>()
                .Where(t => t.Name.Contains(teacherName))
                .ToListAsync();
        }

        public async Task<Teacher> GetByPhoneNumberAsync(string phoneNumber)
        {
            return await appDbContext.Set<Teacher>()
                .Where(t => t.Phone == phoneNumber)
                .FirstOrDefaultAsync();
        }
    }
}
