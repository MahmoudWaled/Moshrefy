using Moshrefy.Domain.Enums;

namespace Moshrefy.Application.DTOs.Item
{
    public class CreateItemDTO
    {
        public string Name { get; set; } = default!;

        public decimal Price { get; set; }

        public int? ReservedByStudentId { get; set; }

        public ItemStatus ItemStatus { get; set; }
    }
}