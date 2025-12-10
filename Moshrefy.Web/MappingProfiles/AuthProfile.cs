using AutoMapper;
using Moshrefy.Application.DTOs.User;

namespace Moshrefy.Web.MappingProfiles
{
    public class AuthProfile : Profile
    {
        public AuthProfile()
        {

            CreateMap<Models.LoginVM, LoginUserDTO>().ReverseMap(); 

        }
    }
}
