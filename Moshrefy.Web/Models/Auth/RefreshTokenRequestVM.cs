using System.ComponentModel.DataAnnotations;

namespace Moshrefy.Web.Models.Auth
{
    public class RefreshTokenRequestVM
    {
        [Required(ErrorMessage = "Refresh token is required")]
        [Display(Name = "Refresh Token")]
        public string RefreshToken { get; set; } = default!;
    }
}
