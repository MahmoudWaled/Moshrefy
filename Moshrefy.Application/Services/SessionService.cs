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
    public class SessionService(IUnitOfWork unitOfWork, IMapper mapper) : ISessionService
    {
        public async Task<SessionResponseDTO> CreateAsync(CreateSessionDTO createSessionDTO)
        {
            var session = mapper.Map<Session>(createSessionDTO);
            await unitOfWork.Sessions.AddAsync(session);
            await unitOfWork.SaveChangesAsync();
            return mapper.Map<SessionResponseDTO>(session);
        }

        public async Task<SessionResponseDTO?> GetByIdAsync(int id)
        {
            var session = await unitOfWork.Sessions.GetByIdAsync(id);
            if (session == null)
                throw new NotFoundException<int>(nameof(session), "session", id);

            return mapper.Map<SessionResponseDTO>(session);
        }

        public async Task<List<SessionResponseDTO>> GetAllAsync(PaginationParamter paginationParamter)
        {
            var sessions = await unitOfWork.Sessions.GetAllAsync(paginationParamter);
            return mapper.Map<List<SessionResponseDTO>>(sessions);
        }

        public async Task<List<SessionResponseDTO>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var sessions = await unitOfWork.Sessions.GetByDateRangeAsync(startDate, endDate);
            return mapper.Map<List<SessionResponseDTO>>(sessions);
        }

        public async Task<List<SessionResponseDTO>> GetBySpecificDateAsync(DateTime date)
        {
            var sessions = await unitOfWork.Sessions.GetBySpecificDateAsync(date);
            return mapper.Map<List<SessionResponseDTO>>(sessions);
        }

        public async Task<List<SessionResponseDTO>> GetByCourseNameAsync(string courseName)
        {
            var sessions = await unitOfWork.Sessions.GetByCourseNameAsync(courseName);
            return mapper.Map<List<SessionResponseDTO>>(sessions);
        }

        public async Task<List<SessionResponseDTO>> GetByTeacherIdAsync(int teacherId)
        {
            var sessions = await unitOfWork.Sessions.GetByTeacherIdAsync(teacherId);
            return mapper.Map<List<SessionResponseDTO>>(sessions);
        }

        public async Task<List<SessionResponseDTO>> GetByTeacherNameAsync(string teacherName)
        {
            var sessions = await unitOfWork.Sessions.GetByTeacherNameAsync(teacherName);
            return mapper.Map<List<SessionResponseDTO>>(sessions);
        }

        public async Task<List<SessionResponseDTO>> GetByClassroomIdAsync(int classroomId)
        {
            var sessions = await unitOfWork.Sessions.GetByClassroomIdAsync(classroomId);
            return mapper.Map<List<SessionResponseDTO>>(sessions);
        }

        public async Task<List<SessionResponseDTO>> GetByStatusAsync(SessionStatus status)
        {
            var sessions = await unitOfWork.Sessions.GetByStatusAsync(status);
            return mapper.Map<List<SessionResponseDTO>>(sessions);
        }

        public async Task<List<SessionResponseDTO>> GetUpcomingSessionsAsync()
        {
            var sessions = await unitOfWork.Sessions.GetUpcomingSessionsAsync();
            return mapper.Map<List<SessionResponseDTO>>(sessions);
        }

        public async Task<List<SessionResponseDTO>> GetPastSessionsAsync(int daysBack)
        {
            var sessions = await unitOfWork.Sessions.GetPastSessionsAsync(daysBack);
            return mapper.Map<List<SessionResponseDTO>>(sessions);
        }

        public async Task<List<SessionResponseDTO>> GetByDayOfWeekAsync(DayOfWeek day)
        {
            var sessions = await unitOfWork.Sessions.GetByDayOfWeekAsync(day);
            return mapper.Map<List<SessionResponseDTO>>(sessions);
        }

        public async Task<List<SessionResponseDTO>> GetUnpaidSessionsAsync()
        {
            var sessions = await unitOfWork.Sessions.GetUnpaidSessionsAsync();
            return mapper.Map<List<SessionResponseDTO>>(sessions);
        }

        public async Task<List<SessionResponseDTO>> GetByAcademicYearAsync(string academicYear)
        {
            var sessions = await unitOfWork.Sessions.GetByAcademicYearAsync(academicYear);
            return mapper.Map<List<SessionResponseDTO>>(sessions);
        }

        public async Task UpdateAsync(int id, UpdateSessionDTO updateSessionDTO)
        {
            var session = await unitOfWork.Sessions.GetByIdAsync(id);
            if (session == null)
                throw new NotFoundException<int>(nameof(session), "session", id);

            mapper.Map(updateSessionDTO, session);
            unitOfWork.Sessions.UpdateAsync(session);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var session = await unitOfWork.Sessions.GetByIdAsync(id);
            if (session == null)
                throw new NotFoundException<int>(nameof(session), "session", id);

            unitOfWork.Sessions.DeleteAsync(session);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int id)
        {
            var session = await unitOfWork.Sessions.GetByIdAsync(id);
            if (session == null)
                throw new NotFoundException<int>(nameof(session), "session", id);

            session.IsDeleted = true;
            unitOfWork.Sessions.UpdateAsync(session);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task RestoreAsync(int id)
        {
            var session = await unitOfWork.Sessions.GetByIdAsync(id);
            if (session == null)
                throw new NotFoundException<int>(nameof(session), "session", id);

            session.IsDeleted = false;
            unitOfWork.Sessions.UpdateAsync(session);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task MarkAsPaidAsync(int id)
        {
            var session = await unitOfWork.Sessions.GetByIdAsync(id);
            if (session == null)
                throw new NotFoundException<int>(nameof(session), "session", id);

            session.IsPaid = true;
            unitOfWork.Sessions.UpdateAsync(session);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task MarkAsUnpaidAsync(int id)
        {
            var session = await unitOfWork.Sessions.GetByIdAsync(id);
            if (session == null)
                throw new NotFoundException<int>(nameof(session), "session", id);

            session.IsPaid = false;
            unitOfWork.Sessions.UpdateAsync(session);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateStatusAsync(int id, SessionStatus status)
        {
            var session = await unitOfWork.Sessions.GetByIdAsync(id);
            if (session == null)
                throw new NotFoundException<int>(nameof(session), "session", id);

            session.SessionStatus = status;
            unitOfWork.Sessions.UpdateAsync(session);
            await unitOfWork.SaveChangesAsync();
        }
    }
}