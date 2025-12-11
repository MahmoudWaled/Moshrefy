using Moshrefy.Application.Interfaces.IGenericRepository;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Paramter;

namespace Moshrefy.Application.Interfaces.IRepositories
{
    public interface ICenterRepository : IGenericRepository<Center, int>
    {
        Task<IEnumerable<Center>> GetNonDeletedCentersAsync(PaginationParamter paginationParamter);
        Task<int> GetTotalCountAsync();
        Task<int> GetNonDeletedCountAsync();
        Task<int> GetDeletedCountAsync();
        Task<IEnumerable<Center>> GetActiveCentersAsync(PaginationParamter paginationParamter);
        Task<IEnumerable<Center>> GetInactiveCentersAsync(PaginationParamter paginationParamter);
        Task<IEnumerable<Center>> GetDeletedCentersAsync(PaginationParamter paginationParamter);
        Task<IEnumerable<Center>> GetByName(string centerName);
    }
}