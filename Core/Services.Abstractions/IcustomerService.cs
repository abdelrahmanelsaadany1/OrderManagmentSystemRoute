using Shared.Dtos.CustomerDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IcustomerService
    {
        Task AddCustomerAsync(CustomerDto customerDto);
        Task<IEnumerable<OrderDto>> GetCustomerOrdersAsync(int customerId);
    }
}
