using AutoMapper;
using Moshrefy.Application.DTOs.Student;
using Moshrefy.Web.Models.Student;

namespace Moshrefy.Web.MappingProfiles
{
    public class StudentProfile : Profile
    {
        public StudentProfile()
        {
            CreateMap<CreateStudentVM, CreateStudentDTO>().ReverseMap();
            CreateMap<UpdateStudentVM, UpdateStudentDTO>().ReverseMap();
            CreateMap<StudentVM, StudentResponseDTO>().ReverseMap();
        }
    }
}
