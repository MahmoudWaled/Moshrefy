using Microsoft.EntityFrameworkCore;
using Moshrefy.Domain.Enums;
using Moshrefy.Domain.SoftDeletable;
using System.ComponentModel.DataAnnotations;

namespace Moshrefy.Domain.Entities
{
    public class Item : BaseEntity, ISoftDeletable
    {
        // Example: "Math Textbook", "Science Kit", "Art Supplies"
        [Required]
        [MaxLength(100)]
        [MinLength(2)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0, 1000000)]
        [Precision(18, 2)]
        public decimal Price { get; set; }

        public int? ReservedByStudentId { get; set; }
        public Student? ReservedByStudent { get; set; }

        // Enum: Available, OutOfStock, Disabled
        [Required]
        public ItemStatus ItemStatus { get; set; } = ItemStatus.Available;

        // Soft delete
        public bool IsDeleted { get; set; } = false;


        // ---- Collection of TeacherItem ----
        public ICollection<TeacherItem> TeacherItems { get; set; } = new List<TeacherItem>();
    }
}
