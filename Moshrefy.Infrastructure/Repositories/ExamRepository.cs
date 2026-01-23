using Microsoft.EntityFrameworkCore;
using Moshrefy.Application.Interfaces.IRepositories;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Paramter;
using Moshrefy.infrastructure.Data;
using Moshrefy.infrastructure.Repositories.GenericRepository;
using System.Linq.Expressions;

namespace Moshrefy.infrastructure.Repositories
{
    public class ExamRepository(AppDbContext appDbContext) : GenericRepository<Exam, int>(appDbContext), IExamRepository
    {
        // Predicate overload for proper server-side filtering
        public new async Task<IEnumerable<Exam>> GetAllAsync(Expression<Func<Exam, bool>> predicate, PaginationParameter paginationParamter)
        {
            return await appDbContext.Set<Exam>()
                .Include(e => e.Course)
                .Where(predicate)
                .Skip((paginationParamter.PageNumber - 1) * paginationParamter.PageSize)
                .Take(paginationParamter.PageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Exam>> GetByDate(DateTime examDate)
        {
            return await appDbContext.Set<Exam>()
                .Where(e => e.Date == examDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Exam>> GetByName(string examName)
        {
            return await appDbContext.Set<Exam>()
                .Where(e => e.Name.Contains(examName))
                .ToListAsync();
        }
    }
}
