using AutoMapper;
using Moshrefy.Application.DTOs.Invoice;
using Moshrefy.Application.Interfaces.IUnitOfWork;
using Moshrefy.Application.Interfaces.IServices;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Exceptions;
using Moshrefy.Domain.Paramter;

namespace Moshrefy.Application.Services
{
    public class InvoiceService(
        IUnitOfWork unitOfWork, 
        IMapper mapper,
        ITenantContext tenantContext
    ) : BaseService(tenantContext), IInvoiceService
    {
        public async Task<InvoiceResponseDTO> CreateAsync(CreateInvoiceDTO createInvoiceDTO)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var invoice = mapper.Map<Invoice>(createInvoiceDTO);
            invoice.CenterId = currentCenterId;
            await unitOfWork.Invoices.AddAsync(invoice);
            await unitOfWork.SaveChangesAsync();
            return mapper.Map<InvoiceResponseDTO>(invoice);
        }

        public async Task<InvoiceResponseDTO?> GetByIdAsync(int id)
        {
            var invoice = await unitOfWork.Invoices.GetByIdAsync(id);
            if (invoice == null)
                throw new NotFoundException<int>(nameof(invoice), "invoice", id);

            ValidateCenterAccess(invoice.CenterId, nameof(Invoice));
            return mapper.Map<InvoiceResponseDTO>(invoice);
        }

        public async Task<List<InvoiceResponseDTO>> GetAllAsync(PaginationParameter paginationParamter)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var invoices = await unitOfWork.Invoices.GetAllAsync(
                i => i.CenterId == currentCenterId && !i.IsDeleted,
                paginationParamter);
            return mapper.Map<List<InvoiceResponseDTO>>(invoices.ToList());
        }

        public async Task<List<InvoiceResponseDTO>> GetPaidInvoicesAsync(PaginationParameter paginationParamter)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var invoices = await unitOfWork.Invoices.GetAllAsync(
                i => i.CenterId == currentCenterId && i.IsPaid && !i.IsDeleted,
                paginationParamter);
            return mapper.Map<List<InvoiceResponseDTO>>(invoices.ToList());
        }

        public async Task<List<InvoiceResponseDTO>> GetUnpaidInvoicesAsync(PaginationParameter paginationParamter)
        {
            var currentCenterId = GetCurrentCenterIdOrThrow();
            var invoices = await unitOfWork.Invoices.GetAllAsync(
                i => i.CenterId == currentCenterId && !i.IsPaid && !i.IsDeleted,
                paginationParamter);
            return mapper.Map<List<InvoiceResponseDTO>>(invoices.ToList());
        }

        public async Task UpdateAsync(int id, UpdateInvoiceDTO updateInvoiceDTO)
        {
            var invoice = await unitOfWork.Invoices.GetByIdAsync(id);
            if (invoice == null)
                throw new NotFoundException<int>(nameof(invoice), "invoice", id);

            ValidateCenterAccess(invoice.CenterId, nameof(Invoice));
            mapper.Map(updateInvoiceDTO, invoice);
            unitOfWork.Invoices.Update(invoice);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var invoice = await unitOfWork.Invoices.GetByIdAsync(id);
            if (invoice == null)
                throw new NotFoundException<int>(nameof(invoice), "invoice", id);

            ValidateCenterAccess(invoice.CenterId, nameof(Invoice));
            unitOfWork.Invoices.SoftDelete(invoice);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int id)
        {
            var invoice = await unitOfWork.Invoices.GetByIdAsync(id);
            if (invoice == null)
                throw new NotFoundException<int>(nameof(invoice), "invoice", id);

            ValidateCenterAccess(invoice.CenterId, nameof(Invoice));
            invoice.IsDeleted = true;
            unitOfWork.Invoices.Update(invoice);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task RestoreAsync(int id)
        {
            var invoice = await unitOfWork.Invoices.GetByIdAsync(id);
            if (invoice == null)
                throw new NotFoundException<int>(nameof(invoice), "invoice", id);

            ValidateCenterAccess(invoice.CenterId, nameof(Invoice));
            invoice.IsDeleted = false;
            unitOfWork.Invoices.Update(invoice);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task MarkAsPaidAsync(int id)
        {
            var invoice = await unitOfWork.Invoices.GetByIdAsync(id);
            if (invoice == null)
                throw new NotFoundException<int>(nameof(invoice), "invoice", id);

            ValidateCenterAccess(invoice.CenterId, nameof(Invoice));
            invoice.IsPaid = true;
            unitOfWork.Invoices.Update(invoice);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task MarkAsUnpaidAsync(int id)
        {
            var invoice = await unitOfWork.Invoices.GetByIdAsync(id);
            if (invoice == null)
                throw new NotFoundException<int>(nameof(invoice), "invoice", id);

            ValidateCenterAccess(invoice.CenterId, nameof(Invoice));
            invoice.IsPaid = false;
            unitOfWork.Invoices.Update(invoice);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
