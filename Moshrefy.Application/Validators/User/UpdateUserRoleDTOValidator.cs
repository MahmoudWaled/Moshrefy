using FluentValidation;
using Moshrefy.Application.DTOs.User;

namespace Moshrefy.Application.Validators.User
{
    public class UpdateUserRoleDTOValidator : AbstractValidator<UpdateUserRoleDTO>
    {
        public UpdateUserRoleDTOValidator()
        {
            RuleFor(x => x.Role)
                .IsInEnum().WithMessage("Invalid role");
        }
    }
}
