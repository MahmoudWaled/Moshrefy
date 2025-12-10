using FluentValidation;
using Moshrefy.Application.DTOs.ExamResult;

namespace Moshrefy.Application.Validators.ExamResult
{
    public class CreateExamResultDTOValidator : AbstractValidator<CreateExamResultDTO>
    {
        public CreateExamResultDTOValidator()
        {
            RuleFor(x => x.Marks)
                .InclusiveBetween(0, 1000).WithMessage("Marks must be between 0 and 1000");

            RuleFor(x => x.ExamResultStatus)
                .IsInEnum().WithMessage("Invalid exam result status");

            RuleFor(x => x.Note)
                .MinimumLength(3).WithMessage("Note must be at least 3 characters")
                .MaximumLength(500).WithMessage("Note must not exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.Note));

            RuleFor(x => x.StudentId)
                .GreaterThan(0).WithMessage("Student ID must be greater than 0");

            RuleFor(x => x.ExamId)
                .GreaterThan(0).WithMessage("Exam ID must be greater than 0");
        }
    }
}
