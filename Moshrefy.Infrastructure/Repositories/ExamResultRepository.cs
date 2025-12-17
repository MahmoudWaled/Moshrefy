using Microsoft.EntityFrameworkCore;
using Moshrefy.Application.Interfaces.IRepositories;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Enums;
using Moshrefy.Domain.Paramter;
using Moshrefy.infrastructure.Data;
using Moshrefy.infrastructure.Repositories.GenericRepository;
using System.Linq.Expressions;

namespace Moshrefy.infrastructure.Repositories
{
    public class ExamResultRepository(AppDbContext appDbContext) : GenericRepository<ExamResult, int>(appDbContext), IExamResultRepository
    {
        // Predicate overload for proper server-side filtering
        public new async Task<IEnumerable<ExamResult>> GetAllAsync(Expression<Func<ExamResult, bool>> predicate, PaginationParamter paginationParamter)
        {
            var pageNumber = paginationParamter.PageNumber ?? 1;
            var pageSize = paginationParamter.PageSize ?? 25;

            return await appDbContext.Set<ExamResult>()
                .Include(er => er.Exam)
                .Include(er => er.Student)
                .Where(predicate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<ExamResult>> GetExamResultsByStatusAsync(ExamResultStatus resultStatus)
        {
            return await appDbContext.Set<ExamResult>()
                    .Where(er => er.ExamResultStatus == resultStatus)
                    .ToListAsync();
        }
    }
}
