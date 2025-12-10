using FluentValidation;
using Moshrefy.Application.DTOs.Payment;

namespace Moshrefy.Application.Validators.Payment
{
    public class CreatePaymentDTOValidator : AbstractValidator<CreatePaymentDTO>
    {
        public CreatePaymentDTOValidator()
        {
            RuleFor(x => x.AmountPaid)
                .InclusiveBetween(0, 1000000).WithMessage("Amount paid must be between 0 and 1,000,000");

            RuleFor(x => x.PaymentMethods)
                .IsInEnum().WithMessage("Invalid payment method");

            RuleFor(x => x.PaymentStatus)
                .IsInEnum().WithMessage("Invalid payment status");

            RuleFor(x => x.InvoiceId)
                .GreaterThan(0).WithMessage("Invoice ID must be greater than 0");

            RuleFor(x => x.StudentId)
                .GreaterThan(0).WithMessage("Student ID must be greater than 0");
        }
    }
}
