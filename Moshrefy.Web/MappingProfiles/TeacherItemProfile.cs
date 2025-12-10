using AutoMapper;
using Moshrefy.Application.DTOs.TeacherItem;
using Moshrefy.Web.Models.TeacherItem;

namespace Moshrefy.Web.MappingProfiles
{
    public class TeacherItemProfile : Profile
    {
        public TeacherItemProfile()
        {
            CreateMap<CreateTeacherItemVM, CreateTeacherItemDTO>().ReverseMap();
            CreateMap<UpdateTeacherItemVM, UpdateTeacherItemDTO>().ReverseMap();
            CreateMap<TeacherItemVM, TeacherItemResponseDTO>().ReverseMap();
        }
    }
}
