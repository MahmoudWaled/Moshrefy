using FluentValidation;
using Moshrefy.Application.DTOs.Exam;

namespace Moshrefy.Application.Validators.Exam
{
    public class UpdateExamDTOValidator : AbstractValidator<UpdateExamDTO>
    {
        public UpdateExamDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MinimumLength(2).WithMessage("Name must be at least 2 characters")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters");

            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("Date is required");

            RuleFor(x => x.ExamStatus)
                .IsInEnum().WithMessage("Invalid exam status");

            RuleFor(x => x.TotalMarks)
                .InclusiveBetween(0, 1000).WithMessage("Total marks must be between 0 and 1000")
                .When(x => x.TotalMarks.HasValue);

            RuleFor(x => x.PassingMarks)
                .InclusiveBetween(0, 1000).WithMessage("Passing marks must be between 0 and 1000")
                .When(x => x.PassingMarks.HasValue);

            RuleFor(x => x.PassingMarks)
                .LessThanOrEqualTo(x => x.TotalMarks)
                .WithMessage("Passing marks cannot exceed total marks")
                .When(x => x.PassingMarks.HasValue && x.TotalMarks.HasValue);

            RuleFor(x => x.Duration)
                .InclusiveBetween(0, 300).WithMessage("Duration must be between 0 and 300 minutes");

            RuleFor(x => x.CourseId)
                .GreaterThan(0).WithMessage("Course ID must be greater than 0");

            RuleFor(x => x.ClassroomId)
                .GreaterThan(0).WithMessage("Classroom ID must be greater than 0");

            RuleFor(x => x.TeacherCourseId)
                .GreaterThan(0).WithMessage("Teacher Course ID must be greater than 0");
        }
    }
}
