using Microsoft.EntityFrameworkCore;
using Moshrefy.Application.Interfaces.IRepositories;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Enums;
using Moshrefy.infrastructure.Data;
using Moshrefy.infrastructure.Repositories.GenericRepository;

namespace Moshrefy.infrastructure.Repositories
{
    public class PaymentRepository(AppDbContext appDbContext) : GenericRepository<Payment, int>(appDbContext), IPaymentRepository
    {
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