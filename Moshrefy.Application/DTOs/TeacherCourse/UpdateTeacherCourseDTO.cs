namespace Moshrefy.Application.DTOs.TeacherCourse
{
    public class UpdateTeacherCourseDTO
    {
        public int TeacherId { get; set; }

        public int CourseId { get; set; }

        public bool IsActive { get; set; }
    }
}