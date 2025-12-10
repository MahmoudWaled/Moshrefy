using Microsoft.EntityFrameworkCore;
using Moshrefy.Application.Interfaces.IRepositories;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Enums;
using Moshrefy.infrastructure.Data;
using Moshrefy.infrastructure.Repositories.GenericRepository;

namespace Moshrefy.infrastructure.Repositories
{
    public class AttendanceRepository(AppDbContext appDbContext) : GenericRepository<Attendance, int>(appDbContext), IAttendanceRepository
    {
        public async Task<IEnumerable<Attendance>> GetByStatusAsync(AttendanceStatus status)
        {
            return await appDbContext.Set<Attendance>()
                    .Where(a => a.AttendanceStatus == status)
                    .ToListAsync();
        }
    }
}