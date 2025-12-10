using AutoMapper;
using Moshrefy.Application.DTOs.Payment;
using Moshrefy.Domain.Entities;

namespace Moshrefy.Application.MappingProfiles
{
    public class PaymentProfile : Profile
    {
        public PaymentProfile()
        {
            CreateMap<CreatePaymentDTO, Payment>();
            CreateMap<UpdatePaymentDTO, Payment>();
            CreateMap<Payment, PaymentResponseDTO>()
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student != null ? src.Student.Name : null));
        }
    }
}