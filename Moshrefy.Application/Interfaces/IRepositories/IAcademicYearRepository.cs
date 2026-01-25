using Moshrefy.Application.Interfaces.IGenericRepository;
using Moshrefy.Domain.Entities;

namespace Moshrefy.Application.Interfaces.IRepositories
{
    public interface IAcademicYearRepository : IGenericRepository<AcademicYear, int>
    {

        public Task<IEnumerable<AcademicYear>> GetByName(string academicYearName);
        Task<(IEnumerable<AcademicYear> Items, int TotalCount)> GetPagedAsync(int centerId, int pageNumber, int pageSize);

    }
}

