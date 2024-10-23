using Microsoft.EntityFrameworkCore;
using ServiceManagementAPI.Data;
using ServiceManagementAPI.Dtos;
using ServiceManagementAPI.Enums;
using ServiceManagementAPI.Utils;
namespace ServiceManagementAPI.Repositories.CustomerRepository
{
    public class CustomerRepository : ICustomerRepository
    {

        private readonly ServiceManagementDbContext _context;
        private readonly BlobStorageUtil _blobStorageUtil;

        public CustomerRepository(ServiceManagementDbContext context, BlobStorageUtil blobStorageUtil)
        {
            _context = context;
            _blobStorageUtil = blobStorageUtil;
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
                var imageUrl = await _blobStorageUtil.UploadImageToBlobAsync(imageStream, updateCustomerProfileDto.ImageFileName, containerName);
                customer.ProfilePictureUrl = imageUrl;
            }

            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
