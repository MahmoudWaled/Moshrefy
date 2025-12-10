using Microsoft.EntityFrameworkCore;
using Moshrefy.Application.Interfaces.IRepositories;
using Moshrefy.Domain.Entities;
using Moshrefy.infrastructure.Data;
using Moshrefy.infrastructure.Repositories.GenericRepository;

namespace Moshrefy.infrastructure.Repositories
{
    public class EnrollmentRepository(AppDbContext appDbContext) : GenericRepository<Enrollment, int>(appDbContext), IEnrollmentRepository
    {
        public async Task<IEnumerable<Enrollment>> GetActiveEnrollments()
        {
            return await appDbContext.Set<Enrollment>()
                .Where(e => e.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Enrollment>> GetDeActiveEnrollments()
        {
            return await appDbContext.Set<Enrollment>()
                .Where(e => !e.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Enrollment>> GetEnrollmentsByStudentId(int studentId)
        {
            return await appDbContext.Set<Enrollment>()
                .Where(e => e.StudentId == studentId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Enrollment>> GetEnrollmentsByStudentName(string studentName)
        {
            return await appDbContext.Set<Enrollment>()
                .Include(e => e.Student)
                .Where(e => e.Student.Name.Contains(studentName))
                .ToListAsync();
        }
    }
}