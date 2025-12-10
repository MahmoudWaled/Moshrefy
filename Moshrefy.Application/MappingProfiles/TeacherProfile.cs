using AutoMapper;
using Moshrefy.Application.DTOs.Teacher;
using Moshrefy.Domain.Entities;

namespace Moshrefy.Application.Mapping
{
    public class TeacherProfile : Profile
    {
        public TeacherProfile()
        {
            CreateMap<CreateTeacherDTO, Teacher>();

            CreateMap<UpdateTeacherDTO, Teacher>();

            CreateMap<Teacher, TeacherResponseDTO>();
        }
    }
}