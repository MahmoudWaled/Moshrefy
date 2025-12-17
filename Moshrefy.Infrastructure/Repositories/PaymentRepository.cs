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
    public class PaymentRepository(AppDbContext appDbContext) : GenericRepository<Payment, int>(appDbContext), IPaymentRepository
    {
        // Predicate overload for proper server-side filtering
        public new async Task<IEnumerable<Payment>> GetAllAsync(Expression<Func<Payment, bool>> predicate, PaginationParamter paginationParamter)
        {
            var pageNumber = paginationParamter.PageNumber ?? 1;
            var pageSize = paginationParamter.PageSize ?? 25;

            return await appDbContext.Set<Payment>()
                .Include(p => p.Student)
                .Where(predicate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetByAmount(decimal amount)
        {
            return await appDbContext.Set<Payment>()
                .Where(p => p.AmountPaid == amount)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetByPaymentMethod(PaymentMethods paymentMethod)
        {
            return await appDbContext.Set<Payment>()
                .Where(p => p.paymentMethods == paymentMethod)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetByPaymentStatus(PaymentStatus paymentStatus)
        {
            return await appDbContext.Set<Payment>()
                .Where(p => p.PaymentStatus == paymentStatus)
                .ToListAsync();
        }
    }
}
