namespace Moshrefy.Application.DTOs.Course
{
    public class CreateCourseDTO
    {
        public string Name { get; set; } = default!;

        public int AcademicYearId { get; set; }
    }
}