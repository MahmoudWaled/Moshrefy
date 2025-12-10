namespace Moshrefy.Application.DTOs.Invoice
{
    public class UpdateInvoiceDTO
    {
        public decimal TotalAmount { get; set; }

        public bool IsPaid { get; set; }
    }
}