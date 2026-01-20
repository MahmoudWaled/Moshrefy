using Moshrefy.Domain.Interfaces;
using Moshrefy.Domain.SoftDeletable;
using System.ComponentModel.DataAnnotations;

namespace Moshrefy.Domain.Entities
{
    public class TeacherItem : BaseEntity, ISoftDeletable, IActivatable
    {

        public bool IsActive { get; set; } = true;
        // soft delete
        public bool IsDeleted { get; set; } = false;


        // ----------- Relation with Teacher  -----------
        [Required]
        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; } = null!;

        // ----------- Relation with Item -----------
        [Required]
        public int ItemId { get; set; }
        public Item Item { get; set; } = null!;



        // ----------- Collection Of Payment -----------
        public ICollection<Payment>? Payments { get; set; }


    }
}
