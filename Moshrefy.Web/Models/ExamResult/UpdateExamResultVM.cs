using Moshrefy.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Moshrefy.Web.Models.ExamResult
{
    public class UpdateExamResultVM
    {
        [Required(ErrorMessage = "Marks is required")]
        [Display(Name = "Marks")]
        public float Marks { get; set; }

        [Required(ErrorMessage = "Exam result status is required")]
        [Display(Name = "Result Status")]
        public ExamResultStatus ExamResultStatus { get; set; }

        [Display(Name = "Note")]
        public string? Note { get; set; }

        [Required(ErrorMessage = "Student is required")]
        [Display(Name = "Student")]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Exam is required")]
        [Display(Name = "Exam")]
        public int ExamId { get; set; }
    }
}
