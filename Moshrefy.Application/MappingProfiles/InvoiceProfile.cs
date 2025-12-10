using AutoMapper;
using Moshrefy.Application.DTOs.Invoice;
using Moshrefy.Domain.Entities;

namespace Moshrefy.Application.MappingProfiles
{
    public class InvoiceProfile : Profile
    {
        public InvoiceProfile()
        {
            CreateMap<CreateInvoiceDTO, Invoice>();
            CreateMap<UpdateInvoiceDTO, Invoice>();
            CreateMap<Invoice, InvoiceResponseDTO>();
        }
    }
}