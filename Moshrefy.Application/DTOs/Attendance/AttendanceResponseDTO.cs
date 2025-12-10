using Moshrefy.Domain.Enums;

namespace Moshrefy.Application.DTOs.Attendance
{
    public class AttendanceResponseDTO
    {
        public int Id { get; set; }

        public AttendanceStatus AttendanceStatus { get; set; }

        public string? Note { get; set; }

        public int StudentId { get; set; }

        public string? StudentName { get; set; }

        public int SessionId { get; set; }

        public int ExamId { get; set; }

        public string? ExamName { get; set; }
    }
}