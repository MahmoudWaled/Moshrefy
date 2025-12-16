namespace Moshrefy.Web.Models.Course
{
    public class CourseVM
    {
        public int Id { get; set; }

        public string Name { get; set; } = default!;

        public int AcademicYearId { get; set; }

        public string? AcademicYearName { get; set; }

        public bool IsActive { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}
