using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Moshrefy.Domain.Entities
{
    public class Invoice : BaseEntity
    {
        [Required]
        [Range(0, 10000000)]
        [Precision(18, 2)]
        public decimal TotalAmount { get; set; }
        public bool IsPaid { get; set; } = false;
        public bool IsDeleted { get; set; }

        // -------- Collection of Payments ----------
        public List<Payment> Payments { get; set; } = new List<Payment>();

    }
}
