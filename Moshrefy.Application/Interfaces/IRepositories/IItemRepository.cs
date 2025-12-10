using Moshrefy.Application.Interfaces.IGenericRepository;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Enums;

namespace Moshrefy.Application.Interfaces.IRepositories
{
    public interface IItemRepository : IGenericRepository<Item, int>
    {
        public Task<IEnumerable<Item>> GetByName(string itemName);
        public Task<IEnumerable<Item>> GetItemsByStaus(ItemStatus status);

    }
}