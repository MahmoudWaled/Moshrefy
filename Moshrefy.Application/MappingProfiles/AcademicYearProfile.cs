using AutoMapper;
using Moshrefy.Application.DTOs.AcademicYear;
using Moshrefy.Domain.Entities;

namespace Moshrefy.Application.MappingProfiles
{
    public class AcademicYearProfile : Profile
    {
        public AcademicYearProfile()
        {
            // Entity to DTO
            CreateMap<AcademicYear, AcademicYearResponseDTO>()
                .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.CreatedBy.UserName))
                .ForMember(dest => dest.ModifiedByName, opt => opt.MapFrom(src => src.ModifiedBy != null ? src.ModifiedBy.UserName : null));

            CreateMap<CreateAcademicYearDTO, AcademicYear>();
            CreateMap<UpdateAcademicYearDTO, AcademicYear>();
        }
    }
}
