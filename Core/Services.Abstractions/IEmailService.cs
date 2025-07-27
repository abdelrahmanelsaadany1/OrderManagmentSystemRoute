using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IEmailService
    {
        Task SendOrderStatusChangedEmailAsync(string toEmail, int orderId, string newStatus);

    }
}
