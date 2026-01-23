using Moshrefy.Application.DTOs.Payment;
using Moshrefy.Domain.Enums;
using Moshrefy.Domain.Paramter;

namespace Moshrefy.Application.Interfaces.IServices
{
    public interface IPaymentService
    {
        Task<PaymentResponseDTO> CreateAsync(CreatePaymentDTO createPaymentDTO);
        Task<PaymentResponseDTO?> GetByIdAsync(int id);
        Task<List<PaymentResponseDTO>> GetAllAsync(PaginationParameter paginationParamter);
        Task<List<PaymentResponseDTO>> GetByStudentIdAsync(int studentId);
        Task<List<PaymentResponseDTO>> GetByInvoiceIdAsync(int invoiceId);
        Task<List<PaymentResponseDTO>> GetBySessionIdAsync(int sessionId);
        Task<List<PaymentResponseDTO>> GetByExamIdAsync(int examId);
        Task<List<PaymentResponseDTO>> GetByPaymentMethodAsync(PaymentMethods paymentMethod);
        Task<List<PaymentResponseDTO>> GetByPaymentStatusAsync(PaymentStatus paymentStatus);
        Task UpdateAsync(int id, UpdatePaymentDTO updatePaymentDTO);
        Task DeleteAsync(int id);
    }
}