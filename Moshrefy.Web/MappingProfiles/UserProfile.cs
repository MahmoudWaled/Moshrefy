using AutoMapper;
using Moshrefy.Application.DTOs.User;
using Moshrefy.Web.Models.User;

namespace Moshrefy.Web.MappingProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<CreateUserVM, CreateUserDTO>().ReverseMap();
            CreateMap<UpdateUserVM, UpdateUserDTO>().ReverseMap();
            CreateMap<UpdateUserProfileVM, UpdateUserProfileDTO>().ReverseMap();
            CreateMap<UpdateUserRoleVM, UpdateUserRoleDTO>().ReverseMap();
            CreateMap<UserVM, UserResponseDTO>().ReverseMap();
        }
    }
}
