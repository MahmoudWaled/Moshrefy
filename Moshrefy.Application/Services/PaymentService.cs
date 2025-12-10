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
    public class PaymentService(IUnitOfWork unitOfWork, IMapper mapper) : IPaymentService
    {
        public async Task<PaymentResponseDTO> CreateAsync(CreatePaymentDTO createPaymentDTO)
        {
            var payment = mapper.Map<Payment>(createPaymentDTO);
            await unitOfWork.Payments.AddAsync(payment);
            await unitOfWork.SaveChangesAsync();
            return mapper.Map<PaymentResponseDTO>(payment);
        }

        public async Task<PaymentResponseDTO?> GetByIdAsync(int id)
        {
            var payment = await unitOfWork.Payments.GetByIdAsync(id);
            if (payment == null)
                throw new NotFoundException<int>(nameof(payment), "payment", id);

            return mapper.Map<PaymentResponseDTO>(payment);
        }

        public async Task<List<PaymentResponseDTO>> GetAllAsync(PaginationParamter paginationParamter)
        {
            var payments = await unitOfWork.Payments.GetAllAsync(paginationParamter);
            return mapper.Map<List<PaymentResponseDTO>>(payments);
        }

        public async Task<List<PaymentResponseDTO>> GetByStudentIdAsync(int studentId)
        {
            var payments = await unitOfWork.Payments.GetAllAsync(new PaginationParamter());
            var filtered = payments.Where(p => p.StudentId == studentId).ToList();
            return mapper.Map<List<PaymentResponseDTO>>(filtered);
        }

        public async Task<List<PaymentResponseDTO>> GetByInvoiceIdAsync(int invoiceId)
        {
            var payments = await unitOfWork.Payments.GetAllAsync(new PaginationParamter());
            var filtered = payments.Where(p => p.InvoiceId == invoiceId).ToList();
            return mapper.Map<List<PaymentResponseDTO>>(filtered);
        }

        public async Task<List<PaymentResponseDTO>> GetBySessionIdAsync(int sessionId)
        {
            var payments = await unitOfWork.Payments.GetAllAsync(new PaginationParamter());
            var filtered = payments.Where(p => p.SessionId == sessionId).ToList();
            return mapper.Map<List<PaymentResponseDTO>>(filtered);
        }

        public async Task<List<PaymentResponseDTO>> GetByExamIdAsync(int examId)
        {
            var payments = await unitOfWork.Payments.GetAllAsync(new PaginationParamter());
            var filtered = payments.Where(p => p.ExamId == examId).ToList();
            return mapper.Map<List<PaymentResponseDTO>>(filtered);
        }

        public async Task<List<PaymentResponseDTO>> GetByPaymentMethodAsync(PaymentMethods paymentMethod)
        {
            var payments = await unitOfWork.Payments.GetAllAsync(new PaginationParamter());
            var filtered = payments.Where(p => p.paymentMethods == paymentMethod).ToList();
            return mapper.Map<List<PaymentResponseDTO>>(filtered);
        }

        public async Task<List<PaymentResponseDTO>> GetByPaymentStatusAsync(PaymentStatus paymentStatus)
        {
            var payments = await unitOfWork.Payments.GetAllAsync(new PaginationParamter());
            var filtered = payments.Where(p => p.PaymentStatus == paymentStatus).ToList();
            return mapper.Map<List<PaymentResponseDTO>>(filtered);
        }

        public async Task UpdateAsync(int id, UpdatePaymentDTO updatePaymentDTO)
        {
            var payment = await unitOfWork.Payments.GetByIdAsync(id);
            if (payment == null)
                throw new NotFoundException<int>(nameof(payment), "payment", id);

            mapper.Map(updatePaymentDTO, payment);
            unitOfWork.Payments.UpdateAsync(payment);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var payment = await unitOfWork.Payments.GetByIdAsync(id);
            if (payment == null)
                throw new NotFoundException<int>(nameof(payment), "payment", id);

            unitOfWork.Payments.DeleteAsync(payment);
            await unitOfWork.SaveChangesAsync();
        }
    }
}