using Domain.Entities;
using Domain.Interfaces;
using Services.Abstractions;
using Shared.Dtos.InvoiceDto;


namespace Application.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IGenericRepository<Invoice> _invoiceRepo;

        public InvoiceService(IGenericRepository<Invoice> invoiceRepo)
        {
            _invoiceRepo = invoiceRepo;
        }

        public async Task<IEnumerable<InvoiceDto>> GetAllAsync()
        {
            var invoices = await _invoiceRepo.GetAllAsync();

            return invoices.Select(i => new InvoiceDto
            {
                InvoiceId = i.InvoiceId,
                OrderId = i.OrderId,
                InvoiceDate = i.InvoiceDate,
                TotalAmount = i.TotalAmount
            });
        }

        public async Task<InvoiceDto> GetByIdAsync(int id)
        {
            var invoice = await _invoiceRepo.GetByIdAsync(id);
            if (invoice == null) return null;

            return new InvoiceDto
            {
                InvoiceId = invoice.InvoiceId,
                OrderId = invoice.OrderId,
                InvoiceDate = invoice.InvoiceDate,
                TotalAmount = invoice.TotalAmount
            };
        }
    }
}
