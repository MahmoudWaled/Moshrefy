namespace Moshrefy.Web.Models.TeacherItem
{
    public class TeacherItemVM
    {
        public int Id { get; set; }

        public int TeacherId { get; set; }

        public string? TeacherName { get; set; }

        public int ItemId { get; set; }

        public string? ItemName { get; set; }

        public bool IsActive { get; set; }
    }
}
