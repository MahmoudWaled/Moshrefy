namespace Moshrefy.Web.Models.Classroom
{
    public class ClassroomVM
    {
        public int Id { get; set; }

        public string Name { get; set; } = default!;

        public string? Location { get; set; }

        public bool IsActive { get; set; }
    }
}
