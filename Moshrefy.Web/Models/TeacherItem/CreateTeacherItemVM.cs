using System.ComponentModel.DataAnnotations;

namespace Moshrefy.Web.Models.TeacherItem
{
    public class CreateTeacherItemVM
    {
        [Required(ErrorMessage = "Teacher is required")]
        [Display(Name = "Teacher")]
        public int TeacherId { get; set; }

        [Required(ErrorMessage = "Item is required")]
        [Display(Name = "Item")]
        public int ItemId { get; set; }
    }
}
