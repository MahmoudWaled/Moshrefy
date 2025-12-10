using Moshrefy.Domain.SoftDeletable;
using System.ComponentModel.DataAnnotations;

namespace Moshrefy.Domain.Entities
{
    public class TeacherCourse : BaseEntity, ISoftDeletable
    {

        public bool IsActive { get; set; } = true;


        // soft delete
        public bool IsDeleted { get; set; } = false;


        // ----------- Relation with Teacher -----------
        // teacher teaching specific course
        [Required]
        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; } = null!;


        // ----------- Relation with Course -----------
        // course taught by specific teacher
        [Required]
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;


        // ---------- Collection of Exams ----------
        // exams created by specific teacher for specific course
        public ICollection<Exam> Exams { get; set; } = new List<Exam>();


        // ---------- Collection of Sessions ----------
        public ICollection<Session> Sessions { get; set; } = new List<Session>();

    }
}
