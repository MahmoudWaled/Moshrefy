using Microsoft.EntityFrameworkCore;
using Moshrefy.Application.Interfaces.IGenericRepository;
using Moshrefy.Domain.Interfaces;
using Moshrefy.Domain.Paramter;
using Moshrefy.Domain.SoftDeletable;
using Moshrefy.infrastructure.Data;
using System.Linq.Expressions;

namespace Moshrefy.infrastructure.Repositories.GenericRepository
{
    public class GenericRepository<TEntity, TKey>(AppDbContext appDbContext) : IGenericRepository<TEntity, TKey> where TEntity : class where TKey : IEquatable<TKey>
    {

        public async Task<IEnumerable<TEntity>> GetAllAsync(PaginationParameter paginationParamter)
        {
            return await appDbContext.Set<TEntity>()
                .Skip((paginationParamter.PageNumber - 1) * paginationParamter.PageSize)
                .Take(paginationParamter.PageSize)
                .ToListAsync();
        }

        // with predicate overload
        public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, PaginationParameter paginationParamter)
        {
            return await appDbContext.Set<TEntity>()
                .Where(predicate)
                .Skip((paginationParamter.PageNumber - 1) * paginationParamter.PageSize)
                .Take(paginationParamter.PageSize)
                .ToListAsync();
        }

        public async Task<TEntity?> GetByIdAsync(TKey id)
        {
            return await appDbContext.Set<TEntity>().FindAsync(id);
        }
        public async Task AddAsync(TEntity entity)
        {
            await appDbContext.Set<TEntity>().AddAsync(entity);
        }
        public void Update(TEntity entity)
        {
            appDbContext.Set<TEntity>().Update(entity);
        }

        public void SoftDelete(TEntity entity)
        {
            if (entity is ISoftDeletable softDeletable)
            {
                softDeletable.IsDeleted = true;
                
                // Also deactivate if entity has IsActive property
                if (entity is IActivatable activatable)
                {
                    activatable.IsActive = false;
                }
                
                appDbContext.Set<TEntity>().Update(entity);
            }
        }

        public void HardDelete(TEntity entity)
        {
            appDbContext.Set<TEntity>().Remove(entity);
        }

        public void Restore(TEntity entity)
        {
            if (entity is ISoftDeletable softDeletable)
            {
                softDeletable.IsDeleted = false;
                
                // Also reactivate if entity has IsActive property
                if (entity is IActivatable activatable)
                {
                    activatable.IsActive = true;
                }
                
                appDbContext.Set<TEntity>().Update(entity);
            }
        }
        public void Activate(TEntity entity)
        {
            if (entity is IActivatable activatable)
            {
                activatable.IsActive = true;
                appDbContext.Set<TEntity>().Update(entity);
            }
        }

        public void Deactivate(TEntity entity)
        {
            if (entity is IActivatable activatable)
            {
                activatable.IsActive = false;
                appDbContext.Set<TEntity>().Update(entity);
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await appDbContext.SaveChangesAsync();
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await appDbContext.Set<TEntity>().CountAsync(predicate);
        }

        public IQueryable<TEntity> GetQueryable()
        {
            return appDbContext.Set<TEntity>().AsQueryable();
        }
    }
}


