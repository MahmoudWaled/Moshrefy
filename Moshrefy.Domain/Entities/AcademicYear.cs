using Moshrefy.Domain.SoftDeletable;
using System.ComponentModel.DataAnnotations;

namespace Moshrefy.Domain.Entities
{

    public class AcademicYear : BaseEntity, ISoftDeletable
    {
        [Required]
        [MaxLength(50)]
        [MinLength(3)]
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        // soft delete
        public bool IsDeleted { get; set; } = false;
        public ICollection<Session>? Sessions { get; set; }
        public ICollection<Course>? Courses { get; set; }


    }
}
