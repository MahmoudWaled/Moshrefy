using FluentValidation;
using Moshrefy.Application.DTOs.Center;

namespace Moshrefy.Application.Validators.Center
{
    public class CreateCenterDTOValidator : AbstractValidator<CreateCenterDTO>
    {
        public CreateCenterDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is required")
                .MaximumLength(200).WithMessage("Address must not exceed 200 characters");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters");

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Invalid email format")
                .When(x => !string.IsNullOrEmpty(x.Email));

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone is required")
                .Matches(@"^[\d\s\-\+\(\)]+$").WithMessage("Invalid phone number format");
        }
    }
}
