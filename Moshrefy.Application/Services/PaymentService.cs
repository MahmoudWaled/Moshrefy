using AutoMapper;
using Moshrefy.Application.DTOs.Payment;
using Moshrefy.Application.Interfaces.IUnitOfWork;
using Moshrefy.Application.Interfaces.IServices;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Enums;
using Moshrefy.Domain.Exceptions;
using Moshrefy.Domain.Paramter;

namespace Moshrefy.Application.Services
{
    public class PaymentService(
        IUnitOfWork unitOfWork, 
        IMapper mapper,
        ITenantContext tenantContext
    ) : BaseService(tenantContext), IPaymentService
    {
        public async Task<PaymentResponseDTO> CreateAsync(CreatePaymentDTO createPaymentDTO)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var payment = mapper.Map<Payment>(createPaymentDTO);
            payment.CenterId = currentCenterId;
            await unitOfWork.Payments.AddAsync(payment);
            await unitOfWork.SaveChangesAsync();
            return mapper.Map<PaymentResponseDTO>(payment);
        }

        public async Task<PaymentResponseDTO?> GetByIdAsync(int id)
        {
            var payment = await unitOfWork.Payments.GetByIdAsync(id);
            if (payment == null)
                throw new NotFoundException<int>(nameof(payment), "payment", id);

            ValidateCenterAccess(payment.CenterId, nameof(Payment));
            return mapper.Map<PaymentResponseDTO>(payment);
        }

        public async Task<List<PaymentResponseDTO>> GetAllAsync(PaginationParamter paginationParamter)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var payments = await unitOfWork.Payments.GetAllAsync(
                p => p.CenterId == currentCenterId,
                paginationParamter);
            return mapper.Map<List<PaymentResponseDTO>>(payments.ToList());
        }

        public async Task<List<PaymentResponseDTO>> GetByStudentIdAsync(int studentId)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var payments = await unitOfWork.Payments.GetAllAsync(
                p => p.CenterId == currentCenterId && p.StudentId == studentId,
                new PaginationParamter { PageSize = 1000 });
            return mapper.Map<List<PaymentResponseDTO>>(payments.ToList());
        }

        public async Task<List<PaymentResponseDTO>> GetByInvoiceIdAsync(int invoiceId)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var payments = await unitOfWork.Payments.GetAllAsync(
                p => p.CenterId == currentCenterId && p.InvoiceId == invoiceId,
                new PaginationParamter { PageSize = 1000 });
            return mapper.Map<List<PaymentResponseDTO>>(payments.ToList());
        }

        public async Task<List<PaymentResponseDTO>> GetBySessionIdAsync(int sessionId)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var payments = await unitOfWork.Payments.GetAllAsync(
                p => p.CenterId == currentCenterId && p.SessionId == sessionId,
                new PaginationParamter { PageSize = 1000 });
            return mapper.Map<List<PaymentResponseDTO>>(payments.ToList());
        }

        public async Task<List<PaymentResponseDTO>> GetByExamIdAsync(int examId)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var payments = await unitOfWork.Payments.GetAllAsync(
                p => p.CenterId == currentCenterId && p.ExamId == examId,
                new PaginationParamter { PageSize = 1000 });
            return mapper.Map<List<PaymentResponseDTO>>(payments.ToList());
        }

        public async Task<List<PaymentResponseDTO>> GetByPaymentMethodAsync(PaymentMethods paymentMethod)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var payments = await unitOfWork.Payments.GetAllAsync(
                p => p.CenterId == currentCenterId && p.paymentMethods == paymentMethod,
                new PaginationParamter { PageSize = 1000 });
            return mapper.Map<List<PaymentResponseDTO>>(payments.ToList());
        }

        public async Task<List<PaymentResponseDTO>> GetByPaymentStatusAsync(PaymentStatus paymentStatus)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var payments = await unitOfWork.Payments.GetAllAsync(
                p => p.CenterId == currentCenterId && p.PaymentStatus == paymentStatus,
                new PaginationParamter { PageSize = 1000 });
            return mapper.Map<List<PaymentResponseDTO>>(payments.ToList());
        }

        public async Task UpdateAsync(int id, UpdatePaymentDTO updatePaymentDTO)
        {
            var payment = await unitOfWork.Payments.GetByIdAsync(id);
            if (payment == null)
                throw new NotFoundException<int>(nameof(payment), "payment", id);

            ValidateCenterAccess(payment.CenterId, nameof(Payment));
            mapper.Map(updatePaymentDTO, payment);
            unitOfWork.Payments.UpdateAsync(payment);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var payment = await unitOfWork.Payments.GetByIdAsync(id);
            if (payment == null)
                throw new NotFoundException<int>(nameof(payment), "payment", id);

            ValidateCenterAccess(payment.CenterId, nameof(Payment));
            unitOfWork.Payments.DeleteAsync(payment);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
