using Moshrefy.Application.Interfaces.IGenericRepository;
using Moshrefy.Domain.Entities;

namespace Moshrefy.Application.Interfaces.IRepositories
{
    public interface IExamRepository : IGenericRepository<Exam, int>
    {

        public Task<IEnumerable<Domain.Entities.Exam>> GetByName(string examName);

        public Task<IEnumerable<Domain.Entities.Exam>> GetByDate(DateTime examDate);

    }
}