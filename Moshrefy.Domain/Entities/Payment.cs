using Microsoft.EntityFrameworkCore;
using Moshrefy.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Moshrefy.Domain.Entities
{
    public class Payment : BaseEntity
    {
        [Required]
        [Range(0, 1000000)]
        [Precision(18, 2)]
        public decimal AmountPaid { get; set; }
        // Enum: Cash, CreditCard, MobileWallet, Instapay, BankTransfer ,Other
        [Required]
        public PaymentMethods paymentMethods { get; set; }
        // New payment or refund
        [Required]
        public PaymentStatus PaymentStatus { get; set; }
        public string? Notes { get; set; }
        // -------- Relation with Invoice --------
        public int InvoiceId { get; set; }
        public Invoice Invoice { get; set; } = null!;

        // -------- Relation with Student ----------
        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;


        // ------- Relation with Session ----------
        public int SessionId { get; set; }
        public Session? Session { get; set; }


        // ------- Relation with TeacherItem ----------
        public int TeacherItemId { get; set; }
        public TeacherItem? TeacherItem { get; set; }


        // ------- Relation with Exam ----------
        public int ExamId { get; set; }
        public Exam? Exam { get; set; }


    }
}
