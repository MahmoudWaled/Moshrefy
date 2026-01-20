using Moshrefy.Domain.Interfaces;
using Moshrefy.Domain.SoftDeletable;
using System.ComponentModel.DataAnnotations;

namespace Moshrefy.Domain.Entities
{
    public class Classroom : BaseEntity, ISoftDeletable, IActivatable
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(200)]
        public string? Location { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        // Soft delete
        public bool IsDeleted { get; set; } = false;



        // ---------- Relation with Session  ----------
        public ICollection<Session> Sessions { get; set; } = new List<Session>();


        // ---------- Relation with Exam  ----------
        public ICollection<Exam> Exams { get; set; } = new List<Exam>();



    }
}
