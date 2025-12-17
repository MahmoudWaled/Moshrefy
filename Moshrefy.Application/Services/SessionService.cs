using AutoMapper;
using Moshrefy.Application.DTOs.Session;
using Moshrefy.Application.Interfaces.IUnitOfWork;
using Moshrefy.Application.Interfaces.IServices;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Enums;
using Moshrefy.Domain.Exceptions;
using Moshrefy.Domain.Paramter;

namespace Moshrefy.Application.Services
{
    public class SessionService(
        IUnitOfWork unitOfWork, 
        IMapper mapper,
        ITenantContext tenantContext
    ) : BaseService(tenantContext), ISessionService
    {
        public async Task<SessionResponseDTO> CreateAsync(CreateSessionDTO createSessionDTO)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var session = mapper.Map<Session>(createSessionDTO);
            session.CenterId = currentCenterId;
            await unitOfWork.Sessions.AddAsync(session);
            await unitOfWork.SaveChangesAsync();
            return mapper.Map<SessionResponseDTO>(session);
        }

        public async Task<SessionResponseDTO?> GetByIdAsync(int id)
        {
            var session = await unitOfWork.Sessions.GetByIdAsync(id);
            if (session == null)
                throw new NotFoundException<int>(nameof(session), "session", id);

            ValidateCenterAccess(session.CenterId, nameof(Session));
            return mapper.Map<SessionResponseDTO>(session);
        }

