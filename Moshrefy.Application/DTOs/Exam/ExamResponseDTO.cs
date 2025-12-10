using Moshrefy.Domain.Enums;

namespace Moshrefy.Application.DTOs.Exam
{
    public class ExamResponseDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = default!;

        public DateTime Date { get; set; }

        public ExamStatus ExamStatus { get; set; }

        public float? TotalMarks { get; set; }

        public float? PassingMarks { get; set; }

        public int Duration { get; set; }

        public string? Description { get; set; }

        public int CourseId { get; set; }

        public string? CourseName { get; set; }

        public int ClassroomId { get; set; }

        public string? ClassroomName { get; set; }

        public int TeacherCourseId { get; set; }

        public string? TeacherName { get; set; }
    }
}