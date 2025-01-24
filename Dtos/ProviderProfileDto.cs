namespace ServiceManagementAPI.Dtos
{
    public class ProviderProfileDto
    {
        public string Id { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        public string? Bio { get; set; }
        public List<string>? Skills { get; set; }
        public List<ServiceDto> Services { get; set; } = new List<ServiceDto>();
        public List<CompletedServiceDto> CompletedServices { get; set; } = new List<CompletedServiceDto>();
        public string? ProfilePictureUrl { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
    }
}
