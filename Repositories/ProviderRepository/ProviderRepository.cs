using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
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

        public async Task<ProviderProfileDto?> GetProviderProfileAsync(string providerId)
        {
            var provider = await _context.Providers
                .Include(p => p.User)
                .Include(p => p.Skills)
                .Include(p => p.Services)
                    .ThenInclude(s => s.Category)
                .Include(p => p.Services)
                     .ThenInclude(s => s.Bookings)
                    .ThenInclude(b => b.Customer)
                    .ThenInclude(c => c.User)
                .Include(p => p.Services)
                    .ThenInclude(s => s.Bookings)
                    .ThenInclude(b => b.CompletedServices)
                    .ThenInclude(cs => cs.Reviews)
                .FirstOrDefaultAsync(p => p.User.Id == providerId);


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

            var completedServices = provider.Services
                .SelectMany(service => service.Bookings)
                .SelectMany(booking => booking.CompletedServices)
                .Select(completedService => new CompletedServiceDto
                {
                    Id = completedService.Id,
                    Description = completedService.Description,
                    MediaUrl = completedService.MediaUrl,
                    UserId = completedService.Booking.Customer.User.Id,
                    CompletedAt = completedService.CompletedAt,
                    Reviews = completedService.Reviews.Select(review => new ReviewDto
                    {
                        Id = review.Id,
                        UserId = review.UserId,
                        Rating = review.Rating,
                        Comment = review.Comment,
                        CreatedAt = review.CreatedAt,
                        UserName = review.User.UserName!
                    }).ToList()
                })
                .ToList();

            return new ProviderProfileDto
            {
                Id = provider.Id,
                DisplayName = provider.DisplayName,
                Bio = provider.Bio,
                Skills = skillNames,
                Services = serviceDtos,
                CompletedServices = completedServices,
                ProfilePictureUrl = provider.ProfilePictureUrl,
                UserName = provider.User.UserName,
                Email = provider.User.Email
            };
        }



        public async Task<bool> UpdateProviderProfileAsync(string providerId, UpdateProviderProfileDto updateProviderProfileDto, Stream? imageStream = null)
        {
            var provider = await _context.Providers
                .Include(p => p.Skills)
                .FirstOrDefaultAsync(p => p.User.Id == providerId);

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

        public async Task<bool> UpdateServiceAsync(string providerId, int serviceId, UpdateServiceDto updateServiceDto, Stream? imageStream = null)
        {
            var service = await _context.Services
                .Include(s => s.Provider)
                .FirstOrDefaultAsync(s => s.Id == serviceId && s.Provider.User.Id == providerId);

            if (service == null)
            {
                return false;
            }

            var category = await _context.ServiceCategories
                .FirstOrDefaultAsync(c => c.Name.ToLower() == updateServiceDto.CategoryName.ToLower());

            if (category == null)
            {
                category = new ServiceCategory
                {
                    Name = updateServiceDto.CategoryName
                };

                _context.ServiceCategories.Add(category);
                await _context.SaveChangesAsync();
            }

            string? mediaUrl = service.MediaUrl;

            if (imageStream != null)
            {
                var containerName = "service-images";
                var uniqueFileName = Guid.NewGuid().ToString();
                mediaUrl = await _blobStorageUtil.UploadImageToBlobAsync(imageStream, uniqueFileName, containerName);
            }

            service.Name = updateServiceDto.Name;
            service.Description = updateServiceDto.Description;
            service.Price = updateServiceDto.Price;
            service.PriceType = (int)updateServiceDto.PriceType;
            service.CategoryId = category.Id;
            service.MediaUrl = mediaUrl;

            _context.Services.Update(service);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteServiceAsync(string providerId, int serviceId)
        {
            var service = await _context.Services
                .Include(s => s.Provider)
                .FirstOrDefaultAsync(s => s.Id == serviceId && s.Provider.User.Id == providerId);

            if (service == null)
            {
                return false;
            }

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();

            return true;
        }


        public async Task<bool> UpdateBookingStatusAsync(int bookingId, BookingStatus status)
        {
            var booking = await _context.Bookings
                .Include(b => b.Customer)
                    .ThenInclude(c => c.User)
                .Include(b => b.Service)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null)
            {
                return false;
            }

            switch (status)
            {
                case BookingStatus.Approved:
                    booking.BookingStatus = (int)BookingStatus.Approved;
                    break;
                case BookingStatus.Rejected:
                    booking.BookingStatus = (int)BookingStatus.Rejected;
                    break;
                case BookingStatus.Completed:
                    booking.BookingStatus = (int)BookingStatus.Completed;
                    break;
                default:
                    return false;
            }

            _context.Bookings.Update(booking);

            var notification = new Notification
            {
                UserId = booking.Customer.UserId,
                Type = (int)NotificationTypes.BookingStatusChange,
                BookingStatus = (int)status,
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                BookingId = bookingId,
                CustomerName = booking.Customer.FullName,
                Email = booking.Customer.User.Email,
                PhoneNumber = booking.Customer.PhoneNumber
            };
            _context.Notifications.Add(notification);

            await _context.SaveChangesAsync();

            string statusMessage = status switch
            {
                BookingStatus.Approved => "approved",
                BookingStatus.Rejected => "rejected",
                BookingStatus.Completed => "service completed",
                _ => "status changed"
            };

            string notificationMessage = $"Your booking for {booking.Service.Name} has been {statusMessage}.";

            await _hubContext.Clients.All.SendAsync("ReceiveNotification", notificationMessage);

            return true;
        }


        public async Task<ServiceDto?> GetServiceByIdAsync(int serviceId)
        {
            var service = await _context.Services
                .Include(s => s.Category)
                .FirstOrDefaultAsync(s => s.Id == serviceId);

            if (service == null)
            {
                return null;
            }

            return new ServiceDto
            {
                Id = service.Id,
                Name = service.Name,
                Description = service.Description,
                Price = service.Price,
                MediaUrl = service.MediaUrl,
                CategoryName = service.Category?.Name!,
                PriceType = (PriceType)service.PriceType
            };
        }

        public async Task<List<ServiceCategoryDto>> GetServiceCategoriesAsync()
        {
            var categories = await _context.ServiceCategories.ToListAsync();

            return categories.Select(category => new ServiceCategoryDto
            {
                Id = category.Id,
                Name = category.Name
            }).ToList();
        }

        public async Task<List<SkillDto>> GetSkillsAsync()
        {
            var skills = await _context.Skills.ToListAsync();

            return skills.Select(skill => new SkillDto
            {
                Id = skill.Id,
                Name = skill.Name
            }).ToList();
        }

        public async Task<List<BookingDetailsDto>> GetAllBookingsAsync(string providerUserId)
        {
            var bookings = await _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Service)
                .Where(b => b.Service.Provider.UserId == providerUserId)
                .Select(b => new BookingDetailsDto
                {
                    BookingId = b.Id,
                    CustomerName = b.Customer.FullName ?? "Unknown",
                    ServiceName = b.Service.Name,
                    Price = b.Service.Price ?? 0m,
                    Date = b.Date,
                    Time = b.Time,
                    PaymentStatus = EnumConverter.GetPaymentStatus(b.PaymentStatus),
                    BookingStatus = EnumConverter.GetBookingStatus(b.BookingStatus)
                })
                .ToListAsync();

            return bookings;
        }

        public async Task<List<NotificationDto>> GetNotificationsByProviderIdAsync(string userId)
        {
            var notifications = await _context.Notifications
        .Where(n => n.UserId == userId)
        .OrderByDescending(n => n.CreatedAt)
        .Select(n => new NotificationDto
        {
            Id = n.Id,
            Type = (NotificationTypes)n.Type,
            Date = n.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss.sssZ"),
            BookingStatus = n.BookingStatus,
            Status = n.IsRead ? "read" : "notRead",
            BookingId = n.BookingId,
            CustomerName = n.Booking != null ? n.Booking.Customer.FullName : null,
            Email = n.Booking != null ? n.Booking.Customer.User.Email : null,
            PhoneNumber = n.Booking != null ? n.Booking.Customer.PhoneNumber : null,
            Amount = n.Booking != null ? n.Booking.Service.Price : null,
            PaymentStatus = n.Booking != null ? n.Booking.PaymentStatus : null,
        })
        .ToListAsync();

            return notifications;
        }

        public async Task<ProviderStatisticsDto> GetProviderStatisticsAsync(string providerId)
        {
            var services = await _context.Services
                .Include(s => s.Bookings)
                .ThenInclude(b => b.Payments)
                .Where(s => s.Provider.User.Id == providerId)
                .ToListAsync();

            if (!services.Any())
            {
                return new ProviderStatisticsDto
                {
                    TotalRevenue = 0,
                    TotalBookings = 0,
                    RevenueGrowthPercentage = 0,
                    RevenueData = new List<Dtos.RevenueData>()
                };
            }

            var totalRevenue = services
                .SelectMany(s => s.Bookings)
                .Where(b => b.BookingStatus == (int)BookingStatus.Confirmed)
                .Sum(b => b.Payments.Sum(p => p.ReleasedAmount ?? 0));

            var totalBookings = services
                .SelectMany(s => s.Bookings)
                .Count();

            var previousRevenue = services
                .SelectMany(s => s.Bookings)
                .Where(b => b.BookingStatus == (int)BookingStatus.Completed &&
                            b.CreatedAt.HasValue &&
                            b.CreatedAt.Value < DateTime.UtcNow.AddMonths(-1))
                .Sum(b => b.Payments.Sum(p => p.ReleasedAmount ?? 0));

            var revenueGrowthPercentage = previousRevenue == 0
                ? 0
                : ((totalRevenue - previousRevenue) / previousRevenue) * 100;

            var monthlyData = RevenueDataUtil.GetMonthlyRevenueData(services);

            return new ProviderStatisticsDto
            {
                TotalRevenue = totalRevenue,
                TotalBookings = totalBookings,
                RevenueGrowthPercentage = Math.Round((double)revenueGrowthPercentage, 2),
                RevenueData = monthlyData
            };
        }

        public async Task<bool> AddCompletedServiceAsync(CreateCompletedServiceDto createCompletedServiceDto, Stream? imageStream = null)
        {
            var booking = await _context.Bookings
                .Include(b => b.CompletedServices)
                .FirstOrDefaultAsync(b => b.Id == createCompletedServiceDto.BookingId);

            if (booking == null)
            {
                return false;
            }

            string? mediaUrl = null;
            if (imageStream != null)
            {
                var containerName = "completed-service-images";
                var uniqueFileName = Guid.NewGuid().ToString();
                mediaUrl = await _blobStorageUtil.UploadImageToBlobAsync(imageStream, uniqueFileName, containerName);
            }

            var completedService = new CompletedService
            {
                BookingId = booking.Id,
                Description = createCompletedServiceDto.Description,
                MediaUrl = mediaUrl,
                CompletedAt = DateTime.UtcNow
            };

            booking.CompletedServices.Add(completedService);
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<CompletedServiceDto>> GetAllCompletedServicesAsync(string providerId)
        {
            var completedServices = await _context.CompletedServices
                .Include(cs => cs.Reviews)
                .Where(cs => cs.Booking.Service.Provider.User.Id == providerId)
                .Select(cs => new CompletedServiceDto
                {
                    Id = cs.Id,
                    Description = cs.Description,
                    MediaUrl = cs.MediaUrl,
                    UserId = cs.Booking.Customer.User.Id,
                    CompletedAt = cs.CompletedAt,
                    Reviews = cs.Reviews.Select(review => new ReviewDto
                    {
                        Id = review.Id,
                        UserId = review.UserId,
                        Rating = review.Rating,
                        Comment = review.Comment,
                        CreatedAt = review.CreatedAt,
                        UserName = review.User.UserName!
                    }).ToList()
                })
                .ToListAsync();

            return completedServices;
        }

        public async Task<bool> UpdateCompletedServiceAsync(int completedServiceId, CompletedServiceDto editCompletedServiceDto, Stream? newImageStream = null)
        {
            var completedService = await _context.CompletedServices
                .Include(cs => cs.Booking)
                .FirstOrDefaultAsync(cs => cs.Id == completedServiceId);

            if (completedService == null)
            {
                return false;
            }

            completedService.Description = editCompletedServiceDto.Description;

            if (newImageStream != null)
            {
                if (!string.IsNullOrEmpty(completedService.MediaUrl))
                {
                    var oldImageFileName = completedService.MediaUrl.Split('/').Last();
                    await _blobStorageUtil.DeleteImageFromBlobAsync(oldImageFileName, "completed-service-images");
                }
                var containerName = "completed-service-images";
                var uniqueFileName = Guid.NewGuid().ToString();
                completedService.MediaUrl = await _blobStorageUtil.UploadImageToBlobAsync(newImageStream, uniqueFileName, containerName);
            }

            _context.CompletedServices.Update(completedService);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteCompletedServiceAsync(int completedServiceId)
        {
            var completedService = await _context.CompletedServices
                .FirstOrDefaultAsync(cs => cs.Id == completedServiceId);

            if (completedService == null)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(completedService.MediaUrl))
            {
                var imageFileName = completedService.MediaUrl.Split('/').Last();
                await _blobStorageUtil.DeleteImageFromBlobAsync(imageFileName, "completed-service-images");
            }

            _context.CompletedServices.Remove(completedService);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<CompletedServiceDto?> GetCompletedServiceByIdAsync(int completedServiceId)
        {
            var completedService = await _context.CompletedServices
                .Include(cs => cs.Booking)
                    .ThenInclude(b => b.Service)
                .Include(cs => cs.Reviews)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(cs => cs.Id == completedServiceId);

            if (completedService == null)
            {
                return null;
            }

            return new CompletedServiceDto
            {
                Id = completedService.Id,
                Description = completedService.Description,
                MediaUrl = completedService.MediaUrl,
                CompletedAt = completedService.CompletedAt,
                Reviews = completedService.Reviews.Select(review => new ReviewDto
                {
                    Id = review.Id,
                    UserId = review.UserId,
                    Rating = review.Rating,
                    Comment = review.Comment,
                    CreatedAt = review.CreatedAt,
                    UserName = review.User.UserName!
                }).ToList()
            };
        }
    }
}
