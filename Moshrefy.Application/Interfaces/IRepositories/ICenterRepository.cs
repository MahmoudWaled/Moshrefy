using Moshrefy.Application.Interfaces.IGenericRepository;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Paramter;

namespace Moshrefy.Application.Interfaces.IRepositories
{
    public interface ICenterRepository : IGenericRepository<Center, int>
    {
        Task<IEnumerable<Center>> GetActiveCentersAsync(PaginationParamter paginationParamter);
        Task<IEnumerable<Center>> GetInactiveCentersAsync(PaginationParamter paginationParamter);
        Task<IEnumerable<Center>> GetDeletedCentersAsync(PaginationParamter paginationParamter);
        public Task<IEnumerable<Center>> GetByName(string centerName);
    }
}