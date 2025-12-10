using AutoMapper;
using Moshrefy.Application.DTOs.Payment;
using Moshrefy.Web.Models.Payment;

namespace Moshrefy.Web.MappingProfiles
{
    public class PaymentProfile : Profile
    {
        public PaymentProfile()
        {
            CreateMap<CreatePaymentVM, CreatePaymentDTO>().ReverseMap();
            CreateMap<UpdatePaymentVM, UpdatePaymentDTO>().ReverseMap();
            CreateMap<PaymentVM, PaymentResponseDTO>().ReverseMap();
        }
    }
}
