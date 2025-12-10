using AutoMapper;
using Moshrefy.Application.DTOs.Course;
using Moshrefy.Domain.Entities;

namespace Moshrefy.Application.MappingProfiles
{
    public class CourseProfile : Profile
    {
        public CourseProfile()
        {
            CreateMap<CreateCourseDTO, Course>();

            CreateMap<UpdateCourseDTO, Course>();

            CreateMap<Course, CourseResponseDTO>()
                .ForMember(dest => dest.AcademicYearName, opt => opt.MapFrom(src => src.AcademicYear != null ? src.AcademicYear.Name : null));
        }
    }
}