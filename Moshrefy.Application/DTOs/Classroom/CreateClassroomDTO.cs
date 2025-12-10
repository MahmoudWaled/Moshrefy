namespace Moshrefy.Application.DTOs.Classroom
{
    public class CreateClassroomDTO
    {
        public string Name { get; set; } = default!;

        public string? Location { get; set; }
    }
}