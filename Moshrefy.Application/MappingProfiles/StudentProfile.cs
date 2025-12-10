using AutoMapper;
using Moshrefy.Application.DTOs.Student;
using Moshrefy.Domain.Entities;

namespace Moshrefy.Application.MappingProfiles
{
    public class StudentProfile : Profile
    {
        public StudentProfile()
        {
            CreateMap<CreateStudentDTO, Student>();

            CreateMap<UpdateStudentDTO, Student>();

            CreateMap<Student, StudentResponseDTO>()
                .ForMember(dest => dest.EnrolledCoursesCount,
                    opt => opt.MapFrom(src => src.Enrollments != null ? src.Enrollments.Count : 0));
        }
    }
}