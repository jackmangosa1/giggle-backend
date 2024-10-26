using ServiceManagementAPI.Enums;

namespace ServiceManagementAPI.Dtos
{
    public class ServiceDto
    {
        public int? Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string CategoryName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public decimal? Price { get; set; }

        public PriceType PriceType { get; set; }

        public string? MediaUrl { get; set; }
    }
}
