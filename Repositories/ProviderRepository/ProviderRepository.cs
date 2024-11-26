using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ServiceManagementAPI.Data;
using ServiceManagementAPI.Dtos;
using ServiceManagementAPI.Entities;
using ServiceManagementAPI.Enums;
using ServiceManagementAPI.Hubs;
using ServiceManagementAPI.Utils;

namespace ServiceManagementAPI.Repositories.ProviderRepository
{
    public class ProviderRepository : IProviderRepository
    {
        private readonly ServiceManagementDbContext _context;
        private readonly BlobStorageUtil _blobStorageUtil;
        private readonly IHubContext<NotificationHub> _hubContext;

        public ProviderRepository(ServiceManagementDbContext context, BlobStorageUtil blobStorageUtil, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _blobStorageUtil = blobStorageUtil;
            _hubContext = hubContext;
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

        public async Task<bool> AddServiceAsync(string providerId, AddServiceDto addServiceDto, Stream? imageStream = null)
        {
            var provider = await _context.Providers
                .FirstOrDefaultAsync(p => p.User.Id == providerId);

            if (provider == null)
            {
                return false;
            }

            var category = await _context.ServiceCategories
                .FirstOrDefaultAsync(c => c.Name.ToLower() == addServiceDto.CategoryName.ToLower());

            if (category == null)
            {
                category = new ServiceCategory
                {
                    Name = addServiceDto.CategoryName
                };

                _context.ServiceCategories.Add(category);
                await _context.SaveChangesAsync();
            }

            string? mediaUrl = null;

            if (imageStream != null)
            {
                var containerName = "service-images";
                var uniqueFileName = Guid.NewGuid().ToString();
                mediaUrl = await _blobStorageUtil.UploadImageToBlobAsync(imageStream, uniqueFileName, containerName);
            }

            var newService = new Service
            {
                Name = addServiceDto.Name,
                Description = addServiceDto.Description,
                Price = addServiceDto.Price,
                PriceType = (int)addServiceDto.PriceType,
                MediaUrl = mediaUrl,
                ProviderId = provider.Id,
                CategoryId = category.Id
            };

            provider.Services.Add(newService);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateBookingStatusAsync(int bookingId, BookingStatus status)
        {
            var booking = await _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Service)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null)
            {
                return false;
            }

            booking.Status = (int)status;
            _context.Bookings.Update(booking);

            var notification = new Notification
            {
                UserId = booking.Customer.UserId,
                Type = (int)NotificationTypes.BookingStatusChange,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };
            _context.Notifications.Add(notification);

            await _context.SaveChangesAsync();

            string statusMessage = status == BookingStatus.Approved ? "approved" : "rejected";
            string notificationMessage = $"Your booking for {booking.Service.Name} has been {statusMessage}.";

            await _hubContext.Clients.User(booking.Customer.UserId.ToString()).SendAsync("ReceiveNotification", notificationMessage);

            return true;
        }


    }
}
