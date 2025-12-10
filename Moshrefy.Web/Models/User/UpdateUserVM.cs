using System.ComponentModel.DataAnnotations;

namespace Moshrefy.Web.Models.User
{
    public class UpdateUserVM
    {
        [Display(Name = "Full Name")]
        public string? Name { get; set; }

        [Display(Name = "Username")]
        public string? UserName { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Display(Name = "Email Address")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Invalid phone number")]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Center")]
        public int? CenterId { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;
    }
}
