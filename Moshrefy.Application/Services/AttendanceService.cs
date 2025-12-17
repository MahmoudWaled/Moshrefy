using AutoMapper;
using Moshrefy.Application.DTOs.Attendance;
using Moshrefy.Application.Interfaces.IUnitOfWork;
using Moshrefy.Application.Interfaces.IServices;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Enums;
using Moshrefy.Domain.Exceptions;
using Moshrefy.Domain.Paramter;

namespace Moshrefy.Application.Services
{
    public class AttendanceService(
        IUnitOfWork unitOfWork, 
        IMapper mapper,
        ITenantContext tenantContext
    ) : BaseService(tenantContext), IAttendanceService
    {
        public async Task<AttendanceResponseDTO> CreateAsync(CreateAttendanceDTO createAttendanceDTO)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var attendance = mapper.Map<Attendance>(createAttendanceDTO);
            attendance.CenterId = currentCenterId;
            await unitOfWork.Attendances.AddAsync(attendance);
            await unitOfWork.SaveChangesAsync();
            return mapper.Map<AttendanceResponseDTO>(attendance);
        }

        public async Task<AttendanceResponseDTO?> GetByIdAsync(int id)
        {
            var attendance = await unitOfWork.Attendances.GetByIdAsync(id);
            if (attendance == null)
                throw new NotFoundException<int>(nameof(attendance), "attendance", id);

            ValidateCenterAccess(attendance.CenterId, nameof(Attendance));
            return mapper.Map<AttendanceResponseDTO>(attendance);
        }

        public async Task<List<AttendanceResponseDTO>> GetAllAsync(PaginationParamter paginationParamter)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var attendances = await unitOfWork.Attendances.GetAllAsync(
                a => a.CenterId == currentCenterId,
                paginationParamter);
            return mapper.Map<List<AttendanceResponseDTO>>(attendances.ToList());
        }

        public async Task<List<AttendanceResponseDTO>> GetByStudentIdAsync(int studentId)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var attendances = await unitOfWork.Attendances.GetAllAsync(
                a => a.CenterId == currentCenterId && a.StudentId == studentId,
                new PaginationParamter { PageSize = 1000 });
            return mapper.Map<List<AttendanceResponseDTO>>(attendances.ToList());
        }

        public async Task<List<AttendanceResponseDTO>> GetBySessionIdAsync(int sessionId)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var attendances = await unitOfWork.Attendances.GetAllAsync(
                a => a.CenterId == currentCenterId && a.SessionId == sessionId,
                new PaginationParamter { PageSize = 1000 });
            return mapper.Map<List<AttendanceResponseDTO>>(attendances.ToList());
        }

        public async Task<List<AttendanceResponseDTO>> GetByExamIdAsync(int examId)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var attendances = await unitOfWork.Attendances.GetAllAsync(
                a => a.CenterId == currentCenterId && a.ExamId == examId,
                new PaginationParamter { PageSize = 1000 });
            return mapper.Map<List<AttendanceResponseDTO>>(attendances.ToList());
        }

        public async Task<List<AttendanceResponseDTO>> GetByStatusAsync(AttendanceStatus status)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var attendances = await unitOfWork.Attendances.GetAllAsync(
                a => a.CenterId == currentCenterId && a.AttendanceStatus == status,
                new PaginationParamter { PageSize = 1000 });
            return mapper.Map<List<AttendanceResponseDTO>>(attendances.ToList());
        }

        public async Task UpdateAsync(int id, UpdateAttendanceDTO updateAttendanceDTO)
        {
            var attendance = await unitOfWork.Attendances.GetByIdAsync(id);
            if (attendance == null)
                throw new NotFoundException<int>(nameof(attendance), "attendance", id);

            ValidateCenterAccess(attendance.CenterId, nameof(Attendance));
            mapper.Map(updateAttendanceDTO, attendance);
            unitOfWork.Attendances.UpdateAsync(attendance);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var attendance = await unitOfWork.Attendances.GetByIdAsync(id);
            if (attendance == null)
                throw new NotFoundException<int>(nameof(attendance), "attendance", id);

            ValidateCenterAccess(attendance.CenterId, nameof(Attendance));
            unitOfWork.Attendances.DeleteAsync(attendance);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
