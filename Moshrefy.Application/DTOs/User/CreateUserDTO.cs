using Moshrefy.Domain.Enums;
using System.Text.Json.Serialization;

namespace Moshrefy.Application.DTOs.User
{
    public class CreateUserDTO
    {
        public string Name { get; set; } = default!;

        public string UserName { get; set; } = default!;

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string Password { get; set; } = default!;

        public int? CenterId { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RolesNames RoleName { get; set; }
    }
}
