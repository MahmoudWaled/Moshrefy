namespace Moshrefy.Application.DTOs.Invoice
{
    public class InvoiceResponseDTO
    {
        public int Id { get; set; }

        public decimal TotalAmount { get; set; }

        public bool IsPaid { get; set; }
    }
}