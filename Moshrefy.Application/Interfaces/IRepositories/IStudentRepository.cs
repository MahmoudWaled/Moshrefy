using Moshrefy.Application.Interfaces.IGenericRepository;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Enums;

namespace Moshrefy.Application.Interfaces.IRepositories
{
    public interface IStudentRepository : IGenericRepository<Student, int>
    {
        Task<Student?> GetByPhoneNumberAsync(string phoneNumber);
        Task<IEnumerable<Student>> GetByNameAsync(string studentName);
        Task<IEnumerable<Student>> GetByStatusAsync(StudentStatus status);
        Task<IEnumerable<Student>> GetActiveStudentsAsync();
    }
}

