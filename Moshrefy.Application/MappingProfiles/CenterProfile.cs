using AutoMapper;
using Moshrefy.Application.DTOs.Center;
using Moshrefy.Domain.Entities;

namespace Moshrefy.Application.MappingProfiles
{
    public class CenterProfile : Profile
    {
        public CenterProfile()
        {
            CreateMap<CreateCenterDTO, Center>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<UpdateCenterDTO, Center>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Center, CenterResponseDTO>();
        }
    }
}