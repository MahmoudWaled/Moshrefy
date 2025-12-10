using Microsoft.AspNetCore.Mvc.Rendering;
using Moshrefy.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Moshrefy.Web.Models.Attendance
{
    public class CreateAttendanceVM
    {
        [Required(ErrorMessage = "Attendance status is required")]
        [Display(Name = "Attendance Status")]
        public AttendanceStatus AttendanceStatus { get; set; }

        [Display(Name = "Note")]
        public string? Note { get; set; }

        [Required(ErrorMessage = "Student is required")]
        [Display(Name = "Student")]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Session is required")]
        [Display(Name = "Session")]
        public int SessionId { get; set; }

        [Required(ErrorMessage = "Exam is required")]
        [Display(Name = "Exam")]
        public int ExamId { get; set; }

        // For dropdown lists
        public SelectList? Students { get; set; }
        public SelectList? Sessions { get; set; }
        public SelectList? Exams { get; set; }
    }
}
