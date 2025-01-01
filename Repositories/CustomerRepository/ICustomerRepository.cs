using ServiceManagementAPI.Dtos;

namespace ServiceManagementAPI.Repositories.CustomerRepository
{
    public interface ICustomerRepository
    {
        Task<bool> UpdateCustomerProfileAsync(int customerId, UpdateCustomerProfileDto updateCustomerProfileDto, Stream imageStream = null!);
        Task<CustomerProfileDto?> GetCustomerProfileAsync(int customerId);
        Task<bool> CreateBookingAsync(BookingDto bookingDto);
        Task<List<ProviderDto>> SearchProvidersAsync(string searchTerm);

        Task<List<NotificationDto>> GetNotificationsByUserIdAsync(string userId);
    }
}
