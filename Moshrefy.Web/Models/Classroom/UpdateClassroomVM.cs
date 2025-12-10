using System.ComponentModel.DataAnnotations;

namespace Moshrefy.Web.Models.Classroom
{
    public class UpdateClassroomVM
    {
        [Required(ErrorMessage = "Classroom name is required")]
        [Display(Name = "Classroom Name")]
        public string Name { get; set; } = default!;

        [Display(Name = "Location")]
        public string? Location { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
    }
}
