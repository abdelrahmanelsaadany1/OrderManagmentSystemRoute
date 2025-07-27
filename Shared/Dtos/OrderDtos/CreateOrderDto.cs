using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos.OrderDtos
{
    public class CreateOrderDto
    {
        public int CustomerId { get; set; }
        [Required]
        [RegularExpression("^(CreditCard|PayPal)$", ErrorMessage = "PaymentMethod must be either 'CreditCard' or 'PayPal'.")]
        public string PaymentMethod { get; set; }
        public List<CreateOrderItemDto> Items { get; set; } = new List<CreateOrderItemDto>();
    }
}
