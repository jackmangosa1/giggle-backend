using Microsoft.AspNetCore.Mvc;
using ServiceManagementAPI.Dtos;
using ServiceManagementAPI.Enums;
using ServiceManagementAPI.Services.ProviderService;

namespace ServiceManagementAPI.Controllers
{
    [Route("api/providers")]
    [ApiController]
    public class ProviderController : ControllerBase
    {
        private readonly IProviderService _providerService;

        public ProviderController(IProviderService providerService)
        {
            _providerService = providerService;
        }

        [HttpGet("profile/{providerId}")]
        public async Task<IActionResult> GetProviderProfile(string providerId)
        {
            var providerProfile = await _providerService.GetProviderProfileAsync(providerId);

            if (providerProfile == null)
            {
                return NotFound(new { message = "Provider not found" });
            }

            return Ok(providerProfile);
        }

        [HttpPut("profile/{providerId}")]
        public async Task<IActionResult> UpdateProviderProfile(int providerId, [FromForm] UpdateProviderProfileDto updateProviderProfileDto, IFormFile? imageFile = null)
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

            var result = await _providerService.UpdateProviderProfileAsync(providerId, updateProviderProfileDto, imageStream!);

            if (!result)
            {
                return NotFound(new { message = "Provider not found" });
            }

            return Ok(new { message = "Profile updated successfully" });
        }

        [HttpPost("{providerId}/services")]
        public async Task<IActionResult> AddService(string providerId, [FromForm] AddServiceDto addServiceDto, IFormFile? imageFile = null)
        {
            if (imageFile == null)
            {
                Console.WriteLine("No image file received");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Stream? imageStream = null;

            if (imageFile != null)
            {
                imageStream = imageFile.OpenReadStream();
            }

            var result = await _providerService.AddServiceAsync(providerId, addServiceDto, imageStream!);
            if (!result)
            {
                return NotFound("Provider or Service Category not found.");
            }

            return Ok("Service added successfully.");
        }

        [HttpPut("bookings/{bookingId}/status")]
        public async Task<IActionResult> UpdateBookingStatus(int bookingId, [FromBody] BookingStatus bookingStatus)
        {
            if (!Enum.IsDefined(typeof(BookingStatus), bookingStatus))
            {
                return BadRequest(new { message = "Invalid booking status" });
            }

            var result = await _providerService.UpdateBookingStatusAsync(bookingId, bookingStatus);

            if (!result)
            {
                return NotFound(new { message = "Booking not found or status update failed" });
            }

            return Ok(new { message = "Booking status updated successfully" });
        }

    }
}
