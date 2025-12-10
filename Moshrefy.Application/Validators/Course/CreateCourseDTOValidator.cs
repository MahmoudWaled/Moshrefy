using FluentValidation;
using Moshrefy.Application.DTOs.Course;

namespace Moshrefy.Application.Validators.Course
{
    public class CreateCourseDTOValidator : AbstractValidator<CreateCourseDTO>
    {
        public CreateCourseDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MinimumLength(2).WithMessage("Name must be at least 2 characters")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters");

            RuleFor(x => x.AcademicYearId)
                .GreaterThan(0).WithMessage("Academic Year ID must be greater than 0");
        }
    }
}
