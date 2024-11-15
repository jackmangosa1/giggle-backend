﻿using ServiceManagementAPI.Dtos;
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

        public async Task<ProviderProfileDto?> GetProviderProfileAsync(int providerId)
        {
            return await _providerRepository.GetProviderProfileAsync(providerId);
        }

        public async Task<bool> UpdateProviderProfileAsync(int providerId, UpdateProviderProfileDto updateProviderProfileDto, Stream imageStream = null!)
        {
            return await _providerRepository.UpdateProviderProfileAsync(providerId, updateProviderProfileDto, imageStream);
        }

        public async Task<bool> AddServiceAsync(int providerId, AddServiceDto addServiceDto, Stream imageStream = null!)
        {
            return await _providerRepository.AddServiceAsync(providerId, addServiceDto, imageStream);
        }

        public async Task<bool> UpdateBookingStatusAsync(int bookingId, BookingStatus bookingStatus)
        {
            return await _providerRepository.UpdateBookingStatusAsync(bookingId, bookingStatus);
        }
    }
}
