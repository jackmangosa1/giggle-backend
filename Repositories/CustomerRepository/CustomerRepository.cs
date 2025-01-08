using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ServiceManagementAPI.Dtos;
using ServiceManagementAPI.Entities;
using ServiceManagementAPI.Enums;
using ServiceManagementAPI.Hubs;
using ServiceManagementAPI.Utils;
namespace ServiceManagementAPI.Repositories.CustomerRepository
{
    public class CustomerRepository : ICustomerRepository
    {

        private readonly ServiceManagementDbContext _context;
        private readonly BlobStorageUtil _blobStorageUtil;
        private readonly IHubContext<NotificationHub> _hubContext;



        public CustomerRepository(ServiceManagementDbContext context, BlobStorageUtil blobStorageUtil, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _blobStorageUtil = blobStorageUtil;
            _hubContext = hubContext;
        }

        public async Task<CustomerProfileDto?> GetCustomerProfileAsync(string customerId)
        {

            var customer = await _context.Customers
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.User.Id == customerId);

            if (customer == null)
            {
                return null;
            }

            PaymentMethod? preferredPaymentMethod = customer.PreferredPaymentMethod.HasValue
        ? (PaymentMethod?)customer.PreferredPaymentMethod.Value
        : null;

            var customerProfile = new CustomerProfileDto
            {
                CustomerId = customer.Id,
                FullName = customer.FullName,
                Address = customer.Address,
                PhoneNumber = customer.PhoneNumber,
                PreferredPaymentMethod = preferredPaymentMethod,
                ProfilePictureUrl = customer.ProfilePictureUrl,
                Username = customer.User.UserName,
                Email = customer.User.Email
            };

            return customerProfile;
        }


