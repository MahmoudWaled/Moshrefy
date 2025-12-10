using AutoMapper;
using Moshrefy.Application.DTOs.Exam;
using Moshrefy.Domain.Entities;

namespace Moshrefy.Application.MappingProfiles
{
    public class ExamProfile : Profile
    {
        public ExamProfile()
        {
            CreateMap<CreateExamDTO, Exam>();
            CreateMap<UpdateExamDTO, Exam>();
            CreateMap<Exam, ExamResponseDTO>()
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course != null ? src.Course.Name : null))
                .ForMember(dest => dest.ClassroomName, opt => opt.MapFrom(src => src.Classroom != null ? src.Classroom.Name : null))
                .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => src.TeacherCourse != null && src.TeacherCourse.Teacher != null ? src.TeacherCourse.Teacher.Name : null));
        }
    }
}