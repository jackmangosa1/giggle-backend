using ServiceManagementAPI.Dtos;
using ServiceManagementAPI.Enums;

namespace ServiceManagementAPI.Repositories.ProviderRepository
{
    public interface IProviderRepository
    {
        Task<ProviderProfileDto?> GetProviderProfileAsync(int providerId);
        Task<bool> UpdateProviderProfileAsync(int providerId, UpdateProviderProfileDto updateProviderProfileDto, Stream imageStream = null!);
        Task<bool> AddServiceAsync(string providerId, AddServiceDto addServiceDto, Stream? imageStream = null);
        Task<bool> UpdateBookingStatusAsync(int bookingId, BookingStatus status);
    }
}
