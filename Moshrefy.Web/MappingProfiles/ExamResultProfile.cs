using AutoMapper;
using Moshrefy.Application.DTOs.ExamResult;
using Moshrefy.Web.Models.ExamResult;

namespace Moshrefy.Web.MappingProfiles
{
    public class ExamResultProfile : Profile
    {
        public ExamResultProfile()
        {
            CreateMap<CreateExamResultVM, CreateExamResultDTO>().ReverseMap();
            CreateMap<UpdateExamResultVM, UpdateExamResultDTO>().ReverseMap();
            CreateMap<ExamResultVM, ExamResultResponseDTO>().ReverseMap();
        }
    }
}
