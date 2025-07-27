using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Services.Abstractions;
using Shared.Dtos;
using Shared.Dtos.OrderDtos;

namespace Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IEmailService _emailService;
        private readonly IOrderRepository _orderRepo;
        private readonly IGenericRepository<Product> _productRepo;
        private readonly IGenericRepository<Invoice> _invoiceRepo;
        private readonly IGenericRepository<Customer> _customerRepo;

        public OrderService(
            IOrderRepository orderRepo, 
            IGenericRepository<Product> productRepo,
            IGenericRepository<Invoice> invoiceRepo,
            IGenericRepository<Customer> customerRepo,
            IEmailService emailService

            )
        {
            _orderRepo = orderRepo;
            _productRepo = productRepo;
            _invoiceRepo = invoiceRepo;
            _emailService = emailService;
            _customerRepo = customerRepo;
        }

        public async Task<OrderDto> CreateAsync(CreateOrderDto dto)
        {
         
            if (!Enum.TryParse<PaymentMethod>(dto.PaymentMethod, true, out var paymentMethod) ||
                !Enum.IsDefined(typeof(PaymentMethod), paymentMethod))
            {
                throw new Exception("Invalid payment method. Allowed values: 'CreditCard' or 'PayPal'.");
            }

            decimal total = 0;
            var orderItems = new List<OrderItem>();

            foreach (var itemDto in dto.Items)
            {
                var product = await _productRepo.GetByIdAsync(itemDto.ProductId);
                if (product == null || product.Stock < itemDto.Quantity)
                    throw new Exception($"Product ID {itemDto.ProductId} not available or insufficient stock");

                decimal unitPrice = product.Price;
                decimal lineTotal = unitPrice * itemDto.Quantity;

                total += lineTotal;

                product.Stock -= itemDto.Quantity;
                _productRepo.Update(product);

                orderItems.Add(new OrderItem
                {
                    ProductId = itemDto.ProductId,
                    Quantity = itemDto.Quantity,
                    UnitPrice = unitPrice,
                    Discount = 0
                });
            }

            decimal discountRate = total > 200 ? 0.10m : total > 100 ? 0.05m : 0;
            decimal discountAmount = total * discountRate;
            decimal finalTotal = total - discountAmount;

            foreach (var item in orderItems)
            {
                var itemTotal = item.UnitPrice * item.Quantity;
                item.Discount = (itemTotal / total) * discountAmount;
            }

            var order = new Order
            {
                CustomerId = dto.CustomerId,
                OrderDate = DateTime.UtcNow,
                TotalAmount = finalTotal,
                PaymentMethod = paymentMethod, 
                Status = OrderStatus.Pending,
                OrderItems = orderItems
            };

            await _orderRepo.AddAsync(order);
            await _productRepo.SaveChangesAsync();
            await _orderRepo.SaveChangesAsync();

            var invoice = new Invoice
            {
                OrderId = order.OrderId,
                InvoiceDate = DateTime.UtcNow,
                TotalAmount = finalTotal
            };

            await _invoiceRepo.AddAsync(invoice);
            await _invoiceRepo.SaveChangesAsync();

            return MapToDto(order);
        }


        public async Task<OrderDto> GetByIdAsync(int id)
        {
            var order = await _orderRepo.GetByIdWithItemsAsync(id); 
            return order == null ? null : MapToDto(order);
        }

        public async Task<IEnumerable<OrderDto>> GetAllAsync()
        {
            var orders = await _orderRepo.GetAllWithItemsAsync();
            return orders.Select(o => MapToDto(o));
        }

        public async Task<bool> UpdateStatusAsync(int id, UpdateOrderStatusDto dto)
        {
            var order = await _orderRepo.GetByIdAsync(id);
            if (order == null) return false;

            order.Status = dto.Status;
            _orderRepo.Update(order);
            await _orderRepo.SaveChangesAsync();

            // Get customer email
            var customer = await _customerRepo.GetByIdAsync(order.CustomerId);
            if (customer != null && !string.IsNullOrWhiteSpace(customer.Email))
            {
                await _emailService.SendOrderStatusChangedEmailAsync(customer.Email, order.OrderId, order.Status.ToString());
            }

            return true;
        }


        private OrderDto MapToDto(Order order)
        {
            return new OrderDto
            {
                OrderId = order.OrderId,
                CustomerId = order.CustomerId,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                PaymentMethod = order.PaymentMethod,
                Status = order.Status,
                Items = order.OrderItems?.Select(i => new OrderItemDto
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    Discount = i.Discount
                }).ToList() ?? new List<OrderItemDto>()
            };
        }
    }
}
