using Microsoft.AspNetCore.Mvc;
using ServiceManagementAPI.Dtos;
using ServiceManagementAPI.Services.AuthService;

namespace ServiceManagementAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("RegisterCustomer")]
        public async Task<IActionResult> RegisterCustomer(UserRegistrationDto registration)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RegisterCustomerAsync(registration);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok(result);
        }

        [HttpPost("RegisterProvider")]
        public async Task<IActionResult> RegisterProvider(UserRegistrationDto registration)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RegisterProviderAsync(registration);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok(result);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.LoginAsync(loginDto.Email, loginDto.Password);
            if (!result.Succeeded)
            {
                if (result.RequiresEmailConfirmation)
                {
                    return BadRequest("Email confirmation required.");
                }
                return BadRequest("Invalid credentials.");
            }

            return Ok(result);
        }

        [HttpGet("VerifyEmail")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string userId, [FromQuery] string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
            {
                return BadRequest("UserId and Token are required.");
            }

            var result = await _authService.VerifyEmailAsync(userId, token);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok(result);
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.ForgotPasswordAsync(forgotPasswordDto);
            if (!result.Succeeded)
            {
                return BadRequest(result.Message);
            }

            return Ok(new { Message = result.Message });
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (resetPasswordDto.Password != resetPasswordDto.ConfirmPassword)
            {
                return BadRequest("Passwords do not match.");
            }

            var result = await _authService.ResetPasswordAsync(resetPasswordDto);
            if (!result.Success)
            {
                return BadRequest(result.Errors);
            }

            return Ok(new { Message = "Password has been reset successfully." });
        }

        [HttpGet("IsEmailConfirmed")]
        public async Task<IActionResult> IsEmailConfirmed([FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest("Email is required.");
            }

            var isConfirmed = await _authService.IsEmailConfirmedAsync(email);
            return Ok(new { EmailConfirmed = isConfirmed });

        }
    }
}