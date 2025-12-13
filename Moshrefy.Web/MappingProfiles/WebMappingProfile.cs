using AutoMapper;
using Moshrefy.Application.DTOs.AcademicYear;
using Moshrefy.Application.DTOs.Course;
using Moshrefy.Web.Models.AcademicYear;
using Moshrefy.Web.Models.Course;

namespace Moshrefy.Web.MappingProfiles
{
    public class WebMappingProfile : Profile
    {
        public WebMappingProfile()
        {
            // Academic Year mappings
            CreateMap<AcademicYearResponseDTO, AcademicYearVM>();
            CreateMap<CreateAcademicYearVM, CreateAcademicYearDTO>();
            CreateMap<UpdateAcademicYearVM, UpdateAcademicYearDTO>();

            // Course mappings
            CreateMap<CourseResponseDTO, CourseVM>();
            CreateMap<CreateCourseVM, CreateCourseDTO>();
            CreateMap<UpdateCourseVM, UpdateCourseDTO>();
        }
    }
}
