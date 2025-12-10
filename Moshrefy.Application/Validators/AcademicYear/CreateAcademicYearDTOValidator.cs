using FluentValidation;
using Moshrefy.Application.DTOs.AcademicYear;

namespace Moshrefy.Application.Validators.AcademicYear
{
    public class CreateAcademicYearDTOValidator : AbstractValidator<CreateAcademicYearDTO>
    {
        public CreateAcademicYearDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50).WithMessage("Name must not exceed 50 characters.");
        }
    }
}
