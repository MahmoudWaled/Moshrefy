using Moshrefy.Application.Interfaces.IGenericRepository;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Enums;

namespace Moshrefy.Application.Interfaces.IRepositories
{
    public interface IPaymentRepository : IGenericRepository<Payment, int>
    {
        public Task<IEnumerable<Domain.Entities.Payment>> GetByAmount(decimal amount);
        public Task<IEnumerable<Domain.Entities.Payment>> GetByPaymentStatus(PaymentStatus paymentStatus);
        public Task<IEnumerable<Domain.Entities.Payment>> GetByPaymentMethod(PaymentMethods paymentMethod);
    }
}