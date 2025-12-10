namespace Moshrefy.Application.DTOs.TeacherItem
{
    public class UpdateTeacherItemDTO
    {
        public int TeacherId { get; set; }

        public int ItemId { get; set; }

        public bool IsActive { get; set; }
    }
}