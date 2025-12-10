using Moshrefy.Domain.Enums;

namespace Moshrefy.Web.Models.Payment
{
    public class PaymentVM
    {
        public int Id { get; set; }

        public decimal AmountPaid { get; set; }

        public PaymentMethods PaymentMethods { get; set; }

        public PaymentStatus PaymentStatus { get; set; }

        public string? Notes { get; set; }

        public int InvoiceId { get; set; }

        public int StudentId { get; set; }

        public string? StudentName { get; set; }

        public int SessionId { get; set; }

        public int TeacherItemId { get; set; }

        public int ExamId { get; set; }
    }
}
