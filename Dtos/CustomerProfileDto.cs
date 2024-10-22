namespace ServiceManagementAPI.Dtos
{
    public class CustomerProfileDto
    {
        public int CustomerId { get; set; }
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? PreferredPaymentMethod { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
    }
}
