using System.ComponentModel.DataAnnotations;

namespace Moshrefy.Application.DTOs.Auth
{
    public class ChangePasswordDTO
    {
        public string CurrentPassword { get; set; } = default!;

        public string NewPassword { get; set; } = default!;

        [Compare(nameof(NewPassword), ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = default!;
    }
}
