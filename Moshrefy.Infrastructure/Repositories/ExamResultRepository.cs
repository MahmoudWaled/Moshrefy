using Microsoft.EntityFrameworkCore;
using Moshrefy.Application.Interfaces.IRepositories;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Enums;
using Moshrefy.infrastructure.Data;
using Moshrefy.infrastructure.Repositories.GenericRepository;

namespace Moshrefy.infrastructure.Repositories
{
    public class ExamResultRepository(AppDbContext appDbContext) : GenericRepository<ExamResult, int>(appDbContext), IExamResultRepository
    {
        public async Task<IEnumerable<ExamResult>> GetExamResultsByStatusAsync(ExamResultStatus resultStatus)
        {
            return await appDbContext.Set<ExamResult>()
                    .Where(er => er.ExamResultStatus == resultStatus)
                    .ToListAsync();
        }
    }
}