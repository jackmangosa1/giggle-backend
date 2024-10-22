using Microsoft.AspNetCore.Mvc;
using ServiceManagementAPI.Dtos;
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
        public async Task<IActionResult> GetProviderProfile(int providerId)
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
    }
}
