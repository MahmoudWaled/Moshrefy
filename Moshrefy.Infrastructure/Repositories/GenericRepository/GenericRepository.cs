using Microsoft.EntityFrameworkCore;
using Moshrefy.Application.Interfaces.IGenericRepository;
using Moshrefy.Domain.Paramter;
using Moshrefy.Domain.SoftDeletable;
using Moshrefy.infrastructure.Data;
using System.Linq.Expressions;

namespace Moshrefy.infrastructure.Repositories.GenericRepository
{
    public class GenericRepository<TEntity, TKey>(AppDbContext appDbContext) : IGenericRepository<TEntity, TKey> where TEntity : class where TKey : IEquatable<TKey>
    {
        public async Task<IEnumerable<TEntity>> GetAllAsync(PaginationParamter paginationParamter)
        {
            var pageNumber = paginationParamter.PageNumber ?? 1;
            var pageSize = paginationParamter.PageSize ?? 25;

            return await appDbContext.Set<TEntity>()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, PaginationParamter paginationParamter)
        {
            var pageNumber = paginationParamter.PageNumber ?? 1;
            var pageSize = paginationParamter.PageSize ?? 25;

            // Apply filter BEFORE pagination at database level
            return await appDbContext.Set<TEntity>()
                .Where(predicate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
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
        public void UpdateAsync(TEntity entity)
        {
            appDbContext.Set<TEntity>().Update(entity);
        }

        public void DeleteAsync(TEntity entity)
        {
            if (entity is ISoftDeletable softDeletable)
            {
                softDeletable.IsDeleted = true;
                appDbContext.Set<TEntity>().Update(entity);
            }
            else
            {

                appDbContext.Set<TEntity>().Remove(entity);

            }
        }

        public void HardDeleteAsync(TEntity entity)
        {
            appDbContext.Set<TEntity>().Remove(entity);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await appDbContext.SaveChangesAsync();
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await appDbContext.Set<TEntity>().CountAsync(predicate);
        }
    }
}


