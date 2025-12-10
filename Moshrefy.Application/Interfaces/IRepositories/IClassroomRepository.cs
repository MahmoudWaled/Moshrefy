using Moshrefy.Application.Interfaces.IGenericRepository;
using Moshrefy.Domain.Entities;

namespace Moshrefy.Application.Interfaces.IRepositories
{
    public interface IClassroomRepository : IGenericRepository<Classroom, int>
    {

        public Task<IEnumerable<Domain.Entities.Classroom>> GetByName(string classroomName);
    }
}