using FluentValidation;
using Moshrefy.Application.DTOs.TeacherItem;

namespace Moshrefy.Application.Validators.TeacherItem
{
    public class UpdateTeacherItemDTOValidator : AbstractValidator<UpdateTeacherItemDTO>
    {
        public UpdateTeacherItemDTOValidator()
        {
            RuleFor(x => x.TeacherId)
                .GreaterThan(0).WithMessage("Teacher ID must be greater than 0");

            RuleFor(x => x.ItemId)
                .GreaterThan(0).WithMessage("Item ID must be greater than 0");
        }
    }
}
