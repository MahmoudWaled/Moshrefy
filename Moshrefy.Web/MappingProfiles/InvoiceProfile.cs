using AutoMapper;
using Moshrefy.Application.DTOs.Invoice;
using Moshrefy.Web.Models.Invoice;

namespace Moshrefy.Web.MappingProfiles
{
    public class InvoiceProfile : Profile
    {
        public InvoiceProfile()
        {
            CreateMap<CreateInvoiceVM, CreateInvoiceDTO>().ReverseMap();
            CreateMap<UpdateInvoiceVM, UpdateInvoiceDTO>().ReverseMap();
            CreateMap<InvoiceVM, InvoiceResponseDTO>().ReverseMap();
        }
    }
}
