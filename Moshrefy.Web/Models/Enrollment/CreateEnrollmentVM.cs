using System.ComponentModel.DataAnnotations;

namespace Moshrefy.Web.Models.Enrollment
{
    public class CreateEnrollmentVM
    {
        [Required(ErrorMessage = "Student is required")]
        [Display(Name = "Student")]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Course is required")]
        [Display(Name = "Course")]
        public int CourseId { get; set; }
    }
}
