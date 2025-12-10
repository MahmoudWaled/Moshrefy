using Microsoft.AspNetCore.Mvc.Rendering;
using Moshrefy.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Moshrefy.Web.Models.Session
{
    public class UpdateSessionVM
    {
        [Display(Name = "Specific Date")]
        [DataType(DataType.Date)]
        public DateTime? SpecificDate { get; set; }

        [Display(Name = "Repeat Day of Week")]
        public DayOfWeek? RepeatDayOfWeek { get; set; }

        [Required(ErrorMessage = "Start time is required")]
        [Display(Name = "Start Time")]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "End time is required")]
        [Display(Name = "End Time")]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }

        [Required(ErrorMessage = "Session status is required")]
        [Display(Name = "Session Status")]
        public SessionStatus SessionStatus { get; set; }

        [Display(Name = "Is Paid")]
        public bool IsPaid { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Display(Name = "Price")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Display(Name = "Note")]
        public string? Note { get; set; }

        [Required(ErrorMessage = "Academic year is required")]
        [Display(Name = "Academic Year")]
        public int AcademicYearId { get; set; }

        [Required(ErrorMessage = "Teacher course is required")]
        [Display(Name = "Teacher Course")]
        public int TeacherCourseId { get; set; }

        [Required(ErrorMessage = "Classroom is required")]
        [Display(Name = "Classroom")]
        public int ClassroomId { get; set; }

        // For dropdown lists
        public SelectList? AcademicYears { get; set; }
        public SelectList? TeacherCourses { get; set; }
        public SelectList? Classrooms { get; set; }
    }
}
