using FluentValidation;
using Moshrefy.Application.DTOs.Teacher;

namespace Moshrefy.Application.Validators.Teacher
{
    public class CreateTeacherDTOValidator : AbstractValidator<CreateTeacherDTO>
    {
        public CreateTeacherDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MinimumLength(2).WithMessage("Name must be at least 2 characters")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters");

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Invalid email format")
                .When(x => !string.IsNullOrEmpty(x.Email));

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone is required")
                .Matches(@"^[\d\s\-\+\(\)]+$").WithMessage("Invalid phone number format");

            RuleFor(x => x.Specialization)
                .NotEmpty().WithMessage("Specialization is required")
                .MinimumLength(2).WithMessage("Specialization must be at least 2 characters")
                .MaximumLength(100).WithMessage("Specialization must not exceed 100 characters");
        }
    }
}
