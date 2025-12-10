using System.ComponentModel.DataAnnotations;

namespace Moshrefy.Application.DTOs.Student
{
    public class CreateStudentDTO
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = default!;

        [EmailAddress]
        public string? Email { get; set; }

        [Required(ErrorMessage = "First phone is required")]
        [Phone]
        public string FirstPhone { get; set; } = default!;

        [Phone]
        public string? SecondPhone { get; set; }

        [Phone]
        public string? FatherPhone { get; set; }

        [Phone]
        public string? MotherPhone { get; set; }

        public string? NationalId { get; set; }

        public string? Notes { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }
    }
}