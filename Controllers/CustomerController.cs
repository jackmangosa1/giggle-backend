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

        [HttpPost("bookings")]
        public async Task<IActionResult> CreateBooking([FromBody] BookingDto bookingDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var bookingSuccess = await _userService.CreateBookingAsync(bookingDto);
            return CreatedAtAction(nameof(CreateBooking), new { message = "Booking created successfully." });
        }

        [HttpGet("search-providers")]
        public async Task<IActionResult> SearchProviders([FromQuery] string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return BadRequest(new { message = "Search term cannot be empty" });
            }

            var providers = await _userService.SearchProvidersAsync(searchTerm);

            if (providers == null || !providers.Any())
            {
                return NotFound(new { message = "No providers found matching the search term" });
            }

            return Ok(providers);
        }

        [HttpGet("notifications/{userId}")]
        public async Task<IActionResult> GetNotificationsByUserId(string userId)
        {
            var notifications = await _userService.GetNotificationsByUserIdAsync(userId);

            if (notifications == null || !notifications.Any())
            {
                return NotFound(new { message = "No notifications found" });
            }

            return Ok(notifications);
        }
    }
}
