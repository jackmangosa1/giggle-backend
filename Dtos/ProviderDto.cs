namespace ServiceManagementAPI.Dtos
{
    public class ProviderDto
    {
        public string ProviderId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
        public string? Bio { get; set; }
        public double AverageRating { get; set; }
        public decimal AveragePrice { get; set; }
    }
}
