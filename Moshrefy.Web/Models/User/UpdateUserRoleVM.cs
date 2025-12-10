using Moshrefy.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Moshrefy.Web.Models.User
{
    public class UpdateUserRoleVM
    {
        [Required(ErrorMessage = "Role is required")]
        [Display(Name = "Role")]
        public RolesNames Role { get; set; }
    }
}
