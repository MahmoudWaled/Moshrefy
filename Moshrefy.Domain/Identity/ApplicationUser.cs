using Microsoft.AspNetCore.Identity;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.SoftDeletable;
using System.ComponentModel.DataAnnotations;

namespace Moshrefy.Domain.Identity
{
    public class ApplicationUser : IdentityUser, ISoftDeletable
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        // center relation
        public int? CenterId { get; set; }
        public Center? Center { get; set; }

        // Audit fields
        public string? CreatedById { get; set; }
        public string? CreatedByName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? ModifiedById { get; set; }
        public string? ModifiedByName { get; set; }
        public DateTime? ModifiedAt { get; set; }

        // Soft delete
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
    }
}
