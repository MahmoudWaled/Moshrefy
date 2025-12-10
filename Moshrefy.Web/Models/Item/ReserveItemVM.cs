using System.ComponentModel.DataAnnotations;

namespace Moshrefy.Web.Models.Item
{
    public class ReserveItemVM
    {
        [Required(ErrorMessage = "Student is required")]
        [Display(Name = "Student")]
        public int StudentId { get; set; }
    }
}
