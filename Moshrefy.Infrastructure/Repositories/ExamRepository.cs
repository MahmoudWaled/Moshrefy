using Microsoft.EntityFrameworkCore;
using Moshrefy.Application.Interfaces.IRepositories;
using Moshrefy.Domain.Entities;
using Moshrefy.infrastructure.Data;
using Moshrefy.infrastructure.Repositories.GenericRepository;

namespace Moshrefy.infrastructure.Repositories
{
    public class ExamRepository(AppDbContext appDbContext) : GenericRepository<Exam, int>(appDbContext), IExamRepository
    {
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