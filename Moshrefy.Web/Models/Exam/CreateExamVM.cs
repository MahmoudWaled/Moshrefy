using Microsoft.AspNetCore.Mvc.Rendering;
using Moshrefy.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Moshrefy.Web.Models.Exam
{
    public class CreateExamVM
    {
        [Required(ErrorMessage = "Exam name is required")]
        [Display(Name = "Exam Name")]
        public string Name { get; set; } = default!;

        [Required(ErrorMessage = "Date is required")]
        [Display(Name = "Exam Date")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Exam status is required")]
        [Display(Name = "Exam Status")]
        public ExamStatus ExamStatus { get; set; }

        [Display(Name = "Total Marks")]
        public float? TotalMarks { get; set; }

        [Display(Name = "Passing Marks")]
        public float? PassingMarks { get; set; }

        [Required(ErrorMessage = "Duration is required")]
        [Display(Name = "Duration (minutes)")]
        public int Duration { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Course is required")]
        [Display(Name = "Course")]
        public int CourseId { get; set; }

        [Required(ErrorMessage = "Classroom is required")]
        [Display(Name = "Classroom")]
        public int ClassroomId { get; set; }

        [Required(ErrorMessage = "Teacher course is required")]
        [Display(Name = "Teacher Course")]
        public int TeacherCourseId { get; set; }

        // For dropdown lists
        public SelectList? Courses { get; set; }
        public SelectList? Classrooms { get; set; }
        public SelectList? TeacherCourses { get; set; }
    }
}
