using FluentValidation;
using Moshrefy.API.Controllers;

namespace Moshrefy.Application.Validators.Item
{
    public class ReserveItemDTOValidator : AbstractValidator<ReserveItemDTO>
    {
        public ReserveItemDTOValidator()
        {
            RuleFor(x => x.StudentId)
                .GreaterThan(0).WithMessage("Student ID must be greater than 0");
        }
    }
}
