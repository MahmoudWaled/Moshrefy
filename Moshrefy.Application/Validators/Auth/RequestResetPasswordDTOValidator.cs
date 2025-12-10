using FluentValidation;
using Moshrefy.Application.DTOs.Auth;

namespace Moshrefy.Application.Validators.Auth
{
    public class RequestResetPasswordDTOValidator : AbstractValidator<RequestResetPasswordDTO>
    {
        public RequestResetPasswordDTOValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");
        }
    }
}
