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
    public class AttendanceService(IUnitOfWork unitOfWork, IMapper mapper) : IAttendanceService
    {
        public async Task<AttendanceResponseDTO> CreateAsync(CreateAttendanceDTO createAttendanceDTO)
        {
            var attendance = mapper.Map<Attendance>(createAttendanceDTO);
            await unitOfWork.Attendances.AddAsync(attendance);
            await unitOfWork.SaveChangesAsync();
            return mapper.Map<AttendanceResponseDTO>(attendance);
        }

        public async Task<AttendanceResponseDTO?> GetByIdAsync(int id)
        {
            var attendance = await unitOfWork.Attendances.GetByIdAsync(id);
            if (attendance == null)
                throw new NotFoundException<int>(nameof(attendance), "attendance", id);

            return mapper.Map<AttendanceResponseDTO>(attendance);
        }

        public async Task<List<AttendanceResponseDTO>> GetAllAsync(PaginationParamter paginationParamter)
        {
            var attendances = await unitOfWork.Attendances.GetAllAsync(paginationParamter);
            return mapper.Map<List<AttendanceResponseDTO>>(attendances);
        }

        public async Task<List<AttendanceResponseDTO>> GetByStudentIdAsync(int studentId)
        {
            var attendances = await unitOfWork.Attendances.GetAllAsync(new PaginationParamter());
            var filtered = attendances.Where(a => a.StudentId == studentId).ToList();
            return mapper.Map<List<AttendanceResponseDTO>>(filtered);
        }

        public async Task<List<AttendanceResponseDTO>> GetBySessionIdAsync(int sessionId)
        {
            var attendances = await unitOfWork.Attendances.GetAllAsync(new PaginationParamter());
            var filtered = attendances.Where(a => a.SessionId == sessionId).ToList();
            return mapper.Map<List<AttendanceResponseDTO>>(filtered);
        }

        public async Task<List<AttendanceResponseDTO>> GetByExamIdAsync(int examId)
        {
            var attendances = await unitOfWork.Attendances.GetAllAsync(new PaginationParamter());
            var filtered = attendances.Where(a => a.ExamId == examId).ToList();
            return mapper.Map<List<AttendanceResponseDTO>>(filtered);
        }

        public async Task<List<AttendanceResponseDTO>> GetByStatusAsync(AttendanceStatus status)
        {
            var attendances = await unitOfWork.Attendances.GetAllAsync(new PaginationParamter());
            var filtered = attendances.Where(a => a.AttendanceStatus == status).ToList();
            return mapper.Map<List<AttendanceResponseDTO>>(filtered);
        }

        public async Task UpdateAsync(int id, UpdateAttendanceDTO updateAttendanceDTO)
        {
            var attendance = await unitOfWork.Attendances.GetByIdAsync(id);
            if (attendance == null)
                throw new NotFoundException<int>(nameof(attendance), "attendance", id);

            mapper.Map(updateAttendanceDTO, attendance);
            unitOfWork.Attendances.UpdateAsync(attendance);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var attendance = await unitOfWork.Attendances.GetByIdAsync(id);
            if (attendance == null)
                throw new NotFoundException<int>(nameof(attendance), "attendance", id);

            unitOfWork.Attendances.DeleteAsync(attendance);
            await unitOfWork.SaveChangesAsync();
        }
    }
}