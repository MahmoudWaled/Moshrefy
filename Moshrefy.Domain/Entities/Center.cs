using Moshrefy.Domain.Identity;
using System.ComponentModel.DataAnnotations;

namespace Moshrefy.Domain.Entities
{
    public class Center
    {

        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        [MinLength(3)]
        public string Name { get; set; } = string.Empty;
        [Required]
        [MaxLength(200)]
        [MinLength(3)]
        public string Address { get; set; } = string.Empty;
        [MaxLength(500)]
        public string? Description { get; set; }
        [EmailAddress]
        [MaxLength(100)]
        public string? Email { get; set; }
        [Phone]
        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;
        [Required]
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }


        // --- Auditing properties ---
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public string CreatedById { get; set; } = null!;
        public string CreatedByName { get; set; } = null!;
        public ApplicationUser CreatedBy { get; set; } = null!;


        public DateTimeOffset? ModifiedAt { get; set; }
        public string? ModifiedById { get; set; }
        public string? ModifiedByName { get; set; }
        public ApplicationUser? ModifiedBy { get; set; }


        // --- Relations with entities ---
        #region Collections of entities

        public ICollection<AcademicYear> AcademicYears { get; set; } = new List<AcademicYear>();
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        public ICollection<Classroom> Classrooms { get; set; } = new List<Classroom>();
        public ICollection<Course> Courses { get; set; } = new List<Course>();
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<Exam> Exams { get; set; } = new List<Exam>();
        public ICollection<ExamResult> ExamResults { get; set; } = new List<ExamResult>();
        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public ICollection<Session> Sessions { get; set; } = new List<Session>();
        public ICollection<Student> Students { get; set; } = new List<Student>();
        public ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();
        public ICollection<TeacherCourse> TeacherCourses { get; set; } = new List<TeacherCourse>();
        public ICollection<TeacherItem> TeacherItems { get; set; } = new List<TeacherItem>();
        public ICollection<Item> Items { get; set; } = new List<Item>();
        #endregion

    }
}
