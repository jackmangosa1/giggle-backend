using ServiceManagementAPI.Dtos;
using ServiceManagementAPI.Enums;

namespace ServiceManagementAPI.Repositories.CustomerRepository
{
    public interface ICustomerRepository
    {
        Task<bool> UpdateCustomerProfileAsync(string customerId, UpdateCustomerProfileDto updateCustomerProfileDto, Stream imageStream = null!);
        Task<CustomerProfileDto?> GetCustomerProfileAsync(string customerId);
        Task<bool> CreateBookingAsync(BookingDto bookingDto);
        Task<List<ProviderDto>> SearchProvidersAsync(string searchTerm);
        Task<List<NotificationDto>> GetNotificationsByUserIdAsync(string userId);
        Task<bool> ProcessPaymentAsync(SavePaymentDto savePaymentDto);
        Task<bool> UpdateBookingStatusAsync(int bookingId, BookingStatus status);
    }
}
