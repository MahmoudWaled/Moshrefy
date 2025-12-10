using Moshrefy.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Moshrefy.Domain.Entities
{
    public class ExamResult : BaseEntity
    {
        // Marks obtained by the student
        [Required]
        [Range(0, 1000)]
        public float Marks { get; set; }

        // Enum: passed, failed, Incomplete
        [Required]
        public ExamResultStatus ExamResultStatus { get; set; }

        [MaxLength(500)]
        [MinLength(3)]
        public string? Note { get; set; } = string.Empty;

        public bool IsDeleted { get; set; }

        // -------- Relation with Student ----------
        // result for specific student
        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;


        // -------- Relation with Exam ----------
        // result for specific exam
        public int ExamId { get; set; }
        public Exam Exam { get; set; } = null!;


    }
}
