using AutoMapper;
using Moshrefy.Application.DTOs.Item;
using Moshrefy.Domain.Entities;

namespace Moshrefy.Application.MappingProfiles
{
    public class ItemProfile : Profile
    {
        public ItemProfile()
        {
            CreateMap<CreateItemDTO, Item>();
            CreateMap<UpdateItemDTO, Item>();
            CreateMap<Item, ItemResponseDTO>()
                .ForMember(dest => dest.ReservedByStudentName, opt => opt.MapFrom(src => src.ReservedByStudent != null ? src.ReservedByStudent.Name : null));
        }
    }
}