namespace Moshrefy.Web.Models.Enrollment
{
    public class EnrollmentVM
    {
        public int Id { get; set; }

        public int StudentId { get; set; }

        public string? StudentName { get; set; }

        public int CourseId { get; set; }

        public string? CourseName { get; set; }

        public bool IsActive { get; set; }
    }
}
