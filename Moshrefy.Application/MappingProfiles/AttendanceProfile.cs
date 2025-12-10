using AutoMapper;
using Moshrefy.Application.DTOs.Attendance;
using Moshrefy.Domain.Entities;

namespace Moshrefy.Application.MappingProfiles
{
    public class AttendanceProfile : Profile
    {
        public AttendanceProfile()
        {
            CreateMap<CreateAttendanceDTO, Attendance>();
            CreateMap<UpdateAttendanceDTO, Attendance>();
            CreateMap<Attendance, AttendanceResponseDTO>()
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student != null ? src.Student.Name : null))
                .ForMember(dest => dest.ExamName, opt => opt.MapFrom(src => src.Exam != null ? src.Exam.Name : null));
        }
    }
}