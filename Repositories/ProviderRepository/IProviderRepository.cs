using ServiceManagementAPI.Dtos;
using ServiceManagementAPI.Enums;

namespace ServiceManagementAPI.Repositories.ProviderRepository
{
    public interface IProviderRepository
    {
        Task<ProviderProfileDto?> GetProviderProfileAsync(string providerId);
        Task<bool> UpdateProviderProfileAsync(string providerId, UpdateProviderProfileDto updateProviderProfileDto, Stream imageStream = null!);
        Task<bool> AddServiceAsync(string providerId, AddServiceDto addServiceDto, Stream? imageStream = null);
        Task<bool> UpdateServiceAsync(string providerId, int serviceId, UpdateServiceDto updateServiceDto, Stream? imageStream = null!);

        Task<ServiceDto?> GetServiceByIdAsync(int serviceId);

        Task<bool> DeleteServiceAsync(string providerId, int serviceId);
        Task<bool> UpdateBookingStatusAsync(int bookingId, BookingStatus status);

        Task<List<ServiceCategoryDto>> GetServiceCategoriesAsync();
        Task<List<SkillDto>> GetSkillsAsync();
        Task<List<BookingDetailsDto>> GetAllBookingsAsync(string providerId);
        Task<List<NotificationDto>> GetNotificationsByProviderIdAsync(string userId);
        Task<ProviderStatisticsDto> GetProviderStatisticsAsync(string providerId);
        Task<bool> AddCompletedServiceAsync(CreateCompletedServiceDto createCompletedServiceDto, Stream? imageStream = null);
        Task<List<CompletedServiceDto>> GetAllCompletedServicesAsync(string providerId);
        Task<bool> UpdateCompletedServiceAsync(int completedServiceId, CompletedServiceDto editCompletedServiceDto, Stream? newImageStream = null);
        Task<bool> DeleteCompletedServiceAsync(int completedServiceId);

        Task<CompletedServiceDto?> GetCompletedServiceByIdAsync(int completedServiceId);
    }
}
