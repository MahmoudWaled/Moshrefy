using Moshrefy.Domain.SoftDeletable;
using System.ComponentModel.DataAnnotations;

namespace Moshrefy.Domain.Entities
{
    public class Teacher : BaseEntity, ISoftDeletable
    {
        [Required]
        [MaxLength(100)]
        [MinLength(2)]
        public string Name { get; set; } = string.Empty;

        [EmailAddress]
        public string? Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string Phone { get; set; } = string.Empty;

        // e.g., Mathematics, Science, English
        [Required]
        [MaxLength(100)]
        [MinLength(2)]
        public string Specialization { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        // soft delete
        public bool IsDeleted { get; set; } = false;



        // --------- Collection of teacherCourses ---------
        public ICollection<TeacherCourse> TeacherCourses { get; set; } = new List<TeacherCourse>();
        // --------- Collection of teacherItems ---------
        public ICollection<TeacherItem> TeacherItems { get; set; } = new List<TeacherItem>();

    }
}
