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
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course != null && !src.Course.IsDeleted ? src.Course.Name : null))
                .ForMember(dest => dest.CourseIsDeleted, opt => opt.MapFrom(src => src.Course == null || src.Course.IsDeleted))
                .ForMember(dest => dest.AcademicYearName, opt => opt.MapFrom(src => src.Course != null && src.Course.AcademicYear != null ? src.Course.AcademicYear.Name : null));
        }
    }
}