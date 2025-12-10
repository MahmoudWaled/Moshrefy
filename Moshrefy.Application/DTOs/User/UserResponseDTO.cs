namespace Moshrefy.Application.DTOs.User
{
    public class UserResponseDTO
    {
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? CenterName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedById { get; set; }
        public string? CreatedByName { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
