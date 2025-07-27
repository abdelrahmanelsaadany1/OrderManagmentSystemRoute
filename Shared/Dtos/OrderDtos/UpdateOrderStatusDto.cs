using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos.OrderDtos
{
    public class UpdateOrderStatusDto
    {
        [Required]
        [EnumDataType(typeof(OrderStatus), ErrorMessage = "Please choose a valid status: Pending, Processing, Shipped, Delivered, Cancelled.")]
        public OrderStatus Status { get; set; }

    }
}
