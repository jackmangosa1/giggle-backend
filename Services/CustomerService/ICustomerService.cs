using ServiceManagementAPI.Dtos;

namespace ServiceManagementAPI.Services.CustomerService
{
    public interface ICustomerService
    {
        Task<bool> UpdateCustomerProfileAsync(int customerId, UpdateCustomerProfileDto updateCustomerProfileDto, Stream imageStream = null!);
        Task<CustomerProfileDto?> GetCustomerProfileAsync(int customerId);
        Task<bool> CreateBookingAsync(BookingDto bookingDto);
    }
}
