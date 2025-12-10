using FluentValidation;
using Moshrefy.Application.DTOs.Attendance;

namespace Moshrefy.Application.Validators.Attendance
{
    public class CreateAttendanceDTOValidator : AbstractValidator<CreateAttendanceDTO>
    {
        public CreateAttendanceDTOValidator()
        {
            RuleFor(x => x.AttendanceStatus)
                .IsInEnum().WithMessage("Invalid attendance status.");

            RuleFor(x => x.StudentId)
                .GreaterThan(0).WithMessage("Student ID must be greater than 0");

            RuleFor(x => x.SessionId)
                           .GreaterThan(0).WithMessage("Session ID must be greater than 0");

            RuleFor(x => x.Note)
                .MaximumLength(500).WithMessage("Note must not exceed 500 characters");
        }
    }
}
