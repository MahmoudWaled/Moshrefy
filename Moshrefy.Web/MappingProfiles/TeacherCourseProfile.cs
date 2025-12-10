using AutoMapper;
using Moshrefy.Application.DTOs.TeacherCourse;
using Moshrefy.Web.Models.TeacherCourse;

namespace Moshrefy.Web.MappingProfiles
{
    public class TeacherCourseProfile : Profile
    {
        public TeacherCourseProfile()
        {
            CreateMap<CreateTeacherCourseVM, CreateTeacherCourseDTO>().ReverseMap();
            CreateMap<UpdateTeacherCourseVM, UpdateTeacherCourseDTO>().ReverseMap();
            CreateMap<TeacherCourseVM, TeacherCourseResponseDTO>().ReverseMap();
        }
    }
}
