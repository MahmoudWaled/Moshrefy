using Microsoft.EntityFrameworkCore;
using Moshrefy.Application.Interfaces.IRepositories;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Paramter;
using Moshrefy.infrastructure.Data;
using Moshrefy.infrastructure.Repositories.GenericRepository;
using System.Linq.Expressions;

namespace Moshrefy.infrastructure.Repositories
{
    public class EnrollmentRepository(AppDbContext appDbContext) : GenericRepository<Enrollment, int>(appDbContext), IEnrollmentRepository
    {
        // Predicate overload for proper server-side filtering
        public new async Task<IEnumerable<Enrollment>> GetAllAsync(Expression<Func<Enrollment, bool>> predicate, PaginationParameter paginationParamter)
        {
            return await appDbContext.Set<Enrollment>()
                .Include(e => e.Student)
                .Include(e => e.Course)
                    .ThenInclude(c => c.AcademicYear)
                .Where(predicate)
                .Skip((paginationParamter.PageNumber - 1) * paginationParamter.PageSize)
                .Take(paginationParamter.PageSize)
                .ToListAsync();
        }

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
