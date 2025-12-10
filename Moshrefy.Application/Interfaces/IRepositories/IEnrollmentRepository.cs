using Moshrefy.Application.Interfaces.IGenericRepository;
using Moshrefy.Domain.Entities;

namespace Moshrefy.Application.Interfaces.IRepositories
{
    public interface IEnrollmentRepository : IGenericRepository<Enrollment, int>
    {

        public Task<IEnumerable<Enrollment>> GetActiveEnrollments();
        public Task<IEnumerable<Enrollment>> GetDeActiveEnrollments();
        public Task<IEnumerable<Enrollment>> GetEnrollmentsByStudentId(int studentId);
        public Task<IEnumerable<Enrollment>> GetEnrollmentsByStudentName(string studentName);

    }
}