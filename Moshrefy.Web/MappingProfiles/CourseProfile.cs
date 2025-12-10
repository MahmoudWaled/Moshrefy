using AutoMapper;
using Moshrefy.Application.DTOs.Course;
using Moshrefy.Web.Models.Course;

namespace Moshrefy.Web.MappingProfiles
{
    public class CourseProfile : Profile
    {
        public CourseProfile()
        {
            CreateMap<CreateCourseVM, CreateCourseDTO>().ReverseMap();
            CreateMap<UpdateCourseVM, UpdateCourseDTO>().ReverseMap();
            CreateMap<CourseVM, CourseResponseDTO>().ReverseMap();
        }
    }
}
