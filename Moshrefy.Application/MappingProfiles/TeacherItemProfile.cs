using AutoMapper;
using Moshrefy.Application.DTOs.TeacherItem;
using Moshrefy.Domain.Entities;

namespace Moshrefy.Application.MappingProfiles
{
    public class TeacherItemProfile : Profile
    {
        public TeacherItemProfile()
        {
            CreateMap<CreateTeacherItemDTO, TeacherItem>();
            CreateMap<UpdateTeacherItemDTO, TeacherItem>();
            CreateMap<TeacherItem, TeacherItemResponseDTO>()
                .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => src.Teacher != null ? src.Teacher.Name : null))
                .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.Item != null ? src.Item.Name : null));
        }
    }
}