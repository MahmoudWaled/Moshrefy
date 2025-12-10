using AutoMapper;
using Moshrefy.Application.DTOs.AcademicYear;
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
        }
    }
}
