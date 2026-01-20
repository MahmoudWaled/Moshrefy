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
    public class SessionRepository(AppDbContext appDbContext) : GenericRepository<Session, int>(appDbContext), ISessionRepository
    {
        // Predicate overload for proper server-side filtering
        public new async Task<IEnumerable<Session>> GetAllAsync(Expression<Func<Session, bool>> predicate, PaginationParamter paginationParamter)
        {
            return await appDbContext.Set<Session>()
                .Include(s => s.TeacherCourse)
                    .ThenInclude(tc => tc.Teacher)
                .Include(s => s.TeacherCourse)
                    .ThenInclude(tc => tc.Course)
                .Include(s => s.Classroom)
                .Include(s => s.AcademicYear)
                .Where(predicate)
                .Skip((paginationParamter.PageNumber - 1) * paginationParamter.PageSize)
                .Take(paginationParamter.PageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Session>> GetByAcademicYearAsync(string academicYear)
        {

            return await appDbContext.Set<Session>()
                .Where(s => s.AcademicYear.Name.Contains(academicYear))
                .ToListAsync();
        }

        public async Task<IEnumerable<Session>> GetByClassroomIdAsync(int classroomId)
        {
            return await appDbContext.Set<Session>()
                .Where(s => s.ClassroomId == classroomId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Session>> GetByCourseNameAsync(string courseName)
        {
            return await appDbContext.Set<Session>()
                .Where(s => s.TeacherCourse.Course.Name.Contains(courseName))
                .ToListAsync();
        }

        public async Task<IEnumerable<Session>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await appDbContext.Set<Session>()
                .Where(s => s.SpecificDate >= startDate && s.SpecificDate <= endDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Session>> GetByDayOfWeekAsync(DayOfWeek day)
        {
            return await appDbContext.Set<Session>()
                .Where(s => s.RepeatDayOfWeek == day)
                .ToListAsync();
        }

        public async Task<IEnumerable<Session>> GetBySpecificDateAsync(DateTime date)
        {
            return await appDbContext.Set<Session>()
                .Where(s => s.SpecificDate == date.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Session>> GetByStatusAsync(SessionStatus status)
        {
            return await appDbContext.Set<Session>()
                .Where(s => s.SessionStatus == status)
                .ToListAsync();
        }

        public async Task<IEnumerable<Session>> GetByTeacherIdAsync(int teacherId)
        {
            return await appDbContext.Set<Session>()
                .Where(s => s.TeacherCourse.TeacherId == teacherId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Session>> GetByTeacherNameAsync(string TeacherName)
        {
            return await appDbContext.Set<Session>()
                .Where(s => s.TeacherCourse.Teacher.Name.Contains(TeacherName))
                .ToListAsync();
        }

        public async Task<IEnumerable<Session>> GetPastSessionsAsync(int daysBack)
        {
            return await appDbContext.Set<Session>()
                .Where(s => s.SpecificDate < DateTime.Now && s.SpecificDate >= DateTime.Now.AddDays(-daysBack))
                .ToListAsync();
        }

        public async Task<IEnumerable<Session>> GetUnpaidSessionsAsync()
        {
            return await appDbContext.Set<Session>()
                .Where(s => s.IsPaid == false)
                .ToListAsync();
        }

        public async Task<IEnumerable<Session>> GetUpcomingSessionsAsync()
        {
            return await appDbContext.Set<Session>()
                .Where(s => s.SpecificDate > DateTime.Now)
                .ToListAsync();
        }
    }
}