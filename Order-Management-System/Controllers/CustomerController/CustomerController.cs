
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared.Dtos;
using Shared.Dtos.CustomerDtos;
using System.Security.Claims;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class CustomersController : ControllerBase
    {
        private readonly IcustomerService _customerService;

        public CustomersController(IcustomerService customerService)
        {
            _customerService = customerService;
        }

     
        [HttpPost]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> AddCustomer([FromBody] CustomerDto dto)
        {
            try
            {
                await _customerService.AddCustomerAsync(dto);
                return StatusCode(201, new { message = "Customer created successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An unexpected error occurred." });
            }
        }


        [HttpGet("{customerId}/orders")]
        [Authorize(Roles = "Admin,Customer")]

        public async Task<IActionResult> GetCustomerOrders(int customerId)
        {
            try
            {
               
                int tokenCustomerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

             

                var orders = await _customerService.GetCustomerOrdersAsync(customerId);
                return Ok(orders);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

    }
}
