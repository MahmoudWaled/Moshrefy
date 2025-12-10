namespace Moshrefy.Application.DTOs.User
{
    public class AuthResponseDTO
    {
        public bool Success { get; set; }
        public string Token { get; set; } = default!;
        public string? RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string UserId { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public IEnumerable<string> Roles { get; set; } = Enumerable.Empty<string>();
    }
}
