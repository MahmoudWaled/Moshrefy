using FluentValidation;
using Moshrefy.Application.DTOs.Student;

namespace Moshrefy.Application.Validators.Student
{
    public class CreateStudentDTOValidator : AbstractValidator<CreateStudentDTO>
    {
        public CreateStudentDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MinimumLength(2).WithMessage("Name must be at least 2 characters")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters");

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Invalid email format")
                .When(x => !string.IsNullOrEmpty(x.Email));

            RuleFor(x => x.FirstPhone)
                .NotEmpty().WithMessage("First phone is required")
                .Matches(@"^[\d\s\-\+\(\)]+$").WithMessage("Invalid phone number format");

            RuleFor(x => x.SecondPhone)
                .Matches(@"^[\d\s\-\+\(\)]+$").WithMessage("Invalid phone number format")
                .When(x => !string.IsNullOrEmpty(x.SecondPhone));

            RuleFor(x => x.FatherPhone)
                .Matches(@"^[\d\s\-\+\(\)]+$").WithMessage("Invalid phone number format")
                .When(x => !string.IsNullOrEmpty(x.FatherPhone));

            RuleFor(x => x.MotherPhone)
                .Matches(@"^[\d\s\-\+\(\)]+$").WithMessage("Invalid phone number format")
                .When(x => !string.IsNullOrEmpty(x.MotherPhone));

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Date of birth is required")
                .LessThan(DateTime.Now).WithMessage("Date of birth must be in the past");
        }
    }
}
