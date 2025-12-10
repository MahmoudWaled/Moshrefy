using AutoMapper;
using Moshrefy.Application.DTOs.Item;
using Moshrefy.API.Controllers;
using Moshrefy.Web.Models.Item;

namespace Moshrefy.Web.MappingProfiles
{
    public class ItemProfile : Profile
    {
        public ItemProfile()
        {
            CreateMap<CreateItemVM, CreateItemDTO>().ReverseMap();
            CreateMap<UpdateItemVM, UpdateItemDTO>().ReverseMap();
            CreateMap<ItemVM, ItemResponseDTO>().ReverseMap();
            CreateMap<ReserveItemVM, ReserveItemDTO>().ReverseMap();
        }
    }
}
