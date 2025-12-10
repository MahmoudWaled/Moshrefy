using Microsoft.EntityFrameworkCore;
using Moshrefy.Application.Interfaces.IRepositories;
using Moshrefy.Domain.Entities;
using Moshrefy.infrastructure.Data;
using Moshrefy.infrastructure.Repositories.GenericRepository;

namespace Moshrefy.infrastructure.Repositories
{
    public class InvoiceRepository(AppDbContext appDbContext) : GenericRepository<Invoice, int>(appDbContext), IInvoiceRepository
    {
        public async Task<IEnumerable<Invoice>> GetUnpaidInvoices()
        {
            return await appDbContext.Set<Invoice>()
                .Where(i => !i.IsPaid)
                .ToListAsync();
        }
    }
}