using Moshrefy.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Moshrefy.Web.Models.User
{
    public class CreateUserVM
    {
        [Required(ErrorMessage = "Name is required")]
        [Display(Name = "Full Name")]
        public string Name { get; set; } = default!;

        [Required(ErrorMessage = "Username is required")]
        [Display(Name = "Username")]
        public string UserName { get; set; } = default!;

        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Display(Name = "Email Address")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Invalid phone number")]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = default!;

        [Display(Name = "Center")]
        public int? CenterId { get; set; }

        [Required(ErrorMessage = "Role is required")]
        [Display(Name = "Role")]
        public RolesNames RoleName { get; set; }
    }
}
