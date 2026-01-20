using System.ComponentModel.DataAnnotations;

namespace Moshrefy.Web.Models.Enrollment
{
    public class BulkEnrollmentVM
    {
        // For mode: 1 student → many courses
        public int? StudentId { get; set; }
        public List<int> CourseIds { get; set; } = new List<int>();

        // For mode: 1 course → many students
        public int? CourseId { get; set; }
        public List<int> StudentIds { get; set; } = new List<int>();
    }
}
