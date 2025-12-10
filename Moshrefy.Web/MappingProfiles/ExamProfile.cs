using AutoMapper;
using Moshrefy.Application.DTOs.Exam;
using Moshrefy.Web.Models.Exam;

namespace Moshrefy.Web.MappingProfiles
{
    public class ExamProfile : Profile
    {
        public ExamProfile()
        {
            CreateMap<CreateExamVM, CreateExamDTO>().ReverseMap();
            CreateMap<UpdateExamVM, UpdateExamDTO>().ReverseMap();
            CreateMap<ExamVM, ExamResponseDTO>().ReverseMap();
        }
    }
}
