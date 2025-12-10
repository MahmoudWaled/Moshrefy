using System.ComponentModel.DataAnnotations;

namespace Moshrefy.Web.Models.AcademicYear
{
    public class CreateAcademicYearVM
    {
        [Required(ErrorMessage = "Academic year name is required")]
        [Display(Name = "Academic Year Name")]
        public string Name { get; set; } = string.Empty;
    }
}
