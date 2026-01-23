using Moshrefy.Domain.Paramter;
using System.Linq.Expressions;

namespace Moshrefy.Application.Interfaces.IGenericRepository
{
    public interface IGenericRepository<TEntity, TKey> where TEntity : class where TKey : IEquatable<TKey>
    {
        Task<IEnumerable<TEntity>> GetAllAsync(PaginationParameter paginationParamter);
        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, PaginationParameter paginationParamter);
        Task<TEntity?> GetByIdAsync(TKey id);
        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        void SoftDelete(TEntity entity);
        void HardDelete(TEntity entity);
        void Restore(TEntity entity);
        void Activate(TEntity entity);
        void Deactivate(TEntity entity);
        Task<int> SaveChangesAsync();
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
        IQueryable<TEntity> GetQueryable();
    }
}



