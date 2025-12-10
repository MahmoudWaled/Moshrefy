using System.ComponentModel.DataAnnotations;

namespace Moshrefy.Web.Models
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Username is required.")]
        [Display(Name = "Username")]
        public string UserName { get; set; } = default!;

        [Required(ErrorMessage = "Password is required.")]
        [Display(Name = "Password")]
        public string Password { get; set; } = default!;
    }
}
