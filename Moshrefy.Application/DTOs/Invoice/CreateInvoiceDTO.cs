namespace Moshrefy.Application.DTOs.Invoice
{
    public class CreateInvoiceDTO
    {
        public decimal TotalAmount { get; set; }

        public bool IsPaid { get; set; }
    }
}