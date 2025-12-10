using AutoMapper;
using Moshrefy.Application.DTOs.TeacherCourse;
using Moshrefy.Domain.Entities;

namespace Moshrefy.Application.MappingProfiles
{
    public class TeacherCourseProfile : Profile
    {
        public TeacherCourseProfile()
        {
            CreateMap<CreateTeacherCourseDTO, TeacherCourse>();
            CreateMap<UpdateTeacherCourseDTO, TeacherCourse>();
            CreateMap<TeacherCourse, TeacherCourseResponseDTO>()
                .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => src.Teacher != null ? src.Teacher.Name : null))
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course != null ? src.Course.Name : null));
        }
    }
}