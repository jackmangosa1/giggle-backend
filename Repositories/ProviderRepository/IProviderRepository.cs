using ServiceManagementAPI.Dtos;

namespace ServiceManagementAPI.Repositories.ProviderRepository
{
    public interface IProviderRepository
    {
        Task<ProviderProfileDto?> GetProviderProfileAsync(int providerId);
        Task<bool> UpdateProviderProfileAsync(int providerId, UpdateProviderProfileDto updateProviderProfileDto, Stream imageStream = null!);
        Task<bool> AddServiceAsync(int providerId, AddServiceDto addServiceDto, Stream? imageStream = null);
    }
}
