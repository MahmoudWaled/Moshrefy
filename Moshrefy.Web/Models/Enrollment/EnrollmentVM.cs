namespace Moshrefy.Web.Models.Enrollment
{
    public class EnrollmentVM
    {
        public int Id { get; set; }

        public int StudentId { get; set; }

        public string? StudentName { get; set; }

        public int CourseId { get; set; }

        public string? CourseName { get; set; }

        public string? AcademicYearName { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public string? CreatedByName { get; set; }

        public DateTimeOffset? ModifiedAt { get; set; }

        public string? ModifiedByName { get; set; }
    }
}