        public async Task<List<SessionResponseDTO>> GetAllAsync(PaginationParamter paginationParamter)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var sessions = await unitOfWork.Sessions.GetAllAsync(
                s => s.CenterId == currentCenterId && !s.IsDeleted,
                paginationParamter);
            return mapper.Map<List<SessionResponseDTO>>(sessions.ToList());
        }

        public async Task<List<SessionResponseDTO>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var sessions = await unitOfWork.Sessions.GetByDateRangeAsync(startDate, endDate);
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var filtered = sessions.Where(s => s.CenterId == currentCenterId && !s.IsDeleted).ToList();
            return mapper.Map<List<SessionResponseDTO>>(filtered);
        }

        public async Task<List<SessionResponseDTO>> GetBySpecificDateAsync(DateTime date)
        {
            var sessions = await unitOfWork.Sessions.GetBySpecificDateAsync(date);
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var filtered = sessions.Where(s => s.CenterId == currentCenterId && !s.IsDeleted).ToList();
            return mapper.Map<List<SessionResponseDTO>>(filtered);
        }

        public async Task<List<SessionResponseDTO>> GetByCourseNameAsync(string courseName)
        {
            var sessions = await unitOfWork.Sessions.GetByCourseNameAsync(courseName);
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var filtered = sessions.Where(s => s.CenterId == currentCenterId && !s.IsDeleted).ToList();
            return mapper.Map<List<SessionResponseDTO>>(filtered);
        }

        public async Task<List<SessionResponseDTO>> GetByTeacherIdAsync(int teacherId)
        {
            var sessions = await unitOfWork.Sessions.GetByTeacherIdAsync(teacherId);
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var filtered = sessions.Where(s => s.CenterId == currentCenterId && !s.IsDeleted).ToList();
            return mapper.Map<List<SessionResponseDTO>>(filtered);
        }

        public async Task<List<SessionResponseDTO>> GetByTeacherNameAsync(string teacherName)
        {
            var sessions = await unitOfWork.Sessions.GetByTeacherNameAsync(teacherName);
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var filtered = sessions.Where(s => s.CenterId == currentCenterId && !s.IsDeleted).ToList();
            return mapper.Map<List<SessionResponseDTO>>(filtered);
        }

        public async Task<List<SessionResponseDTO>> GetByClassroomIdAsync(int classroomId)
        {
            var sessions = await unitOfWork.Sessions.GetByClassroomIdAsync(classroomId);
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var filtered = sessions.Where(s => s.CenterId == currentCenterId && !s.IsDeleted).ToList();
            return mapper.Map<List<SessionResponseDTO>>(filtered);
        }

        public async Task<List<SessionResponseDTO>> GetByStatusAsync(SessionStatus status)
        {
            var sessions = await unitOfWork.Sessions.GetByStatusAsync(status);
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var filtered = sessions.Where(s => s.CenterId == currentCenterId && !s.IsDeleted).ToList();
            return mapper.Map<List<SessionResponseDTO>>(filtered);
        }

        public async Task<List<SessionResponseDTO>> GetUpcomingSessionsAsync()
        {
            var sessions = await unitOfWork.Sessions.GetUpcomingSessionsAsync();
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var filtered = sessions.Where(s => s.CenterId == currentCenterId && !s.IsDeleted).ToList();
            return mapper.Map<List<SessionResponseDTO>>(filtered);
        }

        public async Task<List<SessionResponseDTO>> GetPastSessionsAsync(int daysBack)
        {
            var sessions = await unitOfWork.Sessions.GetPastSessionsAsync(daysBack);
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var filtered = sessions.Where(s => s.CenterId == currentCenterId && !s.IsDeleted).ToList();
            return mapper.Map<List<SessionResponseDTO>>(filtered);
        }

        public async Task<List<SessionResponseDTO>> GetByDayOfWeekAsync(DayOfWeek day)
        {
            var sessions = await unitOfWork.Sessions.GetByDayOfWeekAsync(day);
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var filtered = sessions.Where(s => s.CenterId == currentCenterId && !s.IsDeleted).ToList();
            return mapper.Map<List<SessionResponseDTO>>(filtered);
        }

        public async Task<List<SessionResponseDTO>> GetUnpaidSessionsAsync()
        {
            var sessions = await unitOfWork.Sessions.GetUnpaidSessionsAsync();
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var filtered = sessions.Where(s => s.CenterId == currentCenterId && !s.IsDeleted).ToList();
            return mapper.Map<List<SessionResponseDTO>>(filtered);
        }

        public async Task<List<SessionResponseDTO>> GetByAcademicYearAsync(string academicYear)
        {
            var sessions = await unitOfWork.Sessions.GetByAcademicYearAsync(academicYear);
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var filtered = sessions.Where(s => s.CenterId == currentCenterId && !s.IsDeleted).ToList();
            return mapper.Map<List<SessionResponseDTO>>(filtered);
        }

        public async Task UpdateAsync(int id, UpdateSessionDTO updateSessionDTO)
        {
            var session = await unitOfWork.Sessions.GetByIdAsync(id);
            if (session == null)
                throw new NotFoundException<int>(nameof(session), "session", id);

            ValidateCenterAccess(session.CenterId, nameof(Session));
            mapper.Map(updateSessionDTO, session);
            unitOfWork.Sessions.UpdateAsync(session);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var session = await unitOfWork.Sessions.GetByIdAsync(id);
            if (session == null)
                throw new NotFoundException<int>(nameof(session), "session", id);

            ValidateCenterAccess(session.CenterId, nameof(Session));
            unitOfWork.Sessions.DeleteAsync(session);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int id)
        {
            var session = await unitOfWork.Sessions.GetByIdAsync(id);
            if (session == null)
                throw new NotFoundException<int>(nameof(session), "session", id);

            ValidateCenterAccess(session.CenterId, nameof(Session));
            session.IsDeleted = true;
            unitOfWork.Sessions.UpdateAsync(session);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task RestoreAsync(int id)
        {
            var session = await unitOfWork.Sessions.GetByIdAsync(id);
            if (session == null)
                throw new NotFoundException<int>(nameof(session), "session", id);

            ValidateCenterAccess(session.CenterId, nameof(Session));
            session.IsDeleted = false;
            unitOfWork.Sessions.UpdateAsync(session);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task MarkAsPaidAsync(int id)
        {
            var session = await unitOfWork.Sessions.GetByIdAsync(id);
            if (session == null)
                throw new NotFoundException<int>(nameof(session), "session", id);

            ValidateCenterAccess(session.CenterId, nameof(Session));
            session.IsPaid = true;
            unitOfWork.Sessions.UpdateAsync(session);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task MarkAsUnpaidAsync(int id)
        {
            var session = await unitOfWork.Sessions.GetByIdAsync(id);
            if (session == null)
                throw new NotFoundException<int>(nameof(session), "session", id);

            ValidateCenterAccess(session.CenterId, nameof(Session));
            session.IsPaid = false;
            unitOfWork.Sessions.UpdateAsync(session);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateStatusAsync(int id, SessionStatus status)
        {
            var session = await unitOfWork.Sessions.GetByIdAsync(id);
            if (session == null)
                throw new NotFoundException<int>(nameof(session), "session", id);

            ValidateCenterAccess(session.CenterId, nameof(Session));
            session.SessionStatus = status;
            unitOfWork.Sessions.UpdateAsync(session);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
