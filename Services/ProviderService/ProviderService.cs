using ServiceManagementAPI.Dtos;
using ServiceManagementAPI.Enums;
using ServiceManagementAPI.Repositories.ProviderRepository;

namespace ServiceManagementAPI.Services.ProviderService
{
    public class ProviderService : IProviderService
    {
        private readonly IProviderRepository _providerRepository;

        public ProviderService(IProviderRepository providerRepository)
        {
            _providerRepository = providerRepository;
        }

        public async Task<ProviderProfileDto?> GetProviderProfileAsync(string providerId)
        {
            return await _providerRepository.GetProviderProfileAsync(providerId);
        }

        public async Task<bool> UpdateProviderProfileAsync(string providerId, UpdateProviderProfileDto updateProviderProfileDto, Stream imageStream = null!)
        {
            return await _providerRepository.UpdateProviderProfileAsync(providerId, updateProviderProfileDto, imageStream);
        }

        public async Task<bool> AddServiceAsync(string providerId, AddServiceDto addServiceDto, Stream imageStream = null!)
        {
            return await _providerRepository.AddServiceAsync(providerId, addServiceDto, imageStream);
        }

        public async Task<bool> UpdateServiceAsync(string providerId, int serviceId, UpdateServiceDto updateServiceDto, Stream? imageStream = null!)
        {
            return await _providerRepository.UpdateServiceAsync(providerId, serviceId, updateServiceDto, imageStream);
        }

        public async Task<ServiceDto?> GetServiceByIdAsync(int serviceId)
        {
            return await _providerRepository.GetServiceByIdAsync(serviceId);
        }

        public async Task<bool> DeleteServiceAsync(string providerId, int serviceId)
        {
            return await _providerRepository.DeleteServiceAsync(providerId, serviceId);
        }

        public async Task<bool> UpdateBookingStatusAsync(int bookingId, BookingStatus bookingStatus)
        {
            return await _providerRepository.UpdateBookingStatusAsync(bookingId, bookingStatus);
        }

        public async Task<List<ServiceCategoryDto>> GetServiceCategoriesAsync()
        {
            return await _providerRepository.GetServiceCategoriesAsync();
        }

        public async Task<List<SkillDto>> GetSkillsAsync()
        {
            return await _providerRepository.GetSkillsAsync();
        }

        public async Task<List<BookingDetailsDto>> GetAllBookingsAsync(string providerId)
        {
            return await _providerRepository.GetAllBookingsAsync(providerId);
        }

        public async Task<List<NotificationDto>> GetNotificationsByProviderIdAsync(string userId)
        {
            return await _providerRepository.GetNotificationsByProviderIdAsync(userId);
        }

        public async Task<ProviderStatisticsDto> GetProviderStatisticsAsync(string providerId)
        {
            return await _providerRepository.GetProviderStatisticsAsync(providerId);
        }

        public async Task<bool> AddCompletedServiceAsync(CreateCompletedServiceDto createCompletedServiceDto, Stream? imageStream = null)
        {
            return await _providerRepository.AddCompletedServiceAsync(createCompletedServiceDto, imageStream);
        }

        public async Task<List<CompletedServiceDto>> GetAllCompletedServicesAsync(string providerId)
        {
            return await _providerRepository.GetAllCompletedServicesAsync(providerId);
        }

        public async Task<bool> UpdateCompletedServiceAsync(int completedServiceId, CompletedServiceDto editCompletedServiceDto, Stream? newImageStream = null)
        {
            return await _providerRepository.UpdateCompletedServiceAsync(completedServiceId, editCompletedServiceDto, newImageStream);
        }
        public async Task<bool> DeleteCompletedServiceAsync(int completedServiceId)
        {
            return await _providerRepository.DeleteCompletedServiceAsync(completedServiceId);
        }

        public async Task<CompletedServiceDto?> GetCompletedServiceByIdAsync(int completedServiceId)
        {
            return await _providerRepository.GetCompletedServiceByIdAsync(completedServiceId);
        }
    }
}
