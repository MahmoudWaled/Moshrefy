using AutoMapper;
using Moshrefy.Application.DTOs.Invoice;
using Moshrefy.Application.Interfaces.IUnitOfWork;
using Moshrefy.Application.Interfaces.IServices;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Exceptions;
using Moshrefy.Domain.Paramter;

namespace Moshrefy.Application.Services
{
    public class InvoiceService(IUnitOfWork unitOfWork, IMapper mapper) : IInvoiceService
    {
        public async Task<InvoiceResponseDTO> CreateAsync(CreateInvoiceDTO createInvoiceDTO)
        {
            var invoice = mapper.Map<Invoice>(createInvoiceDTO);
            await unitOfWork.Invoices.AddAsync(invoice);
            await unitOfWork.SaveChangesAsync();
            return mapper.Map<InvoiceResponseDTO>(invoice);
        }

        public async Task<InvoiceResponseDTO?> GetByIdAsync(int id)
        {
            var invoice = await unitOfWork.Invoices.GetByIdAsync(id);
            if (invoice == null)
                throw new NotFoundException<int>(nameof(invoice), "invoice", id);

            return mapper.Map<InvoiceResponseDTO>(invoice);
        }

        public async Task<List<InvoiceResponseDTO>> GetAllAsync(PaginationParamter paginationParamter)
        {
            var invoices = await unitOfWork.Invoices.GetAllAsync(paginationParamter);
            return mapper.Map<List<InvoiceResponseDTO>>(invoices);
        }

        public async Task<List<InvoiceResponseDTO>> GetPaidInvoicesAsync(PaginationParamter paginationParamter)
        {
            var invoices = await unitOfWork.Invoices.GetAllAsync(paginationParamter);
            var paidInvoices = invoices.Where(i => i.IsPaid).ToList();
            return mapper.Map<List<InvoiceResponseDTO>>(paidInvoices);
        }

        public async Task<List<InvoiceResponseDTO>> GetUnpaidInvoicesAsync(PaginationParamter paginationParamter)
        {
            var invoices = await unitOfWork.Invoices.GetAllAsync(paginationParamter);
            var unpaidInvoices = invoices.Where(i => !i.IsPaid).ToList();
            return mapper.Map<List<InvoiceResponseDTO>>(unpaidInvoices);
        }

        public async Task UpdateAsync(int id, UpdateInvoiceDTO updateInvoiceDTO)
        {
            var invoice = await unitOfWork.Invoices.GetByIdAsync(id);
            if (invoice == null)
                throw new NotFoundException<int>(nameof(invoice), "invoice", id);

            mapper.Map(updateInvoiceDTO, invoice);
            unitOfWork.Invoices.UpdateAsync(invoice);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var invoice = await unitOfWork.Invoices.GetByIdAsync(id);
            if (invoice == null)
                throw new NotFoundException<int>(nameof(invoice), "invoice", id);

            unitOfWork.Invoices.DeleteAsync(invoice);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int id)
        {
            var invoice = await unitOfWork.Invoices.GetByIdAsync(id);
            if (invoice == null)
                throw new NotFoundException<int>(nameof(invoice), "invoice", id);

            invoice.IsDeleted = true;
            unitOfWork.Invoices.UpdateAsync(invoice);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task RestoreAsync(int id)
        {
            var invoice = await unitOfWork.Invoices.GetByIdAsync(id);
            if (invoice == null)
                throw new NotFoundException<int>(nameof(invoice), "invoice", id);

            invoice.IsDeleted = false;
            unitOfWork.Invoices.UpdateAsync(invoice);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task MarkAsPaidAsync(int id)
        {
            var invoice = await unitOfWork.Invoices.GetByIdAsync(id);
            if (invoice == null)
                throw new NotFoundException<int>(nameof(invoice), "invoice", id);

            invoice.IsPaid = true;
            unitOfWork.Invoices.UpdateAsync(invoice);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task MarkAsUnpaidAsync(int id)
        {
            var invoice = await unitOfWork.Invoices.GetByIdAsync(id);
            if (invoice == null)
                throw new NotFoundException<int>(nameof(invoice), "invoice", id);

            invoice.IsPaid = false;
            unitOfWork.Invoices.UpdateAsync(invoice);
            await unitOfWork.SaveChangesAsync();
        }
    }
}