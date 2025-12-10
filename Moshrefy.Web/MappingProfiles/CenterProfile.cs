using AutoMapper;
using Moshrefy.Application.DTOs.Center;
using Moshrefy.Web.Models.Center;

namespace Moshrefy.Web.MappingProfiles
{
    public class CenterProfile : Profile
    {
        public CenterProfile()
        {
            CreateMap<CreateCenterVM, CreateCenterDTO>().ReverseMap();
            CreateMap<UpdateCenterVM, UpdateCenterDTO>().ReverseMap();
            CreateMap<CenterVM, CenterResponseDTO>().ReverseMap();
        }
    }
}
