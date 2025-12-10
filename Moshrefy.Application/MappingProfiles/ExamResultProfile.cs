using AutoMapper;
using Moshrefy.Application.DTOs.ExamResult;
using Moshrefy.Domain.Entities;

namespace Moshrefy.Application.MappingProfiles
{
    public class ExamResultProfile : Profile
    {
        public ExamResultProfile()
        {
            CreateMap<CreateExamResultDTO, ExamResult>();
            CreateMap<UpdateExamResultDTO, ExamResult>();
            CreateMap<ExamResult, ExamResultResponseDTO>()
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student != null ? src.Student.Name : null))
                .ForMember(dest => dest.ExamName, opt => opt.MapFrom(src => src.Exam != null ? src.Exam.Name : null));
        }
    }
}