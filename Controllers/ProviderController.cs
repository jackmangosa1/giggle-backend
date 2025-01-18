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
        public async Task<IActionResult> UpdateProviderProfile(string providerId, [FromForm] UpdateProviderProfileDto updateProviderProfileDto, IFormFile? imageFile = null)
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

        [HttpPut("{providerId}/services/{serviceId}")]
        public async Task<IActionResult> UpdateService(string providerId, int serviceId, [FromForm] UpdateServiceDto updateServiceDto, IFormFile? imageFile = null)
        {
            Stream? imageStream = null;

            if (imageFile != null)
            {
                imageStream = imageFile.OpenReadStream();
            }

            var result = await _providerService.UpdateServiceAsync(providerId, serviceId, updateServiceDto, imageStream);

            if (!result)
            {
                return NotFound(new { message = "Service not found" });
            }

            return Ok(new { message = "Service updated successfully" });
        }

        [HttpDelete("{providerId}/services/{serviceId}")]
        public async Task<IActionResult> DeleteService(string providerId, int serviceId)
        {
            var result = await _providerService.DeleteServiceAsync(providerId, serviceId);

            if (!result)
            {
                return NotFound(new { message = "Service not found" });
            }

            return Ok(new { message = "Service deleted successfully" });
        }

        [HttpGet("services/{serviceId}")]
        public async Task<IActionResult> GetServiceById(int serviceId)
        {
            var service = await _providerService.GetServiceByIdAsync(serviceId);

            if (service == null)
            {
                return NotFound(new { message = "Service not found" });
            }

            return Ok(service);
        }

        [HttpGet("services/categories")]
        public async Task<IActionResult> GetServiceCategories()
        {
            var categories = await _providerService.GetServiceCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("services/skills")]
        public async Task<IActionResult> GetSkills()
        {
            var categories = await _providerService.GetSkillsAsync();
            return Ok(categories);
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

        [HttpGet("bookings/{providerUserId}")]
        public async Task<IActionResult> GetAllBookings(string providerUserId)
        {
            var bookings = await _providerService.GetAllBookingsAsync(providerUserId);

            if (!bookings.Any())
            {
                return NotFound(new { message = "No bookings found for this provider" });
            }

            return Ok(bookings);
        }

        [HttpGet("notifications/{userId}")]
        public async Task<IActionResult> GetNotificationsByUserId(string userId)
        {
            var notifications = await _providerService.GetNotificationsByProviderIdAsync(userId);

            if (notifications == null || !notifications.Any())
            {
                return NotFound(new { message = "No notifications found" });
            }

            return Ok(notifications);
        }

        [HttpGet("{providerId}/statistics")]
        public async Task<IActionResult> GetProviderStatistics(string providerId)
        {
            var providerStatistics = await _providerService.GetProviderStatisticsAsync(providerId);

            if (providerStatistics == null)
            {
                return NotFound(new { message = "Provider statistics not found" });
            }

            return Ok(providerStatistics);
        }

        [HttpPost("completed-services")]
        public async Task<IActionResult> AddCompletedService([FromForm] CreateCompletedServiceDto dto, IFormFile? image)
        {
            Stream? imageStream = null;
            if (image != null && image.Length > 0)
            {
                imageStream = image.OpenReadStream();
            }

            var result = await _providerService.AddCompletedServiceAsync(dto, imageStream);

            if (imageStream != null)
            {
                await imageStream.DisposeAsync();
            }

            if (!result)
            {
                return NotFound("Booking not found");
            }
            return Ok();
        }

        [HttpGet("completed-services/{providerId}")]
        public async Task<IActionResult> GetAllCompletedServices(string providerId)
        {
            var completedServices = await _providerService.GetAllCompletedServicesAsync(providerId);

            if (completedServices == null || !completedServices.Any())
            {
                return NotFound(new { message = "No completed services found" });
            }

            return Ok(completedServices);
        }

        [HttpPut("completed-services/{completedServiceId}")]
        public async Task<IActionResult> UpdateCompletedService(int completedServiceId, [FromForm] CompletedServiceDto editCompletedServiceDto, IFormFile? imageFile = null)
        {
            Stream? imageStream = null;
            if (imageFile != null)
            {
                imageStream = imageFile.OpenReadStream();
            }

            var result = await _providerService.UpdateCompletedServiceAsync(completedServiceId, editCompletedServiceDto, imageStream);

            if (!result)
            {
                return NotFound(new { message = "Completed service not found" });
            }

            if (imageStream != null)
            {
                await imageStream.DisposeAsync();
            }

            return Ok(new { message = "Completed service updated successfully" });
        }

        [HttpDelete("completed-services/{completedServiceId}")]
        public async Task<IActionResult> DeleteCompletedService(int completedServiceId)
        {
            var result = await _providerService.DeleteCompletedServiceAsync(completedServiceId);

            if (!result)
            {
                return NotFound(new { message = "Completed service not found" });
            }

            return Ok(new { message = "Completed service deleted successfully" });
        }

        [HttpGet("completed-service/{id}")]
        public async Task<IActionResult> GetCompletedServiceById(int id)
        {
            var completedService = await _providerService.GetCompletedServiceByIdAsync(id);

            if (completedService == null)
            {
                return NotFound(new { Message = "Completed service not found." });
            }

            return Ok(completedService);
        }
    }
}
