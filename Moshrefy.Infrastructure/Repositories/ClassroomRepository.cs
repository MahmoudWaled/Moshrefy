using Microsoft.EntityFrameworkCore;
using Moshrefy.Application.Interfaces.IRepositories;
using Moshrefy.Domain.Entities;
using Moshrefy.infrastructure.Data;
using Moshrefy.infrastructure.Repositories.GenericRepository;

namespace Moshrefy.infrastructure.Repositories
{
    public class ClassroomRepository(AppDbContext appDbContext) : GenericRepository<Classroom, int>(appDbContext), IClassroomRepository
    {
        public async Task<IEnumerable<Classroom>> GetByName(string classroomName)
        {
            return await appDbContext.Set<Classroom>()
                .Where(c => c.Name.Contains(classroomName))
                .ToListAsync();
        }
    }
}