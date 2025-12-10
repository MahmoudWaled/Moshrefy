using System.ComponentModel.DataAnnotations;

namespace Moshrefy.Web.Models.Teacher
{
    public class CreateTeacherVM
    {
        [Required(ErrorMessage = "Name is required")]
        [Display(Name = "Teacher Name")]
        public string Name { get; set; } = default!;

        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Display(Name = "Email Address")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Phone is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; } = default!;

        [Required(ErrorMessage = "Specialization is required")]
        [Display(Name = "Specialization")]
        public string Specialization { get; set; } = default!;
    }
}
