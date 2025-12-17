using Microsoft.EntityFrameworkCore;
using Moshrefy.Application.Interfaces.IRepositories;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Paramter;
using Moshrefy.infrastructure.Data;
using Moshrefy.infrastructure.Repositories.GenericRepository;
using System.Linq.Expressions;

namespace Moshrefy.infrastructure.Repositories
{
    public class TeacherCourseRepository(AppDbContext appDbContext) : GenericRepository<TeacherCourse, int>(appDbContext), ITeacherCourseRepository
    {
        // Predicate overload for proper server-side filtering
        public new async Task<IEnumerable<TeacherCourse>> GetAllAsync(Expression<Func<TeacherCourse, bool>> predicate, PaginationParamter paginationParamter)
        {
            var pageNumber = paginationParamter.PageNumber ?? 1;
            var pageSize = paginationParamter.PageSize ?? 25;

            return await appDbContext.Set<TeacherCourse>()
                .Include(tc => tc.Teacher)
                .Include(tc => tc.Course)
                    .ThenInclude(c => c.AcademicYear)
                .Where(predicate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

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
