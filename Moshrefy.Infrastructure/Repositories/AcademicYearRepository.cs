using Microsoft.EntityFrameworkCore;
using Moshrefy.Application.Interfaces.IRepositories;
using Moshrefy.Domain.Entities;
using Moshrefy.infrastructure.Data;
using Moshrefy.infrastructure.Repositories.GenericRepository;

namespace Moshrefy.infrastructure.Repositories
{
    public class AcademicYearRepository(AppDbContext appDbContext) : GenericRepository<AcademicYear, int>(appDbContext), IAcademicYearRepository
    {
        public async Task<IEnumerable<AcademicYear>> GetByName(string academicYearName)
        {
            return await appDbContext.Set<AcademicYear>()
                .Where(ay => ay.Name.Contains(academicYearName))
                .ToListAsync();
        }
    }
}