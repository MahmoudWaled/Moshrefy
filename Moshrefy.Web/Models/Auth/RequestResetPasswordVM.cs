using System.ComponentModel.DataAnnotations;

namespace Moshrefy.Web.Models.Auth
{
    public class RequestResetPasswordVM
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = default!;
    }
}
