using AutoMapper;
using Moshrefy.Application.DTOs.Session;
using Moshrefy.Domain.Entities;

namespace Moshrefy.Application.MappingProfiles
{
    public class SessionProfile : Profile
    {
        public SessionProfile()
        {
            CreateMap<CreateSessionDTO, Session>();
            CreateMap<UpdateSessionDTO, Session>();
            CreateMap<Session, SessionResponseDTO>()
                .ForMember(dest => dest.AcademicYearName, opt => opt.MapFrom(src => src.AcademicYear != null ? src.AcademicYear.Name : null))
                .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => src.TeacherCourse != null && src.TeacherCourse.Teacher != null ? src.TeacherCourse.Teacher.Name : null))
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.TeacherCourse != null && src.TeacherCourse.Course != null ? src.TeacherCourse.Course.Name : null))
                .ForMember(dest => dest.ClassroomName, opt => opt.MapFrom(src => src.Classroom != null ? src.Classroom.Name : null));
        }
    }
}