using Microsoft.EntityFrameworkCore;
using Moshrefy.Application.Interfaces.IRepositories;
using Moshrefy.Domain.Entities;
using Moshrefy.infrastructure.Data;
using Moshrefy.infrastructure.Repositories.GenericRepository;

namespace Moshrefy.infrastructure.Repositories
{
    public class TeacherCourseRepository(AppDbContext appDbContext) : GenericRepository<TeacherCourse, int>(appDbContext), ITeacherCourseRepository
    {
        public async Task<IEnumerable<TeacherCourse>> GetByTeacherIdAsync(int teacherId)
        {
            return await appDbContext.Set<TeacherCourse>()
                .Include(tc => tc.Teacher)
                .Include(tc => tc.Course)
                    .ThenInclude(c => c.AcademicYear)
                .Where(tc => tc.TeacherId == teacherId)
                .ToListAsync();
        }

        public async Task<IEnumerable<TeacherCourse>> GetByTeacherNameAsync(string teacherName)
        {
            return await appDbContext.Set<TeacherCourse>()
                .Include(tc => tc.Teacher)
                .Where(tc => tc.Teacher.Name.Contains(teacherName))
                .ToListAsync();
        }

        public async Task<IEnumerable<TeacherCourse>> GetByTeacherPhoneAsync(string teacherPhone)
        {
            return await appDbContext.Set<TeacherCourse>()
                .Include(tc => tc.Teacher)
                .Where(tc => tc.Teacher.Phone == teacherPhone)
                .ToListAsync();
        }

        public async Task<IEnumerable<TeacherCourse>> GetByCourseIdAsync(int courseId)
        {
            return await appDbContext.Set<TeacherCourse>()
                .Include(tc => tc.Teacher)
                .Include(tc => tc.Course)
                .Where(tc => tc.CourseId == courseId)
                .ToListAsync();
        }
    }
}