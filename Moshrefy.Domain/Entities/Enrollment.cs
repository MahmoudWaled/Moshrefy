using Moshrefy.Domain.SoftDeletable;

namespace Moshrefy.Domain.Entities
{
    public class Enrollment : BaseEntity, ISoftDeletable
    {
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;


        // -------- Relation with Student ----------
        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;



        // -------- Relation with Course ----------
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;

    }
}
