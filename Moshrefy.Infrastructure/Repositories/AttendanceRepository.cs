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
    public class AttendanceRepository(AppDbContext appDbContext) : GenericRepository<Attendance, int>(appDbContext), IAttendanceRepository
    {
        // Predicate overload for proper server-side filtering
        public new async Task<IEnumerable<Attendance>> GetAllAsync(Expression<Func<Attendance, bool>> predicate, PaginationParamter paginationParamter)
        {
            var pageNumber = paginationParamter.PageNumber ?? 1;
            var pageSize = paginationParamter.PageSize ?? 25;

            return await appDbContext.Set<Attendance>()
                .Include(a => a.Session)
                .Include(a => a.Student)
                .Where(predicate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Attendance>> GetByStatusAsync(AttendanceStatus status)
        {
            return await appDbContext.Set<Attendance>()
                    .Where(a => a.AttendanceStatus == status)
                    .ToListAsync();
        }
    }
}
