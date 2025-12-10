using FluentValidation;
using Moshrefy.Application.DTOs.Classroom;

namespace Moshrefy.Application.Validators.Classroom
{
    public class CreateClassroomDTOValidator : AbstractValidator<CreateClassroomDTO>
    {
        public CreateClassroomDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters");

            RuleFor(x => x.Location)
                .MaximumLength(200).WithMessage("Location must not exceed 200 characters");
        }
    }
}
