using Microsoft.EntityFrameworkCore;
using Moshrefy.Application.Interfaces.IRepositories;
using Moshrefy.Domain.Entities;
using Moshrefy.infrastructure.Data;
using Moshrefy.infrastructure.Repositories.GenericRepository;

namespace Moshrefy.infrastructure.Repositories
{
    public class TeacherItemRepository(AppDbContext appDbContext) : GenericRepository<TeacherItem, int>(appDbContext), ITeacherItemRepository
    {
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