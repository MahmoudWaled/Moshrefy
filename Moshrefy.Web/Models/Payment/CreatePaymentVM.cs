using Microsoft.AspNetCore.Mvc.Rendering;
using Moshrefy.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Moshrefy.Web.Models.Payment
{
    public class CreatePaymentVM
    {
        [Required(ErrorMessage = "Amount paid is required")]
        [Display(Name = "Amount Paid")]
        [DataType(DataType.Currency)]
        public decimal AmountPaid { get; set; }

        [Required(ErrorMessage = "Payment method is required")]
        [Display(Name = "Payment Method")]
        public PaymentMethods PaymentMethods { get; set; }

        [Required(ErrorMessage = "Payment status is required")]
        [Display(Name = "Payment Status")]
        public PaymentStatus PaymentStatus { get; set; }

        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        [Required(ErrorMessage = "Invoice is required")]
        [Display(Name = "Invoice")]
        public int InvoiceId { get; set; }

        [Required(ErrorMessage = "Student is required")]
        [Display(Name = "Student")]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Session is required")]
        [Display(Name = "Session")]
        public int SessionId { get; set; }

        [Required(ErrorMessage = "Teacher item is required")]
        [Display(Name = "Teacher Item")]
        public int TeacherItemId { get; set; }

        [Required(ErrorMessage = "Exam is required")]
        [Display(Name = "Exam")]
        public int ExamId { get; set; }

        // For dropdown lists
        public SelectList? Invoices { get; set; }
        public SelectList? Students { get; set; }
        public SelectList? Sessions { get; set; }
        public SelectList? TeacherItems { get; set; }
        public SelectList? Exams { get; set; }
    }
}
