using AutoMapper;
using Moshrefy.Application.DTOs.Session;
using Moshrefy.Web.Models.Session;

namespace Moshrefy.Web.MappingProfiles
{
    public class SessionProfile : Profile
    {
        public SessionProfile()
        {
            CreateMap<CreateSessionVM, CreateSessionDTO>().ReverseMap();
            CreateMap<UpdateSessionVM, UpdateSessionDTO>().ReverseMap();
            CreateMap<SessionVM, SessionResponseDTO>().ReverseMap();
        }
    }
}
