namespace Moshrefy.Application.DTOs.Center
{
    public class CenterResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string Address { get; set; } = default!;
        public string? Description { get; set; }
        public string? Email { get; set; }
        public string Phone { get; set; } = default!;
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        // Audit fields
        public DateTimeOffset CreatedAt { get; set; }
        public string CreatedByName { get; set; } = default!;
        public DateTimeOffset? ModifiedAt { get; set; }
        public string? ModifiedByName { get; set; }
    }
}