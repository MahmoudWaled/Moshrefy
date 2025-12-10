using Moshrefy.Application.Interfaces.IGenericRepository;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Enums;

namespace Moshrefy.Application.Interfaces.IRepositories
{
    public interface IAttendanceRepository : IGenericRepository<Attendance, int>
    {

        public Task<IEnumerable<Attendance>> GetByStatusAsync(AttendanceStatus status);



    }
}