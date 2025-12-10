using Moshrefy.Domain.Paramter;

namespace Moshrefy.Application.Interfaces.IGenericRepository
{
    public interface IGenericRepository<TEntity, TKey> where TEntity : class where TKey : IEquatable<TKey>
    {

        Task<IEnumerable<TEntity>> GetAllAsync(PaginationParamter paginationParamter);
        Task<TEntity?> GetByIdAsync(TKey id);
        Task AddAsync(TEntity entity);
        void UpdateAsync(TEntity entity);
        void DeleteAsync(TEntity entity);
        void HardDeleteAsync(TEntity entity);
        Task<int> SaveChangesAsync();
    }
}
