namespace Moshrefy.Application.DTOs.Course
{
    public class CourseResponseDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = default!;

        public int AcademicYearId { get; set; }

        public string? AcademicYearName { get; set; }

        public bool IsActive { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public bool IsDeleted { get; set; }
    }
}