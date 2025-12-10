namespace Moshrefy.Web.Models.AcademicYear
{
    public class AcademicYearVM
    {
        public int Id { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public string? CreatedByName { get; set; }

        public DateTimeOffset? ModifiedAt { get; set; }

        public string? ModifiedByName { get; set; }

        public string Name { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}
