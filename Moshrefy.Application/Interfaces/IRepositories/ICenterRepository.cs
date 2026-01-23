using Moshrefy.Application.Interfaces.IGenericRepository;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Paramter;

namespace Moshrefy.Application.Interfaces.IRepositories
{
    public interface ICenterRepository : IGenericRepository<Center, int>
    {
        Task<(IEnumerable<Center> centers, int TotalCount)> GetNonDeletedPagedAsync(PaginationParameter paginationParamter);
        Task<(IEnumerable<Center> centers, int TotalCount)> GetActivePagedAsync(PaginationParameter paginationParamter);
        Task<(IEnumerable<Center> centers, int TotalCount)> GetInactivePagedAsync(PaginationParameter paginationParamter);
        Task<(IEnumerable<Center> centers, int TotalCount)> GetDeletedPagedAsync(PaginationParameter paginationParamter);
        Task<IEnumerable<Center>> GetByName(string centerName);
        Task<int> GetTotalCountAsync();
        Task<int> GetNonDeletedCountAsync();
        Task<int> GetDeletedCountAsync();
    }
}