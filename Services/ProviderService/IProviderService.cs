using ServiceManagementAPI.Dtos;

namespace ServiceManagementAPI.Services.ProviderService
{
    public interface IProviderService
    {
        Task<ProviderProfileDto?> GetProviderProfileAsync(int providerId);
        Task<bool> UpdateProviderProfileAsync(int providerId, UpdateProviderProfileDto updateProviderProfileDto, Stream imageStream = null!);
    }
}
