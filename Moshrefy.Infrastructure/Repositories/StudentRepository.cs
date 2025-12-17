using Microsoft.EntityFrameworkCore;
using Moshrefy.Application.Interfaces.IRepositories;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Enums;
using Moshrefy.Domain.Paramter;
using Moshrefy.infrastructure.Data;
using Moshrefy.infrastructure.Repositories.GenericRepository;
using System.Linq.Expressions;

namespace Moshrefy.infrastructure.Repositories
{
    public class StudentRepository(AppDbContext appDbContext) : GenericRepository<Student, int>(appDbContext), IStudentRepository
    {
        public new async Task<Student?> GetByIdAsync(int id)
        {
            return await appDbContext.Set<Student>()
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                .Include(s => s.Attendances)
                .Include(s => s.ExamResults)
                    .ThenInclude(er => er.Exam)
                .Include(s => s.Payments)
                .Include(s => s.ReservedItems)
                .Where(s => s.Id == id)
                .FirstOrDefaultAsync();
        }

        public new async Task<IEnumerable<Student>> GetAllAsync(PaginationParamter paginationParamter)
        {
            var pageNumber = paginationParamter.PageNumber ?? 1;
            var pageSize = paginationParamter.PageSize ?? 40;

            if (pageSize > 40)
                pageSize = 40;

            return await appDbContext.Set<Student>()
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // Predicate overload with includes for proper server-side filtering
        public new async Task<IEnumerable<Student>> GetAllAsync(Expression<Func<Student, bool>> predicate, PaginationParamter paginationParamter)
        {
            var pageNumber = paginationParamter.PageNumber ?? 1;
            var pageSize = paginationParamter.PageSize ?? 40;

            if (pageSize > 40)
                pageSize = 40;

            return await appDbContext.Set<Student>()
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                .Where(predicate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Student>> GetByNameAsync(string studentName)
        {
            return await appDbContext.Set<Student>()
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                .Where(s => s.Name.Contains(studentName))
                .ToListAsync();
        }

        public async Task<Student?> GetByPhoneNumberAsync(string phoneNumber)
        {
            return await appDbContext.Set<Student>()
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                .Where(s => s.FirstPhone == phoneNumber ||
                           s.SecondPhone == phoneNumber ||
                           s.FatherPhone == phoneNumber ||
                           s.MotherPhone == phoneNumber)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Student>> GetByStatusAsync(StudentStatus status)
        {
            return await appDbContext.Set<Student>()
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                .Where(s => s.StudentStatus == status)
                .ToListAsync();
        }

        public async Task<IEnumerable<Student>> GetActiveStudentsAsync()
        {
            return await appDbContext.Set<Student>()
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                .Where(s => s.StudentStatus == StudentStatus.Active && !s.IsDeleted)
                .ToListAsync();
        }
    }
}
