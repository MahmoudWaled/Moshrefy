using Moshrefy.Application.Interfaces.IGenericRepository;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Enums;

namespace Moshrefy.Application.Interfaces.IRepositories
{
    public interface IExamResultRepository : IGenericRepository<ExamResult, int>
    {

        public Task<IEnumerable<ExamResult>> GetExamResultsByStatusAsync(ExamResultStatus resultStatus);

    }
}