using ServiceManagementAPI.Dtos;

namespace ServiceManagementAPI.Repositories.AuthRepository
{
    public interface IAuthRepository
    {
        Task<AuthResultDto> RegisterCustomerAsync(UserRegistrationDto registration);
        Task<AuthResultDto> RegisterProviderAsync(UserRegistrationDto registration);
        Task<AuthResultDto> VerifyEmailAsync(string userId, string token);
        Task<ForgotPasswordtResultDto> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
        Task<PasswordResetResultDto> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task<LoginResultDto> LoginAsync(string email, string password);
        Task<bool> IsEmailConfirmedAsync(string email);
    }
}
