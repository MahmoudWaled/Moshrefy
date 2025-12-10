using Microsoft.EntityFrameworkCore;
using Moshrefy.Application.Interfaces.IRepositories;
using Moshrefy.Domain.Entities;
using Moshrefy.infrastructure.Data;
using Moshrefy.infrastructure.Repositories.GenericRepository;

namespace Moshrefy.infrastructure.Repositories
{
    public class CourseRepository(AppDbContext appDbContext) : GenericRepository<Course, int>(appDbContext), ICourseRepository
    {
        public async Task<IEnumerable<Course>> GetByName(string courseName)
        {
            return await appDbContext.Set<Course>()
                .Where(c => c.Name.Contains(courseName))
                .ToListAsync();
        }
    }
}