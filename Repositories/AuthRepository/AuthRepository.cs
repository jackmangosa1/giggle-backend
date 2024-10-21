using Microsoft.AspNetCore.Identity;
using ServiceManagementAPI.Data;
using ServiceManagementAPI.Dtos;
using ServiceManagementAPI.Entities;
using ServiceManagementAPI.Services.EmailService;
using ServiceManagementAPI.Utils;
using System.Security.Claims;

namespace ServiceManagementAPI.Repositories.AuthRepository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ServiceManagementDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AuthRepository(
            ServiceManagementDbContext context,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<AuthResultDto> RegisterCustomerAsync(UserRegistrationDto registration)
        {
            var user = new IdentityUser { UserName = registration.UserName, Email = registration.Email };
            var result = await _userManager.CreateAsync(user, registration.Password);

            if (result.Succeeded)
            {
                var customerProfile = new Customer
                {
                    UserId = user.Id,
                    Address = null,
                    PhoneNumber = null,
                    PreferredPaymentMethod = null,
                    FullName = null,
                    ProfilePictureUrl = null
                };

                _context.Customers.Add(customerProfile);
                await _context.SaveChangesAsync();

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = UrlGenerator.GenerateEmailConfirmationLink(user.Id, token, _configuration["ApplicationUrl"]!);

                var emailSubject = "Confirm your email";
                var emailBody = $@"
                <h2>Welcome to our service management platform!</h2>
                <p>Please confirm your email by clicking the link below:</p>
                <a href='{confirmationLink}'>Confirm Email</a>
                ";

                await _emailService.SendEmailAsync(user.Email, emailSubject, emailBody);

                var claims = new List<Claim>
                {
                    new Claim("UserName", registration.UserName),
                    new Claim("Role", "Customer")
                };

                foreach (var claim in claims)
                {
                    await _userManager.AddClaimAsync(user, claim);
                }

                return new AuthResultDto
                {
                    Succeeded = true,
                    UserId = user.Id,
                    Token = null,
                    Errors = new List<string>(),
                    RequiresEmailConfirmation = true
                };
            }

            return new AuthResultDto
            {
                Succeeded = false,
                UserId = user.Id,
                Errors = result.Errors.Select(e => e.Description).ToList()
            };
        }

        public async Task<AuthResultDto> RegisterProviderAsync(UserRegistrationDto registration)
        {
            var user = new IdentityUser { UserName = registration.UserName, Email = registration.Email };
            var result = await _userManager.CreateAsync(user, registration.Password);

            if (result.Succeeded)
            {
                var providerProfile = new Provider
                {
                    UserId = user.Id,
                    Bio = null,
                    Skills = null,
                    DisplayName = null,
                    ProfilePictureUrl = null
                };

                _context.Providers.Add(providerProfile);
                await _context.SaveChangesAsync();

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = UrlGenerator.GenerateEmailConfirmationLink(user.Id, token, _configuration["ApplicationUrl"]!);

                var emailSubject = "Confirm your email (Provider)";
                var emailBody = $@"
                <h2>Welcome to our platform!</h2>
                <p>Please confirm your email by clicking the link below:</p>
                <a href='{confirmationLink}'>Confirm Email</a>
                ";

                await _emailService.SendEmailAsync(user.Email, emailSubject, emailBody);

                var claims = new List<Claim>
                {
                    new Claim("UserName", registration.UserName),
                    new Claim("Role", "Provider")
                };

                foreach (var claim in claims)
                {
                    await _userManager.AddClaimAsync(user, claim);
                }

                return new AuthResultDto
                {
                    Succeeded = true,
                    UserId = user.Id,
                    Token = null,
                    Errors = new List<string>(),
                    RequiresEmailConfirmation = true
                };
            }

            return new AuthResultDto
            {
                Succeeded = false,
                UserId = user.Id,
                Errors = result.Errors.Select(e => e.Description).ToList()
            };
        }

        public async Task<LoginResultDto> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new LoginResultDto { Succeeded = false };
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                return new LoginResultDto
                {
                    Succeeded = false,
                    RequiresEmailConfirmation = true
                };
            }

            var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var claims = await _userManager.GetClaimsAsync(user);
                var token = JwtUtils.GenerateToken(_configuration, user, claims);

                return new LoginResultDto
                {
                    Succeeded = true,
                    UserId = user.Id,
                    UserName = user.UserName,
                    Token = token,
                    Roles = claims.Where(c => c.Type == "Role")
                                .Select(c => c.Value)
                                .ToList()
                };
            }

            return new LoginResultDto
            {
                Succeeded = false,
                RequiresTwoFactor = result.RequiresTwoFactor,
                IsLockedOut = result.IsLockedOut
            };
        }

        public async Task<AuthResultDto> VerifyEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new AuthResultDto
                {
                    Succeeded = false,
                    Errors = new List<string> { "Invalid user ID" }
                };
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            return new AuthResultDto
            {
                Succeeded = result.Succeeded,
                UserId = user.Id,
                Errors = result.Errors.Select(e => e.Description).ToList()
            };
        }

        public async Task<bool> IsEmailConfirmedAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user != null && await _userManager.IsEmailConfirmedAsync(user);
        }

        public async Task<bool> InitiatePasswordResetAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
            {
                return true;
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = UrlGenerator.GeneratePasswordResetLink(user.Id, token, _configuration["ApplicationUrl"]!);

            var emailSubject = "Reset Your Password";
            var emailBody = $@"
                <h2>Password Reset Request</h2>
                <p>We received a request to reset your password. If you did not make this request, please ignore this email.</p>
                <p>To reset your password, click the link below:</p>
                <a href='{resetLink}'>Reset Password</a>
                <p>This link will expire in 24 hours.</p>
            ";

            await _emailService.SendEmailAsync(user.Email!, emailSubject, emailBody);

            return true;
        }

        public async Task<ForgotPasswordtResultDto> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
            var result = new ForgotPasswordtResultDto();

            if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
            {
                result.Succeeded = false;
                result.Message!.Add("Invalid email or unconfirmed.");
                return result;
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var tokenExpirationInHours = int.Parse(_configuration["TokenExpirationInHours"]!);
            var encodedToken = System.Web.HttpUtility.UrlEncode(token);

            var resetUrl = $"{forgotPasswordDto.ClientUri}?email={user.Email}&token={encodedToken}";
            var emailSubject = "Reset Password";
            var emailBody = $"Click <a href='{resetUrl}'>here</a> to reset your password. The link expires in {tokenExpirationInHours} hours.";

            await _emailService.SendEmailAsync(user.Email!, emailSubject, emailBody);

            result.Succeeded = true;
            return result;
        }

        public async Task<PasswordResetResultDto> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
            {
                return new PasswordResetResultDto { Success = false, Errors = new List<string> { "Invalid user" } };
            }

            var decodedToken = System.Web.HttpUtility.UrlDecode(resetPasswordDto.Token);
            var result = await _userManager.ResetPasswordAsync(user, decodedToken, resetPasswordDto.Password);

            if (!result.Succeeded)
            {
                return new PasswordResetResultDto
                {
                    Success = false,
                    Errors = result.Errors.Select(e => e.Description).ToList()
                };
            }

            return new PasswordResetResultDto { Success = true };
        }
    }
}