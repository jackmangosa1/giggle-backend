using System.ComponentModel.DataAnnotations;

namespace ServiceManagementAPI.Dtos
{
    public class UpdateCustomerProfileDto
    {
        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, ErrorMessage = "Full name cannot be longer than 100 characters.")]
        public string FullName { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Address cannot be longer than 200 characters.")]
        public string Address { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Preferred payment method cannot be longer than 50 characters.")]
        public string PreferredPaymentMethod { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string PhoneNumber { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "Image file name cannot be longer than 255 characters.")]
        public string ImageFileName { get; set; } = string.Empty;
    }
}
