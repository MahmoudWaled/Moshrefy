using Microsoft.EntityFrameworkCore;
using Moshrefy.Application.Interfaces.IRepositories;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Paramter;
using Moshrefy.infrastructure.Data;
using Moshrefy.infrastructure.Repositories.GenericRepository;
using System.Linq.Expressions;

namespace Moshrefy.infrastructure.Repositories
{
    public class InvoiceRepository(AppDbContext appDbContext) : GenericRepository<Invoice, int>(appDbContext), IInvoiceRepository
    {
        // Predicate overload for proper server-side filtering
        public new async Task<IEnumerable<Invoice>> GetAllAsync(Expression<Func<Invoice, bool>> predicate, PaginationParamter paginationParamter)
        {
            var pageNumber = paginationParamter.PageNumber ?? 1;
            var pageSize = paginationParamter.PageSize ?? 25;

            return await appDbContext.Set<Invoice>()
                .Where(predicate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Invoice>> GetUnpaidInvoices()
        {
            return await appDbContext.Set<Invoice>()
                .Where(i => !i.IsPaid)
                .ToListAsync();
        }
    }
}
