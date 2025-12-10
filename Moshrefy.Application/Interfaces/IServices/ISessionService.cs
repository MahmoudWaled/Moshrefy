using Moshrefy.Application.DTOs.Session;
using Moshrefy.Domain.Enums;
using Moshrefy.Domain.Paramter;

namespace Moshrefy.Application.Interfaces.IServices
{
    public interface ISessionService
    {
        Task<SessionResponseDTO> CreateAsync(CreateSessionDTO createSessionDTO);
        Task<SessionResponseDTO?> GetByIdAsync(int id);
        Task<List<SessionResponseDTO>> GetAllAsync(PaginationParamter paginationParamter);
        Task<List<SessionResponseDTO>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<List<SessionResponseDTO>> GetBySpecificDateAsync(DateTime date);
        Task<List<SessionResponseDTO>> GetByCourseNameAsync(string courseName);
        Task<List<SessionResponseDTO>> GetByTeacherIdAsync(int teacherId);
        Task<List<SessionResponseDTO>> GetByTeacherNameAsync(string teacherName);
        Task<List<SessionResponseDTO>> GetByClassroomIdAsync(int classroomId);
        Task<List<SessionResponseDTO>> GetByStatusAsync(SessionStatus status);
        Task<List<SessionResponseDTO>> GetUpcomingSessionsAsync();
        Task<List<SessionResponseDTO>> GetPastSessionsAsync(int daysBack);
        Task<List<SessionResponseDTO>> GetByDayOfWeekAsync(DayOfWeek day);
        Task<List<SessionResponseDTO>> GetUnpaidSessionsAsync();
        Task<List<SessionResponseDTO>> GetByAcademicYearAsync(string academicYear);
        Task UpdateAsync(int id, UpdateSessionDTO updateSessionDTO);
        Task DeleteAsync(int id);
        Task SoftDeleteAsync(int id);
        Task RestoreAsync(int id);
        Task MarkAsPaidAsync(int id);
        Task MarkAsUnpaidAsync(int id);
        Task UpdateStatusAsync(int id, SessionStatus status);
    }
}