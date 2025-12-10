using System.ComponentModel.DataAnnotations;

namespace Moshrefy.Web.Models
{
    public class LoginVM
    {
        [Required(ErrorMessage ="Email is required.")]
        [EmailAddress(ErrorMessage ="Invalid Email Address.")]
        public string Email { get; set; } = default!;
        [Required(ErrorMessage ="Password is required.")]
        public string Password { get; set; } = default!;

    }
}
