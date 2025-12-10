using AutoMapper;
using Moshrefy.Application.DTOs.Classroom;
using Moshrefy.Domain.Entities;

namespace Moshrefy.Application.MappingProfiles
{
    public class ClassroomProfile : Profile
    {
        public ClassroomProfile()
        {
            CreateMap<CreateClassroomDTO, Classroom>();
            CreateMap<UpdateClassroomDTO, Classroom>();
            CreateMap<Classroom, ClassroomResponseDTO>();
        }
    }
}