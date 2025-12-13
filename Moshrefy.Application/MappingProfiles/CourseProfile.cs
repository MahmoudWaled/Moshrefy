using AutoMapper;
using Moshrefy.Application.DTOs.Course;
using Moshrefy.Domain.Entities;

namespace Moshrefy.Application.MappingProfiles
{
    public class CourseProfile : Profile
    {
        public CourseProfile()
        {
            // Entity to DTO
            CreateMap<Course, CourseResponseDTO>()
                .ForMember(dest => dest.AcademicYearName, opt => opt.MapFrom(src => src.AcademicYear != null ? src.AcademicYear.Name : null));

            CreateMap<CreateCourseDTO, Course>();
            CreateMap<UpdateCourseDTO, Course>();
        }
    }
}