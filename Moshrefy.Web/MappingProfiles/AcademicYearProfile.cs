using AutoMapper;
using Moshrefy.Application.DTOs.AcademicYear;
using Moshrefy.Application.DTOs.Common;
using Moshrefy.Web.Models.AcademicYear;

namespace Moshrefy.Web.MappingProfiles
{
    public class AcademicYearProfile : Profile
    {
        public AcademicYearProfile()
        {
            CreateMap<CreateAcademicYearVM, CreateAcademicYearDTO>().ReverseMap();
            CreateMap<UpdateAcademicYearVM, UpdateAcademicYearDTO>().ReverseMap();
            CreateMap<AcademicYearVM, AcademicYearResponseDTO>().ReverseMap();
            CreateMap<PaginatedResult<AcademicYearResponseDTO>, PaginatedResult<AcademicYearVM>>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));
        }
    }
}
