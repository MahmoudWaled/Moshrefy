using Moshrefy.Application.Interfaces.IGenericRepository;
using Moshrefy.Domain.Entities;

namespace Moshrefy.Application.Interfaces.IRepositories
{
    public interface ICourseRepository : IGenericRepository<Course, int>
    {

        public Task<IEnumerable<Domain.Entities.Course>> GetByName(string courseName);

    }
}