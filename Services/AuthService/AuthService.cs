using ServiceManagementAPI.Dtos;
using ServiceManagementAPI.Repositories.AuthRepository;

namespace ServiceManagementAPI.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;

        public AuthService(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        public async Task<AuthResultDto> RegisterCustomerAsync(UserRegistrationDto registration)
        {
            return await _authRepository.RegisterCustomerAsync(registration);
        }

        public async Task<AuthResultDto> RegisterProviderAsync(UserRegistrationDto registration)
        {
            return await _authRepository.RegisterProviderAsync(registration);
        }

        public async Task<AuthResultDto> VerifyEmailAsync(string userId, string token)
        {
            return await _authRepository.VerifyEmailAsync(userId, token);
        }

        public async Task<ForgotPasswordtResultDto> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            return await _authRepository.ForgotPasswordAsync(forgotPasswordDto);
        }

        public async Task<PasswordResetResultDto> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            return await _authRepository.ResetPasswordAsync(resetPasswordDto);
        }

        public async Task<LoginResultDto> LoginAsync(string email, string password)
        {
            return await _authRepository.LoginAsync(email, password);
        }

        public async Task<bool> IsEmailConfirmedAsync(string email)
        {
            return await _authRepository.IsEmailConfirmedAsync(email);
        }
    }
}