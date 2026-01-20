namespace Moshrefy.Application.DTOs.Enrollment
{
    public class EnrollmentResponseDTO
    {
        public int Id { get; set; }

        public int StudentId { get; set; }

        public string? StudentName { get; set; }

        public int CourseId { get; set; }

        public string? CourseName { get; set; }

        public string? AcademicYearName { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public bool CourseIsDeleted { get; set; }

        public bool StudentIsDeleted { get; set; }

        // Audit fields
        public DateTimeOffset CreatedAt { get; set; }

        public string? CreatedByName { get; set; }

        public DateTimeOffset? ModifiedAt { get; set; }

        public string? ModifiedByName { get; set; }
    }
}