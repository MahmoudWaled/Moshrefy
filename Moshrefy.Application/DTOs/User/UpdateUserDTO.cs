namespace Moshrefy.Application.DTOs.User
{
    public class UpdateUserDTO
    {
        public string? Name { get; set; }

        public string? UserName { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public int? CenterId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
