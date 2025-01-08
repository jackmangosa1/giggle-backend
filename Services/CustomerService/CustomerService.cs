using ServiceManagementAPI.Dtos;
using ServiceManagementAPI.Enums;
using ServiceManagementAPI.Repositories.CustomerRepository;

namespace ServiceManagementAPI.Services.CustomerService
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<CustomerProfileDto?> GetCustomerProfileAsync(string customerId)
        {
            return await _customerRepository.GetCustomerProfileAsync(customerId);
        }

        public async Task<bool> UpdateCustomerProfileAsync(string customerId, UpdateCustomerProfileDto updateCustomerProfileDto, Stream imageStream = null!)
        {
            return await _customerRepository.UpdateCustomerProfileAsync(customerId, updateCustomerProfileDto, imageStream);
        }

        public async Task<bool> CreateBookingAsync(BookingDto bookingDto)
        {
            return await _customerRepository.CreateBookingAsync(bookingDto);
        }

        public async Task<List<ProviderDto>> SearchProvidersAsync(string searchTerm)
        {
            return await _customerRepository.SearchProvidersAsync(searchTerm);
        }
        public async Task<List<NotificationDto>> GetNotificationsByUserIdAsync(string userId)
        {
            return await _customerRepository.GetNotificationsByUserIdAsync(userId);
        }


        public async Task<bool> ProcessPaymentAsync(SavePaymentDto savePaymentDto)
        {
            return await _customerRepository.ProcessPaymentAsync(savePaymentDto);
        }

        public async Task<bool> UpdateBookingStatusAsync(int bookingId, BookingStatus status)
        {
            return await _customerRepository.UpdateBookingStatusAsync(bookingId, status);
        }
    }
}
