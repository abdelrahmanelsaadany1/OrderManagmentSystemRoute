using Shared.Dtos.InvoiceDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IInvoiceService
    {
        Task<InvoiceDto> GetByIdAsync(int id);
        Task<IEnumerable<InvoiceDto>> GetAllAsync();
    }
}
