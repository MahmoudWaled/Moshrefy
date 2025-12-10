using Moshrefy.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Moshrefy.Web.Models.Student
{
    public class UpdateStudentVM
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        [Display(Name = "Student Name")]
        public string Name { get; set; } = default!;

        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Display(Name = "Email Address")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "First phone is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        [Display(Name = "Primary Phone")]
        public string FirstPhone { get; set; } = default!;

        [Phone(ErrorMessage = "Invalid phone number")]
        [Display(Name = "Secondary Phone")]
        public string? SecondPhone { get; set; }

        [Phone(ErrorMessage = "Invalid phone number")]
        [Display(Name = "Father's Phone")]
        public string? FatherPhone { get; set; }

        [Phone(ErrorMessage = "Invalid phone number")]
        [Display(Name = "Mother's Phone")]
        public string? MotherPhone { get; set; }

        [Display(Name = "National ID")]
        public string? NationalId { get; set; }

        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        [Required(ErrorMessage = "Date of birth is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Student status is required")]
        [Display(Name = "Status")]
        public StudentStatus StudentStatus { get; set; }
    }
}
