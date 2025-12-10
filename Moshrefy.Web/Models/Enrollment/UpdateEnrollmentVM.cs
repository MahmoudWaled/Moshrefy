using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Moshrefy.Web.Models.Enrollment
{
    public class UpdateEnrollmentVM
    {
        [Required(ErrorMessage = "Student is required")]
        [Display(Name = "Student")]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Course is required")]
        [Display(Name = "Course")]
        public int CourseId { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        // For dropdown lists
        public SelectList? Students { get; set; }
        public SelectList? Courses { get; set; }
    }
}
