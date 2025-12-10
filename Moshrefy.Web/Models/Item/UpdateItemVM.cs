using Moshrefy.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Moshrefy.Web.Models.Item
{
    public class UpdateItemVM
    {
        [Required(ErrorMessage = "Item name is required")]
        [Display(Name = "Item Name")]
        public string Name { get; set; } = default!;

        [Required(ErrorMessage = "Price is required")]
        [Display(Name = "Price")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Display(Name = "Reserved By Student")]
        public int? ReservedByStudentId { get; set; }

        [Required(ErrorMessage = "Item status is required")]
        [Display(Name = "Item Status")]
        public ItemStatus ItemStatus { get; set; }
    }
}
