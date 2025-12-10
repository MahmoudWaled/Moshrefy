using FluentValidation;
using Moshrefy.Application.DTOs.Session;

namespace Moshrefy.Application.Validators.Session
{
    public class CreateSessionDTOValidator : AbstractValidator<CreateSessionDTO>
    {
        public CreateSessionDTOValidator()
        {
            RuleFor(x => x.StartTime)
                .NotEmpty().WithMessage("Start time is required");

            RuleFor(x => x.EndTime)
                .NotEmpty().WithMessage("End time is required")
                .GreaterThan(x => x.StartTime).WithMessage("End time must be after start time");

            RuleFor(x => x.SessionStatus)
                .IsInEnum().WithMessage("Invalid session status");

            RuleFor(x => x.Price)
                .InclusiveBetween(0, 1000000).WithMessage("Price must be between 0 and 1,000,000");

            RuleFor(x => x.Note)
                .MinimumLength(3).WithMessage("Note must be at least 3 characters")
                .MaximumLength(500).WithMessage("Note must not exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.Note));

            RuleFor(x => x.AcademicYearId)
                .GreaterThan(0).WithMessage("Academic Year ID must be greater than 0");

            RuleFor(x => x.TeacherCourseId)
                .GreaterThan(0).WithMessage("Teacher Course ID must be greater than 0");

            RuleFor(x => x.ClassroomId)
                .GreaterThan(0).WithMessage("Classroom ID must be greater than 0");

            RuleFor(x => x)
                .Must(x => x.SpecificDate.HasValue || x.RepeatDayOfWeek.HasValue)
                .WithMessage("Either Specific Date or Repeat Day of Week must be provided");

            RuleFor(x => x.RepeatDayOfWeek)
                .IsInEnum().WithMessage("Invalid day of week")
                .When(x => x.RepeatDayOfWeek.HasValue);
        }
    }
}
