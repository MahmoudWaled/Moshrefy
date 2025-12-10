using System.ComponentModel.DataAnnotations;

namespace Moshrefy.Web.Models.Center
{
    public class UpdateCenterVM
    {
        [Required(ErrorMessage = "Center name is required")]
        [Display(Name = "Center Name")]
        public string Name { get; set; } = default!;

        [Required(ErrorMessage = "Address is required")]
        [Display(Name = "Address")]
        public string Address { get; set; } = default!;

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Display(Name = "Email Address")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Phone is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; } = default!;

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
    }
}
