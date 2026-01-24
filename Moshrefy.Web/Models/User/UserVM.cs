using System.ComponentModel.DataAnnotations;

namespace Moshrefy.Web.Models.User
{
    public class UserVM
    {
        public string Id { get; set; } = default!;

        public string Name { get; set; } = default!;

        public string UserName { get; set; } = default!;

        public string? Email { get; set; }
        [Display(Name = "Phone")]
        public string? PhoneNumber { get; set; }

        public int? CenterId { get; set; }

        public string? CenterName { get; set; }
        [Display(Name = "Role")]
        public string? RoleName { get; set; }

        public DateTime CreatedAt { get; set; }

        public string? CreatedById { get; set; }

        public string? CreatedByName { get; set; }
        [Display(Name = "Status")]
        public bool IsActive { get; set; }
        [Display(Name = "state")]
        public bool IsDeleted { get; set; }
    }
}
