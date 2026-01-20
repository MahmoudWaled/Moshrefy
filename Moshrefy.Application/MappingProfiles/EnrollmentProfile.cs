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
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student!.Name))
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course!.Name))
                .ForMember(dest => dest.AcademicYearName, opt => opt.MapFrom(src => src.Course!.AcademicYear != null ? src.Course.AcademicYear.Name : null))
                .ForMember(dest => dest.CourseIsDeleted, opt => opt.MapFrom(src => src.Course != null && src.Course.IsDeleted))
                .ForMember(dest => dest.StudentIsDeleted, opt => opt.MapFrom(src => src.Student != null && src.Student.IsDeleted));
        }
    }
}