using Moshrefy.Domain.Paramter;
using System.Linq.Expressions;

namespace Moshrefy.Application.Interfaces.IGenericRepository
{
    public interface IGenericRepository<TEntity, TKey> where TEntity : class where TKey : IEquatable<TKey>
    {
        // Get All Data For Super Admin
        Task<IEnumerable<TEntity>> GetAllAsync(PaginationParamter paginationParamter);
        // Get All Center Data For Center Admin
        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, PaginationParamter paginationParamter);
        Task<TEntity?> GetByIdAsync(TKey id);
        Task AddAsync(TEntity entity);
        void UpdateAsync(TEntity entity);
        void DeleteAsync(TEntity entity);
        void HardDeleteAsync(TEntity entity);
        Task<int> SaveChangesAsync();
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
    }
}


