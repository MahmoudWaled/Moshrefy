using AutoMapper;
using Moshrefy.Application.DTOs.Statistics;
using Moshrefy.Web.Models.Statistics;

namespace Moshrefy.Web.MappingProfiles
{
    public class StatisticsProfile : Profile
    {
        public StatisticsProfile()
        {
            CreateMap<SystemStatisticsDTO, SystemStatisticsVM>();
        }
    }
}