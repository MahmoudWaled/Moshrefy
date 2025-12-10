using System.ComponentModel.DataAnnotations;

namespace Moshrefy.Web.Models.Invoice
{
    public class UpdateInvoiceVM
    {
        [Required(ErrorMessage = "Total amount is required")]
        [Display(Name = "Total Amount")]
        [DataType(DataType.Currency)]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Is Paid")]
        public bool IsPaid { get; set; }
    }
}
