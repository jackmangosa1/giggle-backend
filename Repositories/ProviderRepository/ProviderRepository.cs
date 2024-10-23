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
                .Include(p => p.Skills) // Include related skills
                .FirstOrDefaultAsync(p => p.Id == providerId);

            if (provider == null)
            {
                return null;
            }

            // Map skills to a list of skill names for the DTO
            var skillNames = provider.Skills.Select(skill => skill.Name).ToList();

            return new ProviderProfileDto
            {
                Id = provider.Id,
                DisplayName = provider.DisplayName,
                Bio = provider.Bio,
                Skills = skillNames, // List of skill names
                ProfilePictureUrl = provider.ProfilePictureUrl,
                UserName = provider.User.UserName, // From AspNetUser
                Email = provider.User.Email        // From AspNetUser
            };
        }

        public async Task<bool> UpdateProviderProfileAsync(int providerId, UpdateProviderProfileDto updateProviderProfileDto, Stream imageStream = null!)
        {
            var provider = await _context.Providers
                .Include(p => p.Skills) // Include existing skills
                .FirstOrDefaultAsync(p => p.Id == providerId);

            if (provider == null)
            {
                return false;
            }

            // Update profile fields
            provider.DisplayName = updateProviderProfileDto.DisplayName;
            provider.Bio = updateProviderProfileDto.Bio;

            // Update skills: clear existing skills and add new ones based on SkillIds
            provider.Skills.Clear();
            if (updateProviderProfileDto.SkillIds != null && updateProviderProfileDto.SkillIds.Any())
            {
                // Fetch the skills from the database using the SkillIds provided
                var skills = await _context.Skills
                    .Where(skill => updateProviderProfileDto.SkillIds.Contains(skill.Id))
                    .ToListAsync();

                provider.Skills = skills;
            }

            // Handle profile picture update
            if (imageStream != null)
            {
                var containerName = "profile-pictures";
                var imageUrl = await _blobStorageUtil.UploadImageToBlobAsync(imageStream, updateProviderProfileDto.ImageFileName!, containerName);
                provider.ProfilePictureUrl = imageUrl;
            }

            _context.Providers.Update(provider);
            await _context.SaveChangesAsync();

            return true;
        }

    }
}
