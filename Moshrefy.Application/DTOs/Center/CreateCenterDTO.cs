namespace Moshrefy.Application.DTOs.Center
{
    public class CreateCenterDTO
    {
        public string Name { get; set; } = default!;

        public string Address { get; set; } = default!;

        public string? Description { get; set; }

        public string? Email { get; set; }

        public string Phone { get; set; } = default!;

    }
}