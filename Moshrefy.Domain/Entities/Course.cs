using Moshrefy.Domain.SoftDeletable;
using System.ComponentModel.DataAnnotations;

namespace Moshrefy.Domain.Entities
{
    public class Course : BaseEntity, ISoftDeletable
    {
        [Required]
        [MaxLength(100)]
        [MinLength(2)]
        public string Name { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        // Soft delete
        public bool IsDeleted { get; set; } = false;


        // ---------- Relation with AcademicYear ----------
        public int AcademicYearId { get; set; }
        public AcademicYear? AcademicYear { get; set; }


        // ---------- Collection of Enrollments ---------
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();


        // ---------- Collection of Exams ----------
        public ICollection<Exam> Exams { get; set; } = new List<Exam>();


        // ---------- Collection of TeacherCourses ----------
        public ICollection<TeacherCourse> TeacherCourses { get; set; } = new List<TeacherCourse>();








    }
}
