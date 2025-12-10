namespace Moshrefy.Application.DTOs.Teacher
{
    public class UpdateTeacherDTO
    {
        public string Name { get; set; } = default!;

        public string? Email { get; set; }

        public string Phone { get; set; } = default!;

        public string Specialization { get; set; } = default!;

        public bool IsActive { get; set; }
    }
}