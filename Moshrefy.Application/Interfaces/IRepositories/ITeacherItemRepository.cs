using Moshrefy.Application.Interfaces.IGenericRepository;
using Moshrefy.Domain.Entities;

namespace Moshrefy.Application.Interfaces.IRepositories
{
    public interface ITeacherItemRepository : IGenericRepository<TeacherItem, int>
    {

        public Task<IEnumerable<TeacherItem>> GetByTeacherNameAsync(string teacherName);
        public Task<IEnumerable<TeacherItem>> GetByTeacherPhoneAsync(string teacherPhone);
        public Task<IEnumerable<TeacherItem>> GetByTeacherIdAsync(int teacherId);
    }
}