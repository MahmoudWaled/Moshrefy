using Moshrefy.Domain.Enums;
using Moshrefy.Domain.SoftDeletable;
using System.ComponentModel.DataAnnotations;

namespace Moshrefy.Domain.Entities
{
    public class Student : BaseEntity, ISoftDeletable
    {
        [Required]
        [MaxLength(100)]
        [MinLength(2)]
        public string Name { get; set; } = string.Empty;

        [EmailAddress]
        public string? Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string FirstPhone { get; set; } = string.Empty;

        [Phone]
        public string? SecondPhone { get; set; }

        [Phone]
        public string? FatherPhone { get; set; }

        [Phone]
        public string? MotherPhone { get; set; }

        public string? NationalId { get; set; }
        public string? Notes { get; set; }
        // Enum: Active, Inactive, Suspended
        [Required]
        public StudentStatus StudentStatus { get; set; } = StudentStatus.Active;

        [Required]
        public DateTime DateOfBirth { get; set; }

        // soft delete
        public bool IsDeleted { get; set; } = false;



        // -------- Collection of Exam ----------
        public ICollection<Exam> Exams { get; set; } = new List<Exam>();


        // -------- Collection of ExamResult ----------
        public ICollection<ExamResult> ExamResults { get; set; } = new List<ExamResult>();


        // -------- Collection of Attendance ----------
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();


        // -------- Collection of Payment ----------
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();


        // -------- Collection of Enrollment ----------
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();


        // -------- Collection of Item ----------
        public ICollection<Item> ReservedItems { get; set; } = new List<Item>();

    }
}
