using Moshrefy.Application.DTOs.Attendance;
using Moshrefy.Domain.Enums;
using Moshrefy.Domain.Paramter;

namespace Moshrefy.Application.Interfaces.IServices
{
    public interface IAttendanceService
    {
        Task<AttendanceResponseDTO> CreateAsync(CreateAttendanceDTO createAttendanceDTO);
        Task<AttendanceResponseDTO?> GetByIdAsync(int id);
        Task<List<AttendanceResponseDTO>> GetAllAsync(PaginationParamter paginationParamter);
        Task<List<AttendanceResponseDTO>> GetByStudentIdAsync(int studentId);
        Task<List<AttendanceResponseDTO>> GetBySessionIdAsync(int sessionId);
        Task<List<AttendanceResponseDTO>> GetByExamIdAsync(int examId);
        Task<List<AttendanceResponseDTO>> GetByStatusAsync(AttendanceStatus status);
        Task UpdateAsync(int id, UpdateAttendanceDTO updateAttendanceDTO);
        Task DeleteAsync(int id);
    }
}