using AutoMapper;
using Moshrefy.Application.DTOs.Teacher;
using Moshrefy.Web.Models.Teacher;

namespace Moshrefy.Web.MappingProfiles
{
    public class TeacherProfile : Profile
    {
        public TeacherProfile()
        {
            CreateMap<CreateTeacherVM, CreateTeacherDTO>().ReverseMap();
            CreateMap<UpdateTeacherVM, UpdateTeacherDTO>().ReverseMap();
            CreateMap<TeacherVM, TeacherResponseDTO>().ReverseMap();
            CreateMap<TeacherResponseDTO, UpdateTeacherVM>();
        }
    }
}
