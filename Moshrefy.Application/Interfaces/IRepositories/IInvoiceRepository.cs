using Moshrefy.Application.Interfaces.IGenericRepository;
using Moshrefy.Domain.Entities;

namespace Moshrefy.Application.Interfaces.IRepositories
{
    public interface IInvoiceRepository : IGenericRepository<Invoice, int>
    {

        public Task<IEnumerable<Invoice>> GetUnpaidInvoices();

    }
}