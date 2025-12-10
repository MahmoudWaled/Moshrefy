using FluentValidation;
using Moshrefy.Application.DTOs.Auth;

namespace Moshrefy.Application.Validators.Auth
{
    public class RefreshTokenRequestDTOValidator : AbstractValidator<RefreshTokenRequestDTO>
    {
        public RefreshTokenRequestDTOValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("Refresh token is required");
        }
    }
}
