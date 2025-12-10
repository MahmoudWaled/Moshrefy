using Moshrefy.Application.Interfaces.IGenericRepository;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Enums;

namespace Moshrefy.Application.Interfaces.IRepositories
{
    public interface ISessionRepository : IGenericRepository<Session, int>
    {
        public Task<IEnumerable<Session>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        public Task<IEnumerable<Session>> GetBySpecificDateAsync(DateTime date);
        public Task<IEnumerable<Session>> GetByCourseNameAsync(string courseName);
        public Task<IEnumerable<Session>> GetByTeacherIdAsync(int teacherId);
        public Task<IEnumerable<Session>> GetByTeacherNameAsync(string TeacherName);
        public Task<IEnumerable<Session>> GetByClassroomIdAsync(int classroomId);
        public Task<IEnumerable<Session>> GetByStatusAsync(SessionStatus status);
        public Task<IEnumerable<Session>> GetUpcomingSessionsAsync();
        public Task<IEnumerable<Session>> GetPastSessionsAsync(int daysBack);
        public Task<IEnumerable<Session>> GetByDayOfWeekAsync(DayOfWeek day);
        public Task<IEnumerable<Session>> GetUnpaidSessionsAsync();
        public Task<IEnumerable<Session>> GetByAcademicYearAsync(string academicYear);
    }
}