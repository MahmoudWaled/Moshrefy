using AutoMapper;
using Moshrefy.Application.DTOs.Enrollment;
using Moshrefy.Web.Models.Enrollment;

namespace Moshrefy.Web.MappingProfiles
{
    public class EnrollmentProfile : Profile
    {
        public EnrollmentProfile()
        {
            CreateMap<CreateEnrollmentVM, CreateEnrollmentDTO>().ReverseMap();
            CreateMap<UpdateEnrollmentVM, UpdateEnrollmentDTO>().ReverseMap();
            CreateMap<EnrollmentVM, EnrollmentResponseDTO>().ReverseMap();
            CreateMap<EnrollmentResponseDTO, UpdateEnrollmentVM>();
        }
    }
}
