using Microsoft.EntityFrameworkCore;
using Moshrefy.Application.Interfaces.IRepositories;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Enums;
using Moshrefy.infrastructure.Data;
using Moshrefy.infrastructure.Repositories.GenericRepository;

namespace Moshrefy.infrastructure.Repositories
{
    public class ItemRepository(AppDbContext appDbContext) : GenericRepository<Item, int>(appDbContext), IItemRepository
    {
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