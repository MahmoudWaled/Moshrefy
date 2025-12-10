using System.ComponentModel.DataAnnotations;

namespace Moshrefy.Web.Models.Course
{
    public class UpdateCourseVM
    {
        [Required(ErrorMessage = "Course name is required")]
        [Display(Name = "Course Name")]
        public string Name { get; set; } = default!;

        [Required(ErrorMessage = "Academic year is required")]
        [Display(Name = "Academic Year")]
        public int AcademicYearId { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
    }
}
