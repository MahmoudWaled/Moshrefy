using Microsoft.EntityFrameworkCore;
using Moshrefy.Domain.Enums;
using Moshrefy.Domain.SoftDeletable;
using System.ComponentModel.DataAnnotations;

namespace Moshrefy.Domain.Entities
{
    public class Session : BaseEntity, ISoftDeletable
    {
        /* Example:
         * SpecificDate: 2025-10-15
         * RepeatDayOfWeek: null
         * StartTime: 19:00
         * EndTime: 21:00
         * 
         * SpecificDate: null
         * RepeatDayOfWeek: Monday
         * StartTime: 19:00
         * EndTime: 21:00
         */
        public DateTime? SpecificDate { get; set; }
        public DayOfWeek? RepeatDayOfWeek { get; set; }

        // 19:00 pm
        [Required]
        public TimeSpan StartTime { get; set; }

        //  21:00 pm
        [Required]
        public TimeSpan EndTime { get; set; }

        // Enum: Scheduled, Completed, Canceled, Postponed
        [Required]
        public SessionStatus SessionStatus { get; set; }


        [Required]
        public bool IsPaid { get; set; }

        [Required]
        [Range(0, 1000000)]
        [Precision(18, 2)]
        public decimal Price { get; set; }

        // any notes about the session
        [MaxLength(500)]
        [MinLength(3)]
        public string? Note { get; set; } = string.Empty;

        public bool IsDeleted { get; set; } = false;

        // -------- Relation with AcademicYear ----------
        public int AcademicYearId { get; set; }
        public AcademicYear AcademicYear { get; set; } = null!;


        // -------- Relation with TeacherCourse ----------
        public int TeacherCourseId { get; set; }
        public TeacherCourse TeacherCourse { get; set; } = null!;


        // -------- Relation with Classroom ----------
        public int ClassroomId { get; set; }
        public Classroom Classroom { get; set; } = null!;


        // ---------- Collection Of Attendance  ----------
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();


        // ---------- Collection Of Payments  ----------
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
