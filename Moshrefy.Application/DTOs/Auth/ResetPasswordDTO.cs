namespace Moshrefy.Application.DTOs.Auth
{
    public class ResetPasswordDTO
    {
        public string UserId { get; set; } = default!;
        public string Token { get; set; } = default!;
        public string NewPassword { get; set; } = default!;
    }
}
