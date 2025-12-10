namespace Moshrefy.Application.DTOs.Classroom
{
    public class UpdateClassroomDTO
    {
        public string Name { get; set; } = default!;

        public string? Location { get; set; }

        public bool IsActive { get; set; }
    }
}