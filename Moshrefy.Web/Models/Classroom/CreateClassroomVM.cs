using System.ComponentModel.DataAnnotations;

namespace Moshrefy.Web.Models.Classroom
{
    public class CreateClassroomVM
    {
        [Required(ErrorMessage = "Classroom name is required")]
        [Display(Name = "Classroom Name")]
        public string Name { get; set; } = default!;

        [Display(Name = "Location")]
        public string? Location { get; set; }
    }
}
