using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ServiceManagementAPI.Data;
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

        public async Task<CustomerProfileDto?> GetCustomerProfileAsync(int customerId)
        {

            var customer = await _context.Customers
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == customerId);

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


        public async Task<bool> UpdateCustomerProfileAsync(int customerId, UpdateCustomerProfileDto updateCustomerProfileDto, Stream imageStream = null!)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == customerId);
            if (customer == null)
            {
                return false;
            }

            customer.FullName = updateCustomerProfileDto.FullName;
            customer.Address = updateCustomerProfileDto.Address;
            customer.PhoneNumber = updateCustomerProfileDto.PhoneNumber;
            customer.PreferredPaymentMethod = (int?)updateCustomerProfileDto.PreferredPaymentMethod;


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
            var service = await _context.Services
                .Include(s => s.Provider)
                .FirstOrDefaultAsync(s => s.Id == bookingDto.ServiceId);

            var customer = await _context.Customers.FindAsync(bookingDto.CustomerId);

            if (service == null || customer == null || service.Provider == null)
            {
                return false;
            }

            var booking = new Booking
            {
                ServiceId = bookingDto.ServiceId,
                CustomerId = bookingDto.CustomerId,
                Date = bookingDto.Date,
                Time = bookingDto.Time,
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

    }
}
