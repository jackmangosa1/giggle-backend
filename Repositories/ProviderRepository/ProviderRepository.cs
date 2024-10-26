using Microsoft.EntityFrameworkCore;
using ServiceManagementAPI.Data;
using ServiceManagementAPI.Dtos;
using ServiceManagementAPI.Entities;
using ServiceManagementAPI.Enums;
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
                .Include(p => p.User)
                .Include(p => p.Skills)
                .Include(p => p.Services)
                    .ThenInclude(s => s.Category)
                .FirstOrDefaultAsync(p => p.Id == providerId);

            if (provider == null)
            {
                return null;
            }

            var skillNames = provider.Skills.Select(skill => skill.Name).ToList();

            var serviceDtos = provider.Services.Select(service => new ServiceDto
            {
                Id = service.Id,
                Name = service.Name,
                Description = service.Description,
                Price = service.Price,
                MediaUrl = service.MediaUrl,
                CategoryName = service.Category.Name,
                PriceType = (PriceType)service.PriceType
            }).ToList();

            return new ProviderProfileDto
            {
                Id = provider.Id,
                DisplayName = provider.DisplayName,
                Bio = provider.Bio,
                Skills = skillNames,
                Services = serviceDtos,
                ProfilePictureUrl = provider.ProfilePictureUrl,
                UserName = provider.User.UserName,
                Email = provider.User.Email
            };
        }

        public async Task<bool> UpdateProviderProfileAsync(int providerId, UpdateProviderProfileDto updateProviderProfileDto, Stream? imageStream = null)
        {
            var provider = await _context.Providers
                .Include(p => p.Skills)
                .FirstOrDefaultAsync(p => p.Id == providerId);

            if (provider == null)
            {
                return false;
            }

            provider.DisplayName = updateProviderProfileDto.DisplayName;
            provider.Bio = updateProviderProfileDto.Bio;

            provider.Skills.Clear();

            if (updateProviderProfileDto.SkillNames != null && updateProviderProfileDto.SkillNames.Any())
            {
                foreach (var skillName in updateProviderProfileDto.SkillNames)
                {
                    var existingSkill = await _context.Skills
                        .FirstOrDefaultAsync(s => s.Name == skillName);

                    if (existingSkill != null)
                    {
                        provider.Skills.Add(existingSkill);
                    }
                    else
                    {
                        var newSkill = new Skill
                        {
                            Name = skillName
                        };

                        _context.Skills.Add(newSkill);
                        await _context.SaveChangesAsync();
                        provider.Skills.Add(newSkill);
                    }
                }
            }

            if (imageStream != null)
            {
                var containerName = "profile-pictures";
                var fileName = provider.DisplayName;
                var imageUrl = await _blobStorageUtil.UploadImageToBlobAsync(imageStream, fileName!, containerName);
                provider.ProfilePictureUrl = imageUrl;
            }

            _context.Providers.Update(provider);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> AddServiceAsync(int providerId, AddServiceDto addServiceDto, Stream? imageStream = null)
        {
            // Check if the provider exists
            var provider = await _context.Providers
                .FirstOrDefaultAsync(p => p.Id == providerId);

            if (provider == null)
            {
                return false; // Provider not found
            }

            // Check if the service category exists, if not create a new one
            var category = await _context.ServiceCategories
                .FirstOrDefaultAsync(c => c.Name.ToLower() == addServiceDto.CategoryName.ToLower());

            if (category == null)
            {
                // Create a new service category if it doesn't exist
                category = new ServiceCategory
                {
                    Name = addServiceDto.CategoryName
                };

                _context.ServiceCategories.Add(category);
                await _context.SaveChangesAsync(); // Save to get the generated Id
            }

            // Initialize the mediaUrl
            string? mediaUrl = null;

            // Upload the service image if provided
            if (imageStream != null)
            {
                var containerName = "service-images";
                var uniqueFileName = Guid.NewGuid().ToString();

                // Upload the image to blob storage
                mediaUrl = await _blobStorageUtil.UploadImageToBlobAsync(imageStream, uniqueFileName, containerName);
            }

            // Create a new service entity
            var newService = new Service
            {
                Name = addServiceDto.Name,
                Description = addServiceDto.Description,
                Price = addServiceDto.Price,
                PriceType = (int)addServiceDto.PriceType,
                MediaUrl = mediaUrl, // This will be null if no image is uploaded
                ProviderId = provider.Id, // Associate with the provider
                CategoryId = category.Id // Associate with the newly created or existing service category
            };

            // Add the new service to the provider's services collection
            provider.Services.Add(newService);

            // Save changes to the database
            await _context.SaveChangesAsync();

            return true;
        }


    }
}
