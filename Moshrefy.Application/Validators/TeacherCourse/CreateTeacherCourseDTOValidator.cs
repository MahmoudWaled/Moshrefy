using FluentValidation;
using Moshrefy.Application.DTOs.TeacherCourse;

namespace Moshrefy.Application.Validators.TeacherCourse
{
    public class CreateTeacherCourseDTOValidator : AbstractValidator<CreateTeacherCourseDTO>
    {
        public CreateTeacherCourseDTOValidator()
        {
            RuleFor(x => x.TeacherId)
                .GreaterThan(0).WithMessage("Teacher ID must be greater than 0");

            RuleFor(x => x.CourseId)
                .GreaterThan(0).WithMessage("Course ID must be greater than 0");
        }
    }
}
