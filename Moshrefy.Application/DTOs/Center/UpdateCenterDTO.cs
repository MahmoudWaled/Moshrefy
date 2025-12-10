namespace Moshrefy.Application.DTOs.Center
{
    public class UpdateCenterDTO
    {

        public string Name { get; set; } = default!;

        public string Address { get; set; } = default!;

        public string? Description { get; set; }

        public string? Email { get; set; }

        public string Phone { get; set; } = default!;

        public bool IsActive { get; set; }
    }
}