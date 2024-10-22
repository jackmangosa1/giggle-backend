using Microsoft.EntityFrameworkCore;
using ServiceManagementAPI.Data;
using ServiceManagementAPI.Dtos;
using ServiceManagementAPI.Utils;

namespace ServiceManagementAPI.Repositories.ProviderRepository
{
    public class ProviderRepository : IProviderRepository
    {
        private readonly ServiceManagementDbContext _context;
        private readonly BlobStorageUtil _blobStorageUtil;

        public ProviderRepository(ServiceManagementDbContext context, BlobStorageUtil blobStorageUtil)
        {
            _context = context;
            _blobStorageUtil = blobStorageUtil;
        }

        public async Task<ProviderProfileDto?> GetProviderProfileAsync(int providerId)
        {
            var provider = await _context.Providers
                .Include(p => p.User) // Include AspNetUser data
                .FirstOrDefaultAsync(p => p.Id == providerId);

            if (provider == null)
            {
                return null;
            }

            return new ProviderProfileDto
            {
                Id = provider.Id,
                DisplayName = provider.DisplayName,
                Bio = provider.Bio,
                Skills = provider.Skills,
                ProfilePictureUrl = provider.ProfilePictureUrl,
                UserName = provider.User.UserName, // From AspNetUser
                Email = provider.User.Email        // From AspNetUser
            };
        }

        public async Task<bool> UpdateProviderProfileAsync(int providerId, UpdateProviderProfileDto updateProviderProfileDto, Stream imageStream = null!)
        {
            var provider = await _context.Providers.FirstOrDefaultAsync(p => p.Id == providerId);

            if (provider == null)
            {
                return false;
            }

            // Update profile fields
            provider.DisplayName = updateProviderProfileDto.DisplayName;
            provider.Bio = updateProviderProfileDto.Bio;
            provider.Skills = updateProviderProfileDto.Skills;

            // Handle profile picture update
            if (imageStream != null)
            {
                var containerName = "profile-pictures";
                var imageUrl = await _blobStorageUtil.UploadImageToBlobAsync(imageStream, updateProviderProfileDto.ImageFileName, containerName);
                provider.ProfilePictureUrl = imageUrl;
            }

            _context.Providers.Update(provider);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
