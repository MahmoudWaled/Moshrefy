using Moshrefy.Domain.Enums;

namespace Moshrefy.Application.DTOs.Student
{
    public class StudentResponseDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = default!;

        public string? Email { get; set; }

        public string FirstPhone { get; set; } = default!;

        public string? SecondPhone { get; set; }

        public string? FatherPhone { get; set; }

        public string? MotherPhone { get; set; }

        public string? NationalId { get; set; }

        public string? Notes { get; set; }

        public DateTime DateOfBirth { get; set; }

        public StudentStatus StudentStatus { get; set; }

        public bool IsDeleted { get; set; }

        public int Age => DateTime.Now.Year - DateOfBirth.Year;

        public int EnrolledCoursesCount { get; set; }
    }
}