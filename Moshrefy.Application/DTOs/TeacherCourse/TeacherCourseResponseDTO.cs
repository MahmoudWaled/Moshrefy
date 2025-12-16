namespace Moshrefy.Application.DTOs.TeacherCourse
{
    public class TeacherCourseResponseDTO
    {
        public int Id { get; set; }

        public int TeacherId { get; set; }

        public string? TeacherName { get; set; }

        public int CourseId { get; set; }

        public string? CourseName { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public bool CourseIsDeleted { get; set; }

        public string? AcademicYearName { get; set; }
    }
}