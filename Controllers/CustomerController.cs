using Microsoft.AspNetCore.Mvc;
using ServiceManagementAPI.Dtos;
using ServiceManagementAPI.Services.CustomerService;

namespace ServiceManagementAPI.Controllers
{
    [Route("api/customers")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _userService;

        public CustomerController(ICustomerService userService)
        {
            _userService = userService;
        }

        [HttpGet("profile/{customerId}")]
        public async Task<IActionResult> GetCustomerProfile(int customerId)
        {
            var customerProfile = await _userService.GetCustomerProfileAsync(customerId);

            if (customerProfile == null)
            {
                return NotFound(new { message = "Customer not found" });
            }

            return Ok(customerProfile);
        }

        [HttpPut("profile/{customerId}")]
        public async Task<IActionResult> UpdateCustomerProfile(int customerId, [FromForm] UpdateCustomerProfileDto updateCustomerProfileDto, IFormFile? imageFile = null)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Stream? imageStream = null;

            if (imageFile != null)
            {
                imageStream = imageFile.OpenReadStream();
            }

            var result = await _userService.UpdateCustomerProfileAsync(customerId, updateCustomerProfileDto, imageStream!);

            if (!result)
            {
                return NotFound(new { message = "Customer not found" });
            }

            return Ok(new { message = "Profile updated successfully" });
        }
    }
}
