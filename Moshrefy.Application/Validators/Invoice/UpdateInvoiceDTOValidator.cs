using FluentValidation;
using Moshrefy.Application.DTOs.Invoice;

namespace Moshrefy.Application.Validators.Invoice
{
    public class UpdateInvoiceDTOValidator : AbstractValidator<UpdateInvoiceDTO>
    {
        public UpdateInvoiceDTOValidator()
        {
            RuleFor(x => x.TotalAmount)
                .InclusiveBetween(0, 10000000).WithMessage("Total amount must be between 0 and 10,000,000");
        }
    }
}
