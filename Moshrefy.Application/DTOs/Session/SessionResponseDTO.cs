using Moshrefy.Domain.Enums;

namespace Moshrefy.Application.DTOs.Session
{
    public class SessionResponseDTO
    {
        public int Id { get; set; }

        public DateTime? SpecificDate { get; set; }

        public DayOfWeek? RepeatDayOfWeek { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public SessionStatus SessionStatus { get; set; }

        public bool IsPaid { get; set; }

        public decimal Price { get; set; }

        public string? Note { get; set; }

        public int AcademicYearId { get; set; }

        public string? AcademicYearName { get; set; }

        public int TeacherCourseId { get; set; }

        public string? TeacherName { get; set; }

        public string? CourseName { get; set; }

        public int ClassroomId { get; set; }

        public string? ClassroomName { get; set; }
    }
}