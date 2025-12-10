using AutoMapper;
using Moshrefy.Application.DTOs.Classroom;
using Moshrefy.Web.Models.Classroom;

namespace Moshrefy.Web.MappingProfiles
{
    public class ClassroomProfile : Profile
    {
        public ClassroomProfile()
        {
            CreateMap<CreateClassroomVM, CreateClassroomDTO>().ReverseMap();
            CreateMap<UpdateClassroomVM, UpdateClassroomDTO>().ReverseMap();
            CreateMap<ClassroomVM, ClassroomResponseDTO>().ReverseMap();
        }
    }
}
