using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using persistence.Data.AppDbContext;
using Services.Abstractions;

namespace Order_Management_System.Controllers.Invoice
{
    [Route("api/[controller]")]
    [ApiController]
   
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;
   

        public InvoiceController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
       
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> GetAll()
        {
            
            var invoices = await _invoiceService.GetAllAsync();
         
            return Ok(invoices); 
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> GetById(int id)
        {
            var invoice = await _invoiceService.GetByIdAsync(id);
            if (invoice == null)
                return NotFound(new { message = "Invoice not found" });

            return Ok(invoice); 
        }
    }
}
