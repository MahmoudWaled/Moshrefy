using AutoMapper;
using Moshrefy.Application.DTOs.Auth;
using Moshrefy.Application.DTOs.User;
using Moshrefy.Web.Models;
using Moshrefy.Web.Models.Auth;

namespace Moshrefy.Web.MappingProfiles
{
    public class AuthProfile : Profile
    {
        public AuthProfile()
        {
            CreateMap<LoginVM, LoginUserDTO>().ReverseMap();
            CreateMap<ChangePasswordVM, ChangePasswordDTO>().ReverseMap();
            CreateMap<RefreshTokenRequestVM, RefreshTokenRequestDTO>().ReverseMap();
            CreateMap<RequestResetPasswordVM, RequestResetPasswordDTO>().ReverseMap();
            CreateMap<ResetPasswordVM, ResetPasswordDTO>().ReverseMap();
        }
    }
}
