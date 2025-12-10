namespace Moshrefy.Application.DTOs.Classroom
{
    public class ClassroomResponseDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = default!;

        public string? Location { get; set; }

        public bool IsActive { get; set; }
    }
}