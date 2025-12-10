using Moshrefy.Domain.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Moshrefy.Domain.Entities
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public string CreatedById { get; set; } = null!;
        public string CreatedByName { get; set; } = null!;
        public ApplicationUser CreatedBy { get; set; } = null!;


        public DateTimeOffset? ModifiedAt { get; set; }
        public string? ModifiedById { get; set; }
        public string? ModifiedByName { get; set; }
        public ApplicationUser? ModifiedBy { get; set; }


        // -------- Relation with Center ----------
        [ForeignKey("Center")]
        public int CenterId { get; set; }
    }
}
