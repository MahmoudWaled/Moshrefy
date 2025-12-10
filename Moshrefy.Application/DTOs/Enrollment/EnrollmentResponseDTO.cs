namespace Moshrefy.Application.DTOs.Enrollment
{
    public class EnrollmentResponseDTO
    {
        public int Id { get; set; }

        public int StudentId { get; set; }

        public string? StudentName { get; set; }

        public int CourseId { get; set; }

        public string? CourseName { get; set; }

        public bool IsActive { get; set; }
    }
}