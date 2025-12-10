namespace Moshrefy.Web.Models.Invoice
{
    public class InvoiceVM
    {
        public int Id { get; set; }

        public decimal TotalAmount { get; set; }

        public bool IsPaid { get; set; }
    }
}
