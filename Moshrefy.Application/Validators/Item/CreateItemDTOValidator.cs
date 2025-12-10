using FluentValidation;
using Moshrefy.Application.DTOs.Item;

namespace Moshrefy.Application.Validators.Item
{
    public class CreateItemDTOValidator : AbstractValidator<CreateItemDTO>
    {
        public CreateItemDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MinimumLength(2).WithMessage("Name must be at least 2 characters")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters");

            RuleFor(x => x.Price)
                .InclusiveBetween(0, 1000000).WithMessage("Price must be between 0 and 1,000,000");

            RuleFor(x => x.ItemStatus)
                .IsInEnum().WithMessage("Invalid item status");

            RuleFor(x => x.ReservedByStudentId)
                .GreaterThan(0).WithMessage("Reserved by Student ID must be greater than 0")
                .When(x => x.ReservedByStudentId.HasValue);
        }
    }
}
