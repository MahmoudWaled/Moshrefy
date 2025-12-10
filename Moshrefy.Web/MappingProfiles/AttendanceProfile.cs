using AutoMapper;
using Moshrefy.Application.DTOs.Attendance;
using Moshrefy.Web.Models.Attendance;

namespace Moshrefy.Web.MappingProfiles
{
    public class AttendanceProfile : Profile
    {
        public AttendanceProfile()
        {
            CreateMap<CreateAttendanceVM, CreateAttendanceDTO>().ReverseMap();
            CreateMap<UpdateAttendanceVM, UpdateAttendanceDTO>().ReverseMap();
            CreateMap<AttendanceVM, AttendanceResponseDTO>().ReverseMap();
        }
    }
}
