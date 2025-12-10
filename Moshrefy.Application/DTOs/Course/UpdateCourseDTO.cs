namespace Moshrefy.Application.DTOs.Course
{
    public class UpdateCourseDTO
    {
        public string Name { get; set; } = default!;

        public int AcademicYearId { get; set; }

        public bool IsActive { get; set; }
    }
}