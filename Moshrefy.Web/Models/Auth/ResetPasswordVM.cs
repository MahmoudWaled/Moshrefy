using System.ComponentModel.DataAnnotations;

namespace Moshrefy.Web.Models.Auth
{
    public class ResetPasswordVM
    {
        [Required]
        public string UserId { get; set; } = default!;

        [Required]
        public string Token { get; set; } = default!;

        [Required(ErrorMessage = "New password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; } = default!;

        [Required(ErrorMessage = "Confirm password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare(nameof(NewPassword), ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = default!;
    }
}
