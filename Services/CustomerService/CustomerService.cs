using ServiceManagementAPI.Dtos;
using ServiceManagementAPI.Repositories.CustomerRepository;

namespace ServiceManagementAPI.Services.CustomerService
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _userRepository;
        public CustomerService(ICustomerRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<CustomerProfileDto?> GetCustomerProfileAsync(int customerId)
        {
            return await _userRepository.GetCustomerProfileAsync(customerId);
        }

        public async Task<bool> UpdateCustomerProfileAsync(int customerId, UpdateCustomerProfileDto updateCustomerProfileDto, Stream imageStream = null!)
        {
            return await _userRepository.UpdateCustomerProfileAsync(customerId, updateCustomerProfileDto, imageStream);
        }

        public async Task<bool> CreateBookingAsync(BookingDto bookingDto)
        {
            return await _userRepository.CreateBookingAsync(bookingDto);
        }
    }
}
