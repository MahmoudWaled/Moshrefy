namespace Moshrefy.Web.Models.TeacherCourse
{
    public class TeacherCourseVM
    {
        public int Id { get; set; }

        public int TeacherId { get; set; }

        public string? TeacherName { get; set; }

        public int CourseId { get; set; }

        public string? CourseName { get; set; }

        public bool IsActive { get; set; }
    }
}
