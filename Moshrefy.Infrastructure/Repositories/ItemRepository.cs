using Microsoft.EntityFrameworkCore;
using Moshrefy.Application.Interfaces.IRepositories;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Enums;
using Moshrefy.Domain.Paramter;
using Moshrefy.infrastructure.Data;
using Moshrefy.infrastructure.Repositories.GenericRepository;
using System.Linq.Expressions;

namespace Moshrefy.infrastructure.Repositories
{
    public class ItemRepository(AppDbContext appDbContext) : GenericRepository<Item, int>(appDbContext), IItemRepository
    {
        // Predicate overload for proper server-side filtering
        public new async Task<IEnumerable<Item>> GetAllAsync(Expression<Func<Item, bool>> predicate, PaginationParameter paginationParamter)
        {
            return await appDbContext.Set<Item>()
                .Where(predicate)
                .Skip((paginationParamter.PageNumber - 1) * paginationParamter.PageSize)
                .Take(paginationParamter.PageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Item>> GetByName(string itemName)
        {
            return await appDbContext.Set<Item>()
                .Where(i => i.Name.Contains(itemName))
                .ToListAsync();
        }

        public async Task<IEnumerable<Item>> GetItemsByStaus(ItemStatus status)
        {
            return await appDbContext.Set<Item>()
                .Where(i => i.ItemStatus.Equals(status))
                .ToListAsync();
        }

    }
}
