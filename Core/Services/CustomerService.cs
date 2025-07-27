using Domain.Entities;
using Domain.Interfaces;
using Services.Abstractions;
using Shared.Dtos;
using Shared.Dtos.CustomerDtos;

namespace Application.Services
{
    public class CustomerService : IcustomerService
    {
        private readonly IGenericRepository<Customer> _customerRepo;
        private readonly IGenericRepository<Order> _orderRepo;

        public CustomerService(
            IGenericRepository<Customer> customerRepo,
            IGenericRepository<Order> orderRepo)
        {
            _customerRepo = customerRepo;
            _orderRepo = orderRepo;
        }

        public async Task AddCustomerAsync(CustomerDto customerDto)
        {
            if (string.IsNullOrWhiteSpace(customerDto.Name))
                throw new ArgumentException("Customer name is required.");

            if (string.IsNullOrWhiteSpace(customerDto.Email))
                throw new ArgumentException("Customer email is required.");

            // Optional: Basic email format check
            if (!customerDto.Email.Contains("@"))
                throw new ArgumentException("Invalid email format.");

            var customer = new Customer
            {
                Name = customerDto.Name.Trim(),
                Email = customerDto.Email.Trim()
            };

            await _customerRepo.AddAsync(customer);
            await _customerRepo.SaveChangesAsync();
        }

        public async Task<IEnumerable<OrderDto>> GetCustomerOrdersAsync(int customerId)
        {
            var customer = await _customerRepo.GetByIdAsync(customerId);
            if (customer == null)
                throw new KeyNotFoundException($"Customer with ID {customerId} not found.");

            var orders = await _orderRepo.GetAllAsync();

            var filteredOrders = orders
                .Where(o => o.CustomerId == customerId)
                .Select(o => new OrderDto
                {
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    PaymentMethod = o.PaymentMethod,
                    Status = o.Status
                })
                .ToList();

            if (!filteredOrders.Any())
                throw new InvalidOperationException("No orders found for this customer.");

            return filteredOrders;
        }

    }
}
