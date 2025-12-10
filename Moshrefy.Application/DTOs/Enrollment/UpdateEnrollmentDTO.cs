namespace Moshrefy.Application.DTOs.Enrollment
{
    public class UpdateEnrollmentDTO
    {
        public int StudentId { get; set; }

        public int CourseId { get; set; }

        public bool IsActive { get; set; }
    }
}