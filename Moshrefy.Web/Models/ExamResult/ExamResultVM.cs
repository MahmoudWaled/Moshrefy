using Moshrefy.Domain.Enums;

namespace Moshrefy.Web.Models.ExamResult
{
    public class ExamResultVM
    {
        public int Id { get; set; }

        public float Marks { get; set; }

        public ExamResultStatus ExamResultStatus { get; set; }

        public string? Note { get; set; }

        public int StudentId { get; set; }

        public string? StudentName { get; set; }

        public int ExamId { get; set; }

        public string? ExamName { get; set; }
    }
}
