using Moshrefy.Domain.Enums;

namespace Moshrefy.Application.DTOs.ExamResult
{
    public class CreateExamResultDTO
    {
        public float Marks { get; set; }

        public ExamResultStatus ExamResultStatus { get; set; }

        public string? Note { get; set; }

        public int StudentId { get; set; }

        public int ExamId { get; set; }
    }
}