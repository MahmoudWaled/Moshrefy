using Moshrefy.Domain.Enums;
using Moshrefy.Domain.SoftDeletable;
using System.ComponentModel.DataAnnotations;

namespace Moshrefy.Domain.Entities
{
    public class Exam : BaseEntity, ISoftDeletable
    {
        [Required]
        [MaxLength(100)]
        [MinLength(2)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public DateTime Date { get; set; }
        // Enum: Scheduled, Completed, Cancelled, Postponed
        [Required]
        public ExamStatus ExamStatus { get; set; }

        [Range(0, 1000)]
        public float? TotalMarks { get; set; }
        [Range(0, 1000)]
        public float? PassingMarks { get; set; }

        // Duration in minutes
        [Required]
        [Range(0, 300)]
        public int Duration { get; set; }
        public string? Description { get; set; }
        public bool IsDeleted { get; set; } = false;


        // -------- Collection of ExamResult ----------
        public ICollection<ExamResult> ExamResults { get; set; } = new List<ExamResult>();

        // -------- Collection of Attendances ----------
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

        // -------- Collection of Payments ----------
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();


        // -------- Relation with Course ----------
        // exam for specific course
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;


        // -------- Relation with Classroom ----------
        // exam held in specific classroom
        public int ClassroomId { get; set; }
        public Classroom Classroom { get; set; } = null!;




        // -------- Relation with TeacherCourse ----------
        // exam created by specific teacher for specific course
        public int TeacherCourseId { get; set; }
        public TeacherCourse? TeacherCourse { get; set; }






    }
}
