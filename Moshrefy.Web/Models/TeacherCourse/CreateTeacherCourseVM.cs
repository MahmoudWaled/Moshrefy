using System.ComponentModel.DataAnnotations;

namespace Moshrefy.Web.Models.TeacherCourse
{
    public class CreateTeacherCourseVM
    {
        [Required(ErrorMessage = "Teacher is required")]
        [Display(Name = "Teacher")]
        public int TeacherId { get; set; }

        [Required(ErrorMessage = "Course is required")]
        [Display(Name = "Course")]
        public int CourseId { get; set; }
    }
}
