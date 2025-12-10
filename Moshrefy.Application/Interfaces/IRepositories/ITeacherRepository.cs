using Moshrefy.Application.Interfaces.IGenericRepository;
using Moshrefy.Domain.Entities;

namespace Moshrefy.Application.Interfaces.IRepositories
{
    public interface ITeacherRepository : IGenericRepository<Teacher, int>
    {

        public Task<IEnumerable<Teacher>> GetByNameAsync(string teacherName);
        public Task<Teacher> GetByPhoneNumberAsync(string phoneNumber);
        public Task<IEnumerable<Teacher>> GetActiveTeachersAsync();
    }
}