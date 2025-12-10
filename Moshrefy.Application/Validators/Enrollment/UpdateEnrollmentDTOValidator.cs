using FluentValidation;
using Moshrefy.Application.DTOs.Enrollment;

namespace Moshrefy.Application.Validators.Enrollment
{
    public class UpdateEnrollmentDTOValidator : AbstractValidator<UpdateEnrollmentDTO>
    {
        public UpdateEnrollmentDTOValidator()
        {
            RuleFor(x => x.StudentId)
                .GreaterThan(0).WithMessage("Student ID must be greater than 0");

            RuleFor(x => x.CourseId)
                .GreaterThan(0).WithMessage("Course ID must be greater than 0");
        }
    }
}
