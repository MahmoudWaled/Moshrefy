using Moshrefy.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Moshrefy.Domain.Entities
{
    public class Attendance : BaseEntity
    {
        // Enum: Present, Absent
        [Required]
        public AttendanceStatus AttendanceStatus { get; set; }

        [MaxLength(500)]
        [MinLength(3)]
        public string? Note { get; set; } = string.Empty;



        // -------- Relation with Student ----------
        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;



        // -------- Relation with Session ----------
        public int SessionId { get; set; }
        public Session Session { get; set; } = null!;


        // -------- Relation with Exam ----------
        public int ExamId { get; set; }
        public Exam Exam { get; set; } = null!;


    }
}
