using Moshrefy.Application.Interfaces.IGenericRepository;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Paramter;

namespace Moshrefy.Application.Interfaces.IRepositories
{
    public interface ICenterRepository : IGenericRepository<Center, int>
    {
        Task<(IEnumerable<Center> centers, int TotalCount)> GetNonDeletedPagedAsync(PaginationParamter paginationParamter);
        Task<(IEnumerable<Center> centers, int TotalCount)> GetActivePagedAsync(PaginationParamter paginationParamter);
        Task<(IEnumerable<Center> centers, int TotalCount)> GetInactivePagedAsync(PaginationParamter paginationParamter);
        Task<(IEnumerable<Center> centers, int TotalCount)> GetDeletedPagedAsync(PaginationParamter paginationParamter);
        Task<IEnumerable<Center>> GetByName(string centerName);
        Task<int> GetTotalCountAsync();
        Task<int> GetNonDeletedCountAsync();
        Task<int> GetDeletedCountAsync();
    }
}