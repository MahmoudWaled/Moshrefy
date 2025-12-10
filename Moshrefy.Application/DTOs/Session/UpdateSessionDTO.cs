using Moshrefy.Domain.Enums;

namespace Moshrefy.Application.DTOs.Session
{
    public class UpdateSessionDTO
    {
        public DateTime? SpecificDate { get; set; }

        public DayOfWeek? RepeatDayOfWeek { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public SessionStatus SessionStatus { get; set; }

        public bool IsPaid { get; set; }

        public decimal Price { get; set; }

        public string? Note { get; set; }

        public int AcademicYearId { get; set; }

        public int TeacherCourseId { get; set; }

        public int ClassroomId { get; set; }
    }
}