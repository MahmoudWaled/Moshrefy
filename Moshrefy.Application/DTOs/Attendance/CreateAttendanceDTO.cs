using Moshrefy.Domain.Enums;

namespace Moshrefy.Application.DTOs.Attendance
{
    public class CreateAttendanceDTO
    {
        public AttendanceStatus AttendanceStatus { get; set; }

        public string? Note { get; set; }

        public int StudentId { get; set; }

        public int SessionId { get; set; }

        public int ExamId { get; set; }
    }
}