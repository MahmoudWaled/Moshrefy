using Moshrefy.Application.DTOs.Invoice;
using Moshrefy.Domain.Paramter;

namespace Moshrefy.Application.Interfaces.IServices
{
    public interface IInvoiceService
    {
        Task<InvoiceResponseDTO> CreateAsync(CreateInvoiceDTO createInvoiceDTO);
        Task<InvoiceResponseDTO?> GetByIdAsync(int id);
        Task<List<InvoiceResponseDTO>> GetAllAsync(PaginationParameter paginationParamter);
        Task<List<InvoiceResponseDTO>> GetPaidInvoicesAsync(PaginationParameter paginationParamter);
        Task<List<InvoiceResponseDTO>> GetUnpaidInvoicesAsync(PaginationParameter paginationParamter);
        Task UpdateAsync(int id, UpdateInvoiceDTO updateInvoiceDTO);
        Task DeleteAsync(int id);
        Task SoftDeleteAsync(int id);
        Task RestoreAsync(int id);
        Task MarkAsPaidAsync(int id);
        Task MarkAsUnpaidAsync(int id);
    }
}