        public async Task<bool> UpdateCustomerProfileAsync(string customerId, UpdateCustomerProfileDto updateCustomerProfileDto, Stream imageStream = null!)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.User.Id == customerId);
            if (customer == null)
            {
                return false;
            }

            if (updateCustomerProfileDto.FullName != null)
                customer.FullName = updateCustomerProfileDto.FullName;

            if (updateCustomerProfileDto.Address != null)
                customer.Address = updateCustomerProfileDto.Address;

            if (updateCustomerProfileDto.PhoneNumber != null)
                customer.PhoneNumber = updateCustomerProfileDto.PhoneNumber;

            if (updateCustomerProfileDto.PreferredPaymentMethod.HasValue)
                customer.PreferredPaymentMethod = (int)updateCustomerProfileDto.PreferredPaymentMethod;

            if (imageStream != null)
            {
                var containerName = "profile-pictures";
                var fileName = customer.FullName;
                var imageUrl = await _blobStorageUtil.UploadImageToBlobAsync(imageStream, fileName, containerName);
                customer.ProfilePictureUrl = imageUrl;
            }

            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();

            return true;
        }


        public async Task<bool> CreateBookingAsync(BookingDto bookingDto)
        {
            DateOnly date = DateOnly.Parse(bookingDto.Date);
            TimeOnly time = TimeOnly.Parse(bookingDto.Time);
            var service = await _context.Services
                .Include(s => s.Provider)
                .FirstOrDefaultAsync(s => s.Id == bookingDto.ServiceId);

            var customer = _context.Customers.FirstOrDefault(c => c.UserId == bookingDto.CustomerId);

            if (service == null || customer == null || service.Provider == null)
            {
                return false;
            }

            var booking = new Booking
            {
                ServiceId = bookingDto.ServiceId,
                CustomerId = customer.Id,
                Date = date,
                Time = time,
                BookingStatus = (int)BookingStatus.Pending,
                PaymentStatus = (int)PaymentStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _context.Bookings.Add(booking);

            var notification = new Notification
            {
                UserId = service.Provider.UserId,
                Type = (int)NotificationTypes.NewBooking,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);

            await _context.SaveChangesAsync();

            string serviceName = service.Name;
            string notificationMessage = $"You have a new booking for {serviceName}";

            await _hubContext.Clients.User(service.ProviderId.ToString())
                .SendAsync("ReceiveNotification", notificationMessage);

            return true;
        }

        public async Task<List<ProviderDto>> SearchProvidersAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return new List<ProviderDto>();
            }

            searchTerm = searchTerm.Trim();

            var providers = await _context.Providers
                .Include(p => p.Services)
                    .ThenInclude(s => s.Category)
                .Include(p => p.Skills)
                .Where(p =>
                    p.DisplayName!.Contains(searchTerm) ||
                    p.Services.Any(s =>
                        s.Name.Contains(searchTerm) ||
                        s.Category.Name.Contains(searchTerm)) ||
                    p.Skills.Any(skill => skill.Name.Contains(searchTerm))
                )
                .Select(p => new ProviderDto
                {
                    ProviderId = p.User.Id,
                    DisplayName = p.DisplayName!,
                    ProfilePictureUrl = p.ProfilePictureUrl,
                    Bio = p.Bio,
                })
                .ToListAsync();

            return providers;
        }

        public async Task<List<NotificationDto>> GetNotificationsByUserIdAsync(string userId)
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

        public async Task<bool> ProcessPaymentAsync(SavePaymentDto savePaymentDto)
        {
            var booking = await _context.Bookings
                .Include(b => b.Payments)
                .Include(b => b.Customer)
                .Include(b => b.Service)
                    .ThenInclude(s => s.Provider)
                .FirstOrDefaultAsync(b => b.Id == savePaymentDto.BookingId);

            if (booking == null) return false;

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.User.Id == savePaymentDto.CustomerId);

            if (customer == null) return false;

            var payment = booking.Payments.FirstOrDefault(p => p.TransactionId == savePaymentDto.TransactionId);

            if (payment != null)
            {
                payment.PaymentStatus = (int)savePaymentDto.PaymentStatus;
                payment.PaymentDate = savePaymentDto.PaymentDate;

                if (savePaymentDto.PaymentStatus == PaymentStatus.Escrow)
                {
                    booking.PaymentStatus = (int)PaymentStatus.Escrow;
                    booking.EscrowAmount = payment.PaymentAmount;
                }
            }
            else
            {
                payment = new Payment
                {
                    TransactionId = savePaymentDto.TransactionId,
                    BookingId = savePaymentDto.BookingId,
                    CustomerId = customer.Id,
                    PaymentAmount = savePaymentDto.Amount,
                    PaymentMethod = (int)savePaymentDto.PaymentMethod,
                    PaymentStatus = (int)savePaymentDto.PaymentStatus,
                    PaymentDate = savePaymentDto.PaymentDate,
                    EscrowAmount = savePaymentDto.PaymentStatus == PaymentStatus.Escrow ? savePaymentDto.Amount : 0,
                    ReleasedAmount = 0
                };

                _context.Payments.Add(payment);

                if (savePaymentDto.PaymentStatus == PaymentStatus.Escrow)
                {
                    booking.PaymentStatus = (int)PaymentStatus.Escrow;
                    booking.EscrowAmount = savePaymentDto.Amount;
                }
            }
            var notification = new Notification
            {
                UserId = booking.Service.Provider.UserId,
                Type = (int)NotificationTypes.PaymentStatusChange,
                BookingStatus = (int)savePaymentDto.PaymentStatus,
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                BookingId = savePaymentDto.BookingId,
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            string notificationMessage = $"Your payment for booking #{savePaymentDto.BookingId} has been processed successfully";

            await _hubContext.Clients.All.SendAsync("ReceiveNotification", notificationMessage);

            _context.Bookings.Update(booking);
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

            if (status == BookingStatus.Confirmed)
            {
                booking.BookingStatus = (int)BookingStatus.Confirmed;
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

            string notificationMessage = $"Service completion for the booking {booking.Service.Name} has been confirmed.";

            await _hubContext.Clients.All.SendAsync("ReceiveNotification", notificationMessage);

            return true;
        }
    }
}
