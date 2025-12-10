using AutoMapper;
using Moshrefy.Application.DTOs.Enrollment;
using Moshrefy.Domain.Entities;

namespace Moshrefy.Application.MappingProfiles
{
    public class EnrollmentProfile : Profile
    {
        public EnrollmentProfile()
        {
            CreateMap<CreateEnrollmentDTO, Enrollment>();
            CreateMap<UpdateEnrollmentDTO, Enrollment>();
            CreateMap<Enrollment, EnrollmentResponseDTO>()
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student != null ? src.Student.Name : null))
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course != null ? src.Course.Name : null));
        }
    }
